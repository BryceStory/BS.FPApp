using FiiiPay.Data;
using Newtonsoft.Json;
using System;
using System.Transactions;
using FiiiPay.Entities;
using FiiiPay.Framework.Queue;
using log4net;

namespace FiiiPay.MessageWorkerService
{
    public class WithdrawSubmitComponent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(WithdrawSubmitComponent));

        public void SubmitWithdrawal(CreateWithdrawModel model)
        {
            if (model.AccountType == AccountTypeEnum.Merchant)
            {
                var withdrawalDAC = new MerchantWithdrawalDAC();
                var withdrawal = withdrawalDAC.GetById(model.WithdrawalId);
                if (withdrawal == null || withdrawal.Status != Entities.Enums.TransactionStatus.UnSubmit)
                {
                    LogHelper.Info("Invalid withdrawal id or invalid withdrawal status");
                    return;
                }

                var agent = new FiiiFinanceAgent();
                
                try
                {
                    var requestInfo = agent.CreateWithdraw(model);

                    if (requestInfo.Code == 0)
                    {
                        withdrawalDAC.WithdrawalSubmited(model.WithdrawalId, requestInfo.RequestID,
                            requestInfo.TransactionId);
                    }
                    else if (requestInfo.Code == 20002 || requestInfo.Code == 300001)
                    {
                        var merchantWalletDac = new MerchantWalletDAC();
                        var merchantWallet = merchantWalletDac.GetById(withdrawal.MerchantWalletId);
                        using (var scope = new TransactionScope())
                        {
                            try
                            {
                                withdrawalDAC.RejectById(withdrawal.Id, "Invalid address");
                                merchantWalletDac.Unfreeze(merchantWallet.Id, withdrawal.Amount);
                                new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                                {
                                    WalletId = merchantWallet.Id,
                                    Action = UserWalletStatementAction.Withdrawal,
                                    Amount = withdrawal.Amount,
                                    Balance = merchantWallet.Balance + withdrawal.Amount,
                                    Timestamp = DateTime.UtcNow
                                });
                                scope.Complete();
                            }
                            catch (Exception exception)
                            {
                                _log.Error(exception);
                            }
                        }
                    }
                    else
                    {
                        RabbitMQSender.SendMessage("WithdrawSubmit_Delay", model);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"UpdateTransactionId erorr, message:{JsonConvert.SerializeObject(model)}", ex);

                    RabbitMQSender.SendMessage("WithdrawSubmit_Delay", model);
                }
            }
            else if (model.AccountType == AccountTypeEnum.User)
            {
                var withdrawalDAC = new UserWithdrawalDAC();
                var withdrawal = withdrawalDAC.GetById(model.WithdrawalId);
                if (withdrawal == null || withdrawal.Status != Entities.Enums.TransactionStatus.UnSubmit)
                {
                    LogHelper.Info("Invalid withdrawal id or invalid withdrawal status");
                    return;
                }

                var agent = new FiiiFinanceAgent();

                try
                {
                    var requestInfo = agent.CreateWithdraw(model);

                    if (requestInfo.Code == 0)
                    {
                        withdrawalDAC.WithdrawalSubmited(model.WithdrawalId, requestInfo.RequestID, requestInfo.TransactionId);
                    }
                    else if (requestInfo.Code == 20002 || requestInfo.Code == 300001)
                    {
                        var userWalletDac = new UserWalletDAC();
                        var userWallet = userWalletDac.GetById(withdrawal.UserWalletId);
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                withdrawalDAC.RejectById(withdrawal.Id, "Invalid address");
                                userWalletDac.Unfreeze(withdrawal.UserWalletId, withdrawal.Amount);

                                new UserWalletStatementDAC().Insert(new Entities.UserWalletStatement
                                {
                                    WalletId = userWallet.Id,
                                    Action = Entities.UserWalletStatementAction.Withdrawal,
                                    Amount = withdrawal.Amount,
                                    Balance = userWallet.Balance + withdrawal.Amount,
                                    FrozenAmount = -withdrawal.Amount,
                                    FrozenBalance = userWallet.FrozenBalance - withdrawal.Amount,
                                    Timestamp = DateTime.UtcNow
                                });

                                scope.Complete();
                            }
                            catch (Exception exception)
                            {
                                _log.Error(exception);
                            }
                        }
                    }
                    else
                    {
                        RabbitMQSender.SendMessage("WithdrawSubmit_Delay", model);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"UpdateTransactionId erorr, message:{JsonConvert.SerializeObject(model)}", ex);
                    RabbitMQSender.SendMessage("WithdrawSubmit_Delay", model);
                }
            }
            else
            {
                LogHelper.Info("Invalid AccountType");
            }
        }
    }
}
