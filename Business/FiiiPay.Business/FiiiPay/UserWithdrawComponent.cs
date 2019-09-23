using FiiiPay.Data;
using FiiiPay.DTO.Withdraw;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using FiiiPay.Business.Properties;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Exceptions;
using log4net;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Enums;
using TransactionStatus = FiiiPay.Entities.Enums.TransactionStatus;

namespace FiiiPay.Business
{
    public class UserWithdrawComponent : BaseComponent
    {
        //private string CreateOrderno()
        //{
        //    return DateTime.Now.ToUnixTime() + new Random().Next(0, 100).ToString("00");
        //}

        public WithdrawDetailOM Detail(UserAccount user, long id, bool isZH)
        {
            var data = new UserWithdrawalDAC().GetById(user.Id, id);
            //var agent = FiiiFinanceFactory.GetByCountryId(user.CountryId.Value);
            //var markPrice = agent.GetMarketPrice(user.FiatCurrency, data.CryptoCode);
            var wallet = new UserWalletDAC().GetById(data.UserWalletId);
            var coin = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var fee = new UserWithdrawalFeeDAC().GetByWithdrawalId(id) ?? new UserWithdrawalFee { Amount = 0 };
            data.Amount = data.Amount - fee.Fee;

            var om = new WithdrawDetailOM
            {
                Address = data.Address,
                Tag = data.Tag,
                CryptoAmount = data.Amount.ToString(coin.DecimalPlace),
                //FiatAmount = (markPrice.Price * data.Amount.Value).ToString(2),
                Code = coin.Code,
                NeedTag = coin.NeedTag,
                //FiatCurrency = user.FiatCurrency,
                Id = data.Id,
                OrderNo = data.OrderNo,
                StatusStr = new UserStatementComponent().GetStatusStr(1, (int)data.Status, isZH),
                Status = data.Status,
                Timestamp = data.Timestamp.ToUnixTime().ToString(),
                TransactionFee = data.WithdrawalFee.ToString(coin.DecimalPlace),
                Type = Resources.Withdrawal,
                SelfPlatform = data.SelfPlatform,
                TransactionId = data.SelfPlatform ? "-" : data.TransactionId ?? "-",
                Remark = data.Remark// data.Status == TransactionStatus.Cancelled ? MessageResources.提币失败备注 : null
            };

            bool showCheckTime = coin.Code != "XRP";

            if (showCheckTime && !data.SelfPlatform &&
                om.Status == TransactionStatus.Pending &&
                data.RequestId.HasValue)
            {
                var agent = new FiiiFinanceAgent();
                var statusInfo = agent.GetStatus(data.RequestId.Value);
                om.CheckTime = $"{statusInfo.TotalConfirmation}/{statusInfo.MinRequiredConfirmation}";
                om.TransactionId = statusInfo.TransactionID;
            }

            return om;
        }

        public void VerifyWithdrawPIN(Guid accountId, string pin)
        {
            UserAccount user = new UserAccountDAC().GetById(accountId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, accountId.ToString(), user.Pin, pin);

            var model = new WithdrawVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UserWithdraw"), SystemPlatform.FiiiPay, accountId.ToString(), model);
        }

