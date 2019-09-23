using System;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Data;
using FiiiPay.MiningConfirmService.Factories;
using FiiiPay.MiningConfirmService.Models;
using log4net;

namespace FiiiPay.MiningConfirmService.Components
{
    public class MiningComponent
    {
        private static readonly object obj = new object();
        private readonly ILog _logger = LogManager.GetLogger("LogicError");
        public void MiningConfirmed(MiningConfirmedModel model)
        {
            var crypto = new CryptocurrencyDAC().GetByCode("FIII");
            lock (obj)
            {
                try
                {

                    if (model.AccountType == (byte)AccountType.User)
                        AccountMiningConfirmed(crypto.Id, model, new UserWalletFactory(), new UserWalletStatementFactory());
                    else if (model.AccountType == (byte)AccountType.Merchant)
                        AccountMiningConfirmed(crypto.Id, model, new MerchantWalletFactory(), new MerchantWalletStatementFactory());
                }
                catch (Exception ex)
                {
                    _logger.Error($"Mining-Confirmed - ErrorMessage - {ex.Message}", ex);
                }
            }
        }

        private void AccountMiningConfirmed(int cryptoId, MiningConfirmedModel model, WalletFactory _walletFactory, WalletStatementFactory _walletStatementFactory)
        {
            var wallet = _walletFactory.GetByAccountId(model.AccountId, cryptoId);
            if (wallet == null)
            {
                MiningConfirmedFaild(model, "Wallet is null");
                return;
            }
            if (wallet.FrozenBalance < model.Amount)
            {
                MiningConfirmedFaild(model, "Insufficient frozenbalance");
                return;
            }
            try
            {
                _walletFactory.Unfreeze(wallet.Id, model.Amount);
                _walletStatementFactory.Insert(new WalletStatement
                {
                    Action = UserWalletStatementAction.AwardDeposit,
                    Amount = model.Amount,
                    Balance = wallet.Balance + model.Amount,
                    FrozenAmount = -model.Amount,
                    FrozenBalance = wallet.FrozenBalance - model.Amount,
                    Remark = null,
                    Timestamp = DateTime.UtcNow,
                    WalletId = wallet.Id
                });
            }
            catch (Exception ex)
            {
                MiningConfirmedFaild(model, (ex.InnerException == null ? ex.Message : ex.InnerException.Message));
            }
        }

        private void MiningConfirmedFaild(MiningConfirmedModel model, string message)
        {
            _logger.Info($"Mining-Confirmed - ErrorMessage:{message},accountType:{model.AccountType}, accountId:{model.AccountId},amount:{model.Amount}");
        }
    }
}