        public void VerifyWithdrawCombine(Guid accountId, string smsCode, string googleCode, string divisionCode)
        {
            UserAccount user = new UserAccountDAC().GetById(accountId);
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode },
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.GooogleAuthenticator, Code=googleCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = user.ValidationFlag,
                GoogleAuthSecretKey = user.AuthSecretKey
            };

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPay, user.Id.ToString(), userSecrets, options, divisionCode);

            var model = SecurityVerify.GetModel<WithdrawVerify>(new CustomVerifier("UserWithdraw"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UserWithdraw"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public WithdrawOM Withdraw(UserAccount user, WithdrawIM im, string clientIP)
        {
            SecurityVerify.Verify<WithdrawVerify>(new CustomVerifier("UserWithdraw"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });


            if (user.L1VerifyStatus != VerifyStatus.Certified)
                throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.EMNeedLV1Verfied);

            var cryptocurrency = new CryptocurrencyDAC().GetById(im.CoinId);

            if (!cryptocurrency.Status.HasFlag(CryptoStatus.Withdrawal) || cryptocurrency.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            CryptoAddressValidation.ValidateAddress(cryptocurrency.Code, im.Address);
            if (!string.IsNullOrEmpty(im.Tag))
            {
                CryptoAddressValidation.ValidateTag(cryptocurrency.Code, im.Tag);
            }
            //else if (cryptocurrency.NeedTag)
            //{
            //    throw new CommonException(ReasonCode.NEED_INPUT_TAG, GeneralResources.EMNeedInputTag);
            //}

            if (im.Amount <= 0)
            {
                throw new ApplicationException(MessageResources.AmountGreater);
            }

            var IsAllowWithdrawal = user.IsAllowWithdrawal ?? true;
            if (!IsAllowWithdrawal)
            {
                throw new CommonException(ReasonCode.Not_Allow_Withdrawal, MessageResources.WithdrawalDisabled);
            }

            var fromWallet = new UserWalletDAC().GetByAccountId(user.Id, im.CoinId);


            //var profile = new UserProfileAgent().GetUserProfile(user.Id);

            //标准化这个金额，防止超过8位
            im.Amount = im.Amount.ToSpecificDecimal(cryptocurrency.DecimalPlace);

            var mastSettings = new MasterSettingDAC().SelectByGroup("UserWithdrawal");

            var Withdrawal_PerTx_Limit_User_NotVerified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerTx_Limit_User_NotVerified").Value);
            var Withdrawal_PerDay_Limit_User_NotVerified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerDay_Limit_User_NotVerified").Value);
            var Withdrawal_PerMonth_Limit_User_NotVerified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerMonth_Limit_User_NotVerified").Value);

            var Withdrawal_PerTx_Limit_User_Lv1Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerTx_Limit_User_Lv1Verified").Value);
            var Withdrawal_PerDay_Limit_User_Lv1Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerDay_Limit_User_Lv1Verified").Value);
            var Withdrawal_PerMonth_Limit_User_Lv1Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerMonth_Limit_User_Lv1Verified").Value);

            var Withdrawal_PerTx_Limit_User_Lv2Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerTx_Limit_User_Lv2Verified").Value);
            var Withdrawal_PerDay_Limit_User_Lv2Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerDay_Limit_User_Lv2Verified").Value);
            var Withdrawal_PerMonth_Limit_User_Lv2Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerMonth_Limit_User_Lv2Verified").Value);

            var MinAmount = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_MinAmount").Value);

            var merchantWalletDac = new MerchantWalletDAC();
            var userWalletDac = new UserWalletDAC();

            var isWithdrawToInside = (merchantWalletDac.IsMerchantWalletAddress(im.Address) || userWalletDac.IsUserWalletAddress(im.Address));
            if (isWithdrawToInside)
            {
                MinAmount = Convert.ToDecimal(mastSettings.First(e => e.Name == "UserWithdrawal_ToUser_MinAmount").Value);
            }

            var exchangeRate = GetMarketPrice(user.CountryId, cryptocurrency.Code, "USD");

            var _minAmount = (MinAmount / exchangeRate).ToSpecificDecimal(cryptocurrency.DecimalPlace);

            if (im.Amount < _minAmount)
                throw new ApplicationException(MessageResources.MinWidrawalError);

            var withdrawalDAC = new UserWithdrawalDAC();
            var today = DateTime.UtcNow.Date;
            decimal dailyWithdrawal = withdrawalDAC.DailyWithdrawal(user.Id, im.CoinId, today);
            decimal monthlyWithdrawal = withdrawalDAC.MonthlyWithdrawal(user.Id, im.CoinId, new DateTime(today.Year, today.Month, 1));

            var PerDayLimit = 0M;
            var PerMonthLimit = 0M;

            if (user.L1VerifyStatus == VerifyStatus.Certified && user.L2VerifyStatus == VerifyStatus.Certified)
            {
                PerDayLimit = Withdrawal_PerDay_Limit_User_Lv2Verified;
                PerMonthLimit = Withdrawal_PerMonth_Limit_User_Lv2Verified;
            }
            else if (user.L1VerifyStatus == VerifyStatus.Certified && user.L2VerifyStatus != VerifyStatus.Certified)
            {
                PerDayLimit = Withdrawal_PerDay_Limit_User_Lv1Verified;
                PerMonthLimit = Withdrawal_PerMonth_Limit_User_Lv1Verified;
            }
            else if (user.L1VerifyStatus != VerifyStatus.Certified && user.L2VerifyStatus != VerifyStatus.Certified)
            {
                PerDayLimit = Withdrawal_PerDay_Limit_User_NotVerified;
                PerMonthLimit = Withdrawal_PerMonth_Limit_User_NotVerified;
            }
            if ((PerDayLimit / exchangeRate - dailyWithdrawal).ToSpecificDecimal(cryptocurrency.DecimalPlace) < im.Amount)
            {
                throw new ApplicationException(MessageResources.TodayWidrawalLimit);
            }
            if ((PerMonthLimit / exchangeRate - monthlyWithdrawal).ToSpecificDecimal(cryptocurrency.DecimalPlace) < im.Amount)
            {
                throw new ApplicationException(MessageResources.MonthWithdrawalLimit);
            }

            var fromWithdraw = new UserWithdrawal
            {
                UserAccountId = user.Id,
                UserWalletId = fromWallet.Id,
                Address = im.Address,
                Tag = im.Tag,
                Amount = im.Amount,
                Status = TransactionStatus.UnSubmit,
                Timestamp = DateTime.UtcNow,
                OrderNo = IdentityHelper.OrderNo(),
                CryptoCode = fromWallet.CryptoCode,
                CryptoId = fromWallet.CryptoId
            };

            //是否是商户地址
            var toMerchantWallet = merchantWalletDac.GetByAddressAndCrypto(im.CoinId, im.Address, im.Tag);
            if (toMerchantWallet != null)
            {
                //if (toMerchantWallet.CryptoId != im.CoinId)
                //    throw new CommonException(ReasonCode.GENERAL_ERROR, GeneralResources.EMInvalidAddress);
                //return WithdrawalToMerchantAccount(fromWallet, fromWithdraw, fromWithdrawFee, toMerchantWallet, cryptocurrency);
                //042018
                throw new CommonException(ReasonCode.CAN_NOT_WITHDRAW_TO_FiiiPOS, MessageResources.FiiiPayCantWithdrawToFiiiPOS);
            }

            //是否是用户地址
            var toUserWallet = userWalletDac.GetByAddressAndCrypto(im.CoinId, im.Address, im.Tag);
            if (toUserWallet != null)
            {
                if (toUserWallet.UserAccountId == user.Id)
                    throw new CommonException(ReasonCode.CANNOT_TRANSFER_TO_YOURSELF, MessageResources.WithdrawalToSelfError);

                var toFiiiPayMinWithdrawAmount = GetToFiiiPayMinAmount(Convert.ToDecimal(mastSettings.First(e => e.Name == "UserWithdrawal_ToUser_MinAmount").Value), exchangeRate, cryptocurrency.DecimalPlace);
                if (im.Amount < toFiiiPayMinWithdrawAmount)
                    throw new CommonException(10000, MessageResources.MinWidrawalError);

                return WithdrawalToUserAccount(fromWallet, fromWithdraw, toUserWallet);
            }

            var tier = cryptocurrency.Withdrawal_Tier ?? 0;
            var fee = im.Amount * tier + (cryptocurrency.Withdrawal_Fee ?? 0);
            fee = fee.ToSpecificDecimal(cryptocurrency.DecimalPlace);
            var actualAmount = im.Amount - fee;
            if (fromWallet.Balance < im.Amount)
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            if (actualAmount <= 0)
            {
                throw new CommonException(ReasonCode.GENERAL_ERROR, MessageResources.ArrivalAmountError);
            }

            //地址是FiiiPay的地址，但tag找不到
            if (cryptocurrency.NeedTag && isWithdrawToInside)
            {
                return CancelWithdrawal(fromWithdraw);
            }

            ILog _logger = LogManager.GetLogger("LogicError");

            //如果都不是，向Finance申请提现

            //var agent = new FiiiFinanceAgent();

            var requestModel = new CreateWithdrawModel
            {
                AccountID = user.Id,
                AccountType = AccountTypeEnum.User,
                CryptoName = cryptocurrency.Code,
                ReceivingAddress = im.Address,
                DestinationTag = im.Tag,
                Amount = actualAmount,
                IPAddress = clientIP,
                TransactionFee = fee
            };

            using (var scope = new TransactionScope())
            {
                try
                {
                    fromWithdraw.Id = withdrawalDAC.Create(fromWithdraw);
                    var fromWithdrawFee = new UserWithdrawalFee
                    {
                        Amount = im.Amount,
                        Fee = fee,
                        Timestamp = DateTime.UtcNow,
                        WithdrawalId = fromWithdraw.Id
                    };

                    new UserTransactionDAC().Insert(new UserTransaction
                    {
                        Id = Guid.NewGuid(),
                        AccountId = fromWithdraw.UserAccountId,
                        CryptoId = fromWithdraw.CryptoId,
                        CryptoCode = fromWithdraw.CryptoCode,
                        Type = UserTransactionType.Withdrawal,
                        DetailId = fromWithdraw.Id.ToString(),
                        Status = (byte)fromWithdraw.Status,
                        Timestamp = fromWithdraw.Timestamp,
                        Amount = fromWithdraw.Amount,
                        OrderNo = fromWithdraw.OrderNo
                    });

                    new UserWithdrawalFeeDAC().Create(fromWithdrawFee);

                    userWalletDac.Freeze(fromWallet.Id, im.Amount);

                    new UserWalletStatementDAC().Insert(new UserWalletStatement
                    {
                        WalletId = fromWallet.Id,
                        Action = UserWalletStatementAction.Withdrawal,
                        Amount = 0 - im.Amount,
                        Balance = fromWallet.Balance - im.Amount,
                        FrozenAmount = im.Amount,
                        FrozenBalance = fromWallet.FrozenBalance + im.Amount,
                        Timestamp = DateTime.UtcNow
                    });

                    requestModel.WithdrawalId = fromWithdraw.Id;
                    Framework.Queue.RabbitMQSender.SendMessage("WithdrawSubmit", requestModel);

                    scope.Complete();
                }
                catch (CommonException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Info($"Withdraw CreateWithdrawRequest faild.WithdrawID:{fromWithdraw.Id},OrderNo:{fromWithdraw.OrderNo}.request Parameter - {requestModel}. Error message:{ex.Message}");
                    throw;
                }
            }

            return new WithdrawOM
            {
                OrderId = fromWithdraw.Id,
                OrderNo = fromWithdraw.OrderNo,
                Timestamp = fromWithdraw.Timestamp.ToUnixTime().ToString()
            };
        }

        public bool ManalWithdraw(long id)
        {
            var withdrawalDac = new UserWithdrawalDAC();
            var withdrawal = withdrawalDac.GetById(id);
            if (withdrawal != null)
            {
                if (withdrawal.Status == TransactionStatus.UnSubmit && withdrawal.RequestId == null)
                {
                    var model = new CreateWithdrawModel
                    {
                        AccountID = withdrawal.UserAccountId,
                        CryptoName = withdrawal.CryptoCode,
                        AccountType = AccountTypeEnum.User,
                        ReceivingAddress = withdrawal.Address,
                        DestinationTag = withdrawal.Tag,
                        Amount = withdrawal.Amount,
                        IPAddress = "207.226.141.205",
                        TransactionFee = 0,
                        WithdrawalId = withdrawal.Id
                    };

                    Framework.Queue.RabbitMQSender.SendMessage("WithdrawSubmit", model);
                }
            }

            return false;
        }

        private WithdrawOM WithdrawalToUserAccount(UserWallet fromWallet, UserWithdrawal fromWithdraw, UserWallet toWallet)
        {
            var mastSettings = new MasterSettingDAC().SelectByGroup("UserWithdrawal");
            UserDeposit model = new UserDeposit();
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                fromWithdraw.Status = TransactionStatus.Confirmed;
                fromWithdraw.TransactionId = toWallet.UserAccountId.ToString("N");
                fromWithdraw.SelfPlatform = true;//平台内提币
                fromWithdraw.Id = new UserWithdrawalDAC().Create(fromWithdraw);

                new UserTransactionDAC().Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = fromWithdraw.UserAccountId,
                    CryptoId = fromWallet.CryptoId,
                    CryptoCode = fromWallet.CryptoCode,
                    Type = UserTransactionType.Withdrawal,
                    DetailId = fromWithdraw.Id.ToString(),
                    Status = (byte)fromWithdraw.Status,
                    Timestamp = fromWithdraw.Timestamp,
                    Amount = fromWithdraw.Amount,
                    OrderNo = fromWithdraw.OrderNo
                });

                var fromWithdrawFee = new UserWithdrawalFee
                {
                    Amount = fromWithdraw.Amount,
                    WithdrawalId = fromWithdraw.Id,
                    Fee = fromWithdraw.Amount *
                          Convert.ToDecimal(mastSettings.First(e => e.Name == "UserWithdrawal_ToUser").Value),
                    Timestamp = DateTime.UtcNow
                };

                new UserWithdrawalFeeDAC().Create(fromWithdrawFee);

                new UserWalletDAC().Decrease(fromWallet.Id, fromWithdraw.Amount);

                new UserWalletStatementDAC().Insert(new UserWalletStatement
                {
                    WalletId = fromWallet.Id,
                    Action = MerchantWalletStatementAction.Withdrawal,
                    Amount = -fromWithdraw.Amount,
                    Balance = fromWallet.Balance - fromWithdraw.Amount,
                    FrozenAmount = 0,
                    FrozenBalance = fromWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });

                //充币
                var amount = fromWithdraw.Amount - fromWithdrawFee.Fee;
                if (amount <= 0)
                {
                    throw new CommonException(ReasonCode.GENERAL_ERROR, MessageResources.ArrivalAmountError);
                }

                new UserWalletDAC().Increase(toWallet.Id, amount);
                new UserWalletStatementDAC().Insert(new UserWalletStatement
                {
                    WalletId = toWallet.Id,
                    Action = UserWalletStatementAction.Deposit,
                    Amount = amount,
                    Balance = toWallet.Balance + amount,
                    FrozenAmount = 0,
                    FrozenBalance = toWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });
                
                model = new UserDepositDAC().Insert(new UserDeposit
                {
                    UserAccountId = toWallet.UserAccountId,
                    UserWalletId = toWallet.Id,
                    FromAddress = fromWallet.Address,
                    FromTag = fromWallet.Tag,
                    ToAddress = toWallet.Address,
                    ToTag = toWallet.Tag,
                    Amount = amount,
                    Status = TransactionStatus.Confirmed,
                    Timestamp = DateTime.UtcNow,
                    OrderNo = IdentityHelper.OrderNo(),
                    TransactionId = fromWallet.UserAccountId.ToString("N"),
                    SelfPlatform = true,
                    CryptoCode = fromWallet.CryptoCode
                });
                new UserTransactionDAC().Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = model.UserAccountId,
                    CryptoId = fromWallet.CryptoId,
                    CryptoCode = fromWallet.CryptoCode,
                    Type = UserTransactionType.Deposit,
                    DetailId = model.Id.ToString(),
                    Status = (byte)model.Status,
                    Timestamp = model.Timestamp,
                    Amount = model.Amount,
                    OrderNo = model.OrderNo
                });

                scope.Complete();
            }

            UserMSMQ.PubUserWithdrawCompleted(fromWithdraw.Id, 0);
            UserMSMQ.PubUserDeposit(model.Id, 0);

            return new WithdrawOM
            {
                OrderId = fromWithdraw.Id,
                OrderNo = fromWithdraw.OrderNo,
                Timestamp = fromWithdraw.Timestamp.ToUnixTime().ToString()
            };
        }

        //private WithdrawOM WithdrawalToMerchantAccount(UserWallet fromWallet, UserWithdrawal fromWithdraw, UserWithdrawalFee fromWithdrawFee, MerchantWallet toWallet, Cryptocurrency cryptocurrency)
        //{
        //    var mastSettings = new MasterSettingDAC().SelectByGroup("UserWithdrawal");
        //    using (var scope = new TransactionScope())
        //    {
        //        //提币
        //        fromWithdraw.Status = Framework.Enums.TransactionStatus.Confirmed;
        //        fromWithdraw.TransactionId = toWallet.MerchantAccountId.ToString("N");
        //        fromWithdraw.SelfPlatform = true;//平台内提币
        //        fromWithdraw.Id = new UserWithdrawalDAC().Create(fromWithdraw);
        //        fromWithdrawFee.WithdrawalId = fromWithdraw.Id;
        //        fromWithdrawFee.Fee = fromWithdraw.Amount * Convert.ToDecimal(mastSettings.First(e => e.Name == "UserWithdrawal_ToMerchant").Value);
        //        fromWithdrawFee.Fee = fromWithdrawFee.Fee.ToSpecificDecimal(cryptocurrency.DecimalPlace);
        //        new UserWithdrawalFeeDAC().Create(fromWithdrawFee);

        //        new UserWalletDAC().Decrease(fromWallet.Id, fromWithdraw.Amount);

        //        new UserWalletStatementDAC().Insert(new UserWalletStatement
        //        {
        //            WalletId = fromWallet.Id,
        //            Action = MerchantWalletStatementAction.Withdrawal,
        //            Amount = -fromWithdraw.Amount,
        //            Balance = fromWallet.Balance - fromWithdraw.Amount,
        //            Timestamp = DateTime.UtcNow
        //        });

        //        //充币
        //        var amount = fromWithdraw.Amount - fromWithdrawFee.Fee;
        //        if (amount <= 0)
        //        {
        //            throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.到账数量不能为零或者负数);
        //        }

        //        var deposit = new MerchantDepositDAC().Insert(new MerchantDeposit
        //        {
        //            MerchantAccountId = toWallet.MerchantAccountId,
        //            MerchantWalletId = toWallet.Id,
        //            FromAddress = fromWallet.Address,
        //            ToAddress = toWallet.Address,
        //            Amount = fromWithdraw.Amount - fromWithdrawFee.Fee,
        //            Status = Framework.Enums.TransactionStatus.Confirmed,
        //            Timestamp = DateTime.UtcNow,
        //            OrderNo = CreateOrderno(),
        //            TransactionId = fromWallet.UserAccountId.ToString("N"),
        //            SelfPlatform = true
        //        });
        //        new MerchantWalletDAC().Increase(toWallet.Id, amount);
        //        new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
        //        {
        //            WalletId = toWallet.Id,
        //            Action = MerchantWalletStatementAction.Deposit,
        //            Amount = amount,
        //            Balance = toWallet.Balance + amount,
        //            Timestamp = DateTime.UtcNow
        //        });

        //        new FiiiPayPushComponent().PushWithdrawCompleted(fromWithdraw.Id);
        //        scope.Complete();

        //        UserMSMQ.PubMerchantDeposit(deposit.Id, 0);
        //    }

        //    return new WithdrawOM
        //    {
        //        OrderId = fromWithdraw.Id,
        //        OrderNo = fromWithdraw.OrderNo,
        //        Timestamp = fromWithdraw.Timestamp.ToUnixTime().ToString()
        //    };
        //}

        private WithdrawOM CancelWithdrawal(UserWithdrawal fromWithdraw)
        {
            fromWithdraw.Status = TransactionStatus.Cancelled;
            fromWithdraw.TransactionId = Guid.NewGuid().ToString("N");
            fromWithdraw.SelfPlatform = true;//平台内提币

            using (var scope = new TransactionScope())
            {
                fromWithdraw.Id = new UserWithdrawalDAC().Create(fromWithdraw);

                new UserTransactionDAC().Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = fromWithdraw.UserAccountId,
                    CryptoId = fromWithdraw.CryptoId,
                    CryptoCode = fromWithdraw.CryptoCode,
                    Type = UserTransactionType.Withdrawal,
                    DetailId = fromWithdraw.Id.ToString(),
                    Status = (byte)fromWithdraw.Status,
                    Timestamp = fromWithdraw.Timestamp,
                    Amount = fromWithdraw.Amount,
                    OrderNo = fromWithdraw.OrderNo
                });

                var fromWithdrawFee = new UserWithdrawalFee
                {
                    Amount = fromWithdraw.Amount,
                    Fee = 0,
                    Timestamp = DateTime.UtcNow,
                    WithdrawalId = fromWithdraw.Id
                };

                new UserWithdrawalFeeDAC().Create(fromWithdrawFee);

                scope.Complete();
            }

            UserMSMQ.PubUserWithdrawReject(fromWithdraw.Id, 0);

            return new WithdrawOM
            {
                OrderId = fromWithdraw.Id,
                OrderNo = fromWithdraw.OrderNo,
                Timestamp = fromWithdraw.Timestamp.ToUnixTime().ToString()
            };
        }

        public PreWithdrawOM PreWithdraw(UserAccount user, int cryptoId)
        {
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);

            var exchangeRate = GetMarketPrice(user.CountryId, crypto.Code, "USD");
            var wallet = new UserWalletDAC().GetByAccountId(user.Id, cryptoId) ?? new UserWalletComponent().GenerateWallet(user.Id, cryptoId);
            //var profile = new UserProfileAgent().GetUserProfile(user.Id);

            var mastSettings = new MasterSettingDAC().SelectByGroup("UserWithdrawal");

            var Withdrawal_PerTx_Limit_User_NotVerified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerTx_Limit_User_NotVerified").Value) / exchangeRate;
            var Withdrawal_PerDay_Limit_User_NotVerified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerDay_Limit_User_NotVerified").Value) / exchangeRate;
            var Withdrawal_PerMonth_Limit_User_NotVerified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerMonth_Limit_User_NotVerified").Value) / exchangeRate;

            var Withdrawal_PerTx_Limit_User_Lv1Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerTx_Limit_User_Lv1Verified").Value) / exchangeRate;
            var Withdrawal_PerDay_Limit_User_Lv1Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerDay_Limit_User_Lv1Verified").Value) / exchangeRate;
            var Withdrawal_PerMonth_Limit_User_Lv1Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerMonth_Limit_User_Lv1Verified").Value) / exchangeRate;

            var Withdrawal_PerTx_Limit_User_Lv2Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerTx_Limit_User_Lv2Verified").Value) / exchangeRate;
            var Withdrawal_PerDay_Limit_User_Lv2Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerDay_Limit_User_Lv2Verified").Value) / exchangeRate;
            var Withdrawal_PerMonth_Limit_User_Lv2Verified = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_PerMonth_Limit_User_Lv2Verified").Value) / exchangeRate;

            var MinAmount = Convert.ToDecimal(mastSettings.First(e => e.Name == "Withdrawal_MinAmount").Value) / exchangeRate;

            var om = new PreWithdrawOM();

            om.Lv1Verified = user.L1VerifyStatus == VerifyStatus.Certified;
            om.Lv2Verified = user.L2VerifyStatus == VerifyStatus.Certified;

            om.Code = crypto.Code;
            om.NeedTag = crypto.NeedTag;

            om.DecimalPlace = crypto.DecimalPlace;

            //余额
            om.UseableBalance = wallet.Balance.ToString(crypto.DecimalPlace);

            //最小提币量
            om.MinAmount = MinAmount.ToString(crypto.DecimalPlace);

            // to fiiipay min withdraw amount
            var toFiiiPayMinWithdrawAmount = GetToFiiiPayMinAmount(Convert.ToDecimal(mastSettings.First(e => e.Name == "UserWithdrawal_ToUser_MinAmount").Value), exchangeRate, crypto.DecimalPlace);
            om.ToFiiiPayMinWithdrawalAmount = toFiiiPayMinWithdrawAmount.ToString(crypto.DecimalPlace);


            var dac = new UserWithdrawalDAC();
            var today = DateTime.UtcNow.Date;
            decimal dailyWithdrawal = dac.DailyWithdrawal(user.Id, cryptoId, today);
            decimal monthlyWithdrawal = dac.MonthlyWithdrawal(user.Id, cryptoId, new DateTime(today.Year, today.Month, 1));

            if (user.L1VerifyStatus == VerifyStatus.Certified && user.L2VerifyStatus == VerifyStatus.Certified)
            {
                om.PerTxLimit = Withdrawal_PerTx_Limit_User_Lv2Verified.ToString(crypto.DecimalPlace);
                om.PerDayLimit = Withdrawal_PerDay_Limit_User_Lv2Verified.ToString(crypto.DecimalPlace);
                om.PerMonthLimit = Withdrawal_PerMonth_Limit_User_Lv2Verified.ToString(crypto.DecimalPlace);
            }
            else if (user.L1VerifyStatus == VerifyStatus.Certified && user.L2VerifyStatus != VerifyStatus.Certified)
            {
                om.PerTxLimit = Withdrawal_PerTx_Limit_User_Lv1Verified.ToString(crypto.DecimalPlace);
                om.PerDayLimit = Withdrawal_PerDay_Limit_User_Lv1Verified.ToString(crypto.DecimalPlace);
                om.PerMonthLimit = Withdrawal_PerMonth_Limit_User_Lv1Verified.ToString(crypto.DecimalPlace);
            }
            else if (user.L1VerifyStatus != VerifyStatus.Certified && user.L2VerifyStatus != VerifyStatus.Certified)
            {
                om.PerTxLimit = Withdrawal_PerTx_Limit_User_NotVerified.ToString(crypto.DecimalPlace);
                om.PerDayLimit = Withdrawal_PerDay_Limit_User_NotVerified.ToString(crypto.DecimalPlace);
                om.PerMonthLimit = Withdrawal_PerMonth_Limit_User_NotVerified.ToString(crypto.DecimalPlace);
            }

            om.PerTxUsable = om.PerTxLimit;
            //当月可用限额小于当日可用限额时，当月可用限额更新为当日可用限额。
            decimal dayUsable = Convert.ToDecimal(om.PerDayLimit) - dailyWithdrawal;
            decimal monthUsable = Convert.ToDecimal(om.PerMonthLimit) - monthlyWithdrawal;
            dayUsable = Math.Min(dayUsable, monthUsable);

            om.PerDayUsable = Math.Max(0, dayUsable).ToString(crypto.DecimalPlace);
            om.PerMonthUsable = Math.Max(0, monthUsable).ToString(crypto.DecimalPlace);

            om.PerTxLimitIfLv1Veried = Withdrawal_PerTx_Limit_User_Lv1Verified.ToString(crypto.DecimalPlace);
            om.PerDayLimitIfLv1Veried = Withdrawal_PerDay_Limit_User_Lv1Verified.ToString(crypto.DecimalPlace);
            om.PerMonthLimitIfLv1Veried = Withdrawal_PerMonth_Limit_User_Lv1Verified.ToString(crypto.DecimalPlace);

            om.PerTxLimitIfLv2Veried = Withdrawal_PerTx_Limit_User_Lv2Verified.ToString(crypto.DecimalPlace);
            om.PerDayLimitIfLv2Veried = Withdrawal_PerDay_Limit_User_Lv2Verified.ToString(crypto.DecimalPlace);
            om.PerMonthLimitIfLv2Veried = Withdrawal_PerMonth_Limit_User_Lv2Verified.ToString(crypto.DecimalPlace);

            return om;
        }

        private decimal GetToFiiiPayMinAmount(decimal settingToUserMinAmount, decimal exchangeRate, byte cryptoDecimalPlace)
        {
            if (settingToUserMinAmount > 0)
                settingToUserMinAmount /= exchangeRate;
            else
                settingToUserMinAmount = 1 / ((decimal)Math.Pow(10, cryptoDecimalPlace));

            return settingToUserMinAmount;
        }

        public decimal GetMarketPrice(int countryId, string code, string FiatCurrency)
        {
            return new MarketPriceComponent().GetMarketPrice(FiatCurrency, code)?.Price ?? 0;
        }

        public AddressListOM AddressList(UserAccount user, int cryptoId)
        {
            return new AddressListOM
            {
                List = new CryptoAddressDAC().GetByCryptoId(user.Id, cryptoId).Select(a => new AddressListOMItem
                {
                    Address = a.Address,
                    Tag = a.Tag,
                    Alias = a.Alias,
                    Id = a.Id
                }).ToList()
            };
        }

        public List<ListForManageWithdrawalAddressOMItem> ListForManageWithdrawalAddress(UserAccount user)
        {
            var dac = new CryptoAddressDAC();

            var list = dac.GetByAccountId(user.Id);

            var cryptoList = new CryptocurrencyDAC().GetAllActived();

            return cryptoList.Select(e =>
            {
                return new ListForManageWithdrawalAddressOMItem
                {
                    Id = e.Id,
                    Code = e.Code,
                    AddressCount = list.Count(c => c.CryptoId == e.Id),
                    NeedTag = e.NeedTag
                };
            }).ToList();

        }

        public AddAddressOM AddAddress(UserAccount user, string address, string tag, string alias, int coinId)
        {
            var crypto = new CryptocurrencyDAC().GetById(coinId);
            CryptoAddressValidation.ValidateAddress(crypto.Code, address);
            if (!string.IsNullOrEmpty(tag))
            {
                CryptoAddressValidation.ValidateTag(crypto.Code, tag);
            }

            var agent = new FiiiFinanceAgent();

            try
            {
                if (!agent.ValidateAddress(crypto.Code, address))
                    throw new CommonException(ReasonCode.GENERAL_ERROR, GeneralResources.EMInvalidAddress);
            }
            catch (FiiiFinanceException ex)
            {
                if (ex.ReasonCode == 20002)
                    throw new CommonException(ReasonCode.GENERAL_ERROR, GeneralResources.EMInvalidAddress);
            }
            var id = new CryptoAddressDAC().Insert(new CryptoAddress
            {
                AccountId = user.Id,
                AccountType = AccountType.User,
                Address = address,
                Tag = tag,
                Alias = alias,
                CryptoId = coinId,
            });

            return new AddAddressOM
            {
                Id = id
            };
        }

        public void DeleteAddress(UserAccount user, long id)
        {
            var address = new CryptoAddressDAC().GetById(id);
            if (address == null || address.AccountId != user.Id)
            {
                throw new ApplicationException(MessageResources.AddressNotExist);
            }
            new CryptoAddressDAC().Delete(id);
        }

        public TransactionFeeRateOM TransactionFeeRate(int coinId, string address, string tag)
        {
            var userWalletDAC = new UserWalletDAC();
            var merchantWalletDAC = new MerchantWalletDAC();
            var coin = new CryptocurrencyDAC().GetById(coinId);
            var mastSettings = new MasterSettingDAC().SelectByGroup("UserWithdrawal");
            var isUserWallet = userWalletDAC.IsUserWalletAddress(address);
            var isMerchantWallet = merchantWalletDAC.IsMerchantWalletAddress(address);
            bool isFiiiAccount = isUserWallet || isMerchantWallet;

            if (isFiiiAccount)
            {
                var userWallet = userWalletDAC.GetByAddressAndCrypto(coinId, address, tag);
                var result = new TransactionFeeRateOM
                {
                    CryptoAddressType = CryptoAddressType.FiiiPay,
                    TransactionFee = "0",
                    TransactionFeeRate = mastSettings.First(e => e.Name == "UserWithdrawal_ToUser").Value
                };
                if (userWallet != null)
                    return result;

                var merchantWallet = merchantWalletDAC.GetByAddressAndCrypto(coinId, address, tag);
                if (merchantWallet != null)
                {
                    result.CryptoAddressType = CryptoAddressType.FiiiPOS;
                    return result;
                }

                result.CryptoAddressType = CryptoAddressType.InsideWithError;
                return result;
            }
            else
            {
                return new TransactionFeeRateOM
                {
                    CryptoAddressType = CryptoAddressType.Outside,
                    TransactionFee = (coin.Withdrawal_Fee ?? 0).ToString(CultureInfo.InvariantCulture),
                    TransactionFeeRate = (coin.Withdrawal_Tier ?? 0).ToString(CultureInfo.InvariantCulture)
                };
            }
        }
    }
}
