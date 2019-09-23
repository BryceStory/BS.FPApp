using FiiiPay.Data;
using FiiiPay.DTO.Transfer;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using System;
using FiiiPay.Business.Properties;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Queue;
using FiiiPay.Framework.Component.Verification;

namespace FiiiPay.Business
{
    public class TransferComponent : BaseComponent
    {
        public TransferDetailOM Detail(Guid accountId, long transferId)
        {
            var transfer = new UserTransferDAC().GetTransfer(transferId);
            Guid showAccountId = Guid.Empty;
            string transferType = "";
            if (accountId == transfer.FromUserAccountId)
            {
                transferType = Resources.TransferOut;
                showAccountId = transfer.ToUserAccountId;
            }
            else if (accountId == transfer.ToUserAccountId)
            {
                transferType = Resources.TransferInto;
                showAccountId = transfer.FromUserAccountId;
            }

            UserProfile profile = new UserProfileAgent().GetUserProfile(showAccountId);
            var account = new UserAccountDAC().GetById(showAccountId);
            var country = new CountryComponent().GetById(profile.Country.Value);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(transfer.CoinId);

            return new TransferDetailOM
            {
                Status = (TransactionStatus)transfer.Status,
                StatusStr = Resources.OrderCompleted,
                TradeType = Resources.Transfer,
                TransferType = transferType,
                CoinCode = transfer.CoinCode,
                Amount = transfer.Amount.ToString(cryptoCurrency.DecimalPlace),
                AccountName = country.PhoneCode + " *******" + account.Cellphone.Substring(Math.Max(0, account.Cellphone.Length - 4)),
                Fullname = profile == null ? "" : ((string.IsNullOrEmpty(profile.FirstName) ? "" : "* ") + profile.LastName),
                Timestamp = transfer.Timestamp.ToUnixTime().ToString(),
                OrderNo = transfer.OrderNo
            };
        }
        public PreTransferOM PreTransfer(UserAccount account, PreTransferIM im)
        {
            var toAccount = new UserAccountDAC().GetByCountryIdAndCellphone(im.ToCountryId, im.ToCellphone);
            if (toAccount == null)
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, MessageResources.AccountNotExist);
            if (account.Id == toAccount.Id)
                throw new CommonException(ReasonCode.TRANSFER_TO_SELF, MessageResources.TransferToSelf);
            var country = new CountryComponent().GetById(im.ToCountryId);
            var profile = new UserProfileAgent().GetUserProfile(toAccount.Id);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(im.CoinId);
            var fromWallet = new UserWalletComponent().GetUserWallet(account.Id, im.CoinId);

            return new PreTransferOM
            {
                ToAvatar = toAccount.Photo ?? Guid.Empty,
                ToAccountName = country.PhoneCode + " " + toAccount.Cellphone,
                ToFullname = profile == null ? "" : ("* " + profile.LastName),
                IsTransferAbled = !toAccount.IsAllowTransfer.HasValue || toAccount.IsAllowTransfer.Value,
                IsProfileVerified = toAccount.L1VerifyStatus == VerifyStatus.Certified,
                CoinId = cryptoCurrency.Id,
                CoinCode = cryptoCurrency.Code,
                MinCount = ((decimal)Math.Pow(10, -cryptoCurrency.DecimalPlace)).ToString("G"),
                CoinDecimalPlace = cryptoCurrency.DecimalPlace.ToString(),
                CoinBalance = (fromWallet == null ? "0" : fromWallet.Balance.ToString(cryptoCurrency.DecimalPlace)),
                FiatCurrency = account.FiatCurrency,
                Price = (GetExchangeRate(account.CountryId, account.FiatCurrency, cryptoCurrency.Code)).ToString(),
                ChargeFee = "0"
            };
        }

        public TransferOM Transfer(UserAccount account, TransferIM im)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, account.Id.ToString(), account.Pin, im.Pin);
            if (account.L1VerifyStatus != VerifyStatus.Certified)
                throw new ApplicationException();
            if (account.IsAllowTransfer.HasValue && !account.IsAllowTransfer.Value)
                throw new CommonException(ReasonCode.TRANSFER_FORBIDDEN, MessageResources.TransferForbidden);
            var toAccount = new UserAccountDAC().GetByCountryIdAndCellphone(im.ToCountryId, im.ToCellphone);
            if (toAccount == null)
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, MessageResources.AccountNotExist);
            if (toAccount.IsAllowTransfer.HasValue && !toAccount.IsAllowTransfer.Value)
                throw new CommonException(ReasonCode.TRANSFER_FORBIDDEN, MessageResources.ToAccountTransferForbidden);
            if (im.Amount >= Convert.ToDecimal(Math.Pow(10, 11)))
                throw new CommonException(ReasonCode.TRANSFER_AMOUNT_OVERFLOW, MessageResources.TransferAmountOverflow);
            var currency = new CryptocurrencyDAC().GetById(im.CoinId);
            if (!currency.Status.HasFlag(CryptoStatus.Transfer) || currency.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);
            if (im.Amount < (decimal)Math.Pow(10, -currency.DecimalPlace))
                throw new CommonException(ReasonCode.TRANSFER_AMOUNT_OVERFLOW, MessageResources.TransferAmountTooSmall);
            var decimalDigits = im.Amount.ToString().Length - im.Amount.ToString().IndexOf('.') - 1;
            if (decimalDigits > currency.DecimalPlace)
                throw new CommonException(ReasonCode.TRANSFER_AMOUNT_OVERFLOW, MessageResources.TransferAmountOverflow);

            if (account.Id == toAccount.Id)
                throw new CommonException(ReasonCode.TRANSFER_TO_SELF, MessageResources.TransferToSelf);

            var uwComponent = new UserWalletComponent();

            var toWallet = uwComponent.GetUserWallet(toAccount.Id, im.CoinId);
            if (toWallet == null)
                toWallet = uwComponent.GenerateWallet(toAccount.Id, currency.Id);

            var country = new CountryComponent().GetById(im.ToCountryId);
            DateTime dtCreateTime = DateTime.UtcNow;

            var fromWallet = uwComponent.GetUserWallet(account.Id, im.CoinId);
            if (fromWallet.Balance < im.Amount)
                throw new CommonException(ReasonCode.TRANSFER_BALANCE_LOW, MessageResources.TransferBalanceLow);

            UserTransfer transfer = new UserTransfer
            {
                Timestamp = dtCreateTime,
                OrderNo = CreateOrderno(),
                FromUserAccountId = account.Id,
                FromUserWalletId = fromWallet.Id,
                CoinId = currency.Id,
                CoinCode = currency.Code,
                ToUserAccountId = toAccount.Id,
                ToUserWalletId = toWallet.Id,
                Amount = im.Amount,
                Status = (byte)TransactionStatus.Confirmed
            };

            var uwDAC = new UserWalletDAC();
            var uwsDAC = new UserWalletStatementDAC();
            var utDAC = new UserTransactionDAC();
            //var pushComponent = new FiiiPayPushComponent();
            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                transfer.Id = new UserTransferDAC().Insert(transfer);

                utDAC.Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = transfer.FromUserAccountId,
                    CryptoId = transfer.CoinId,
                    CryptoCode = transfer.CoinCode,
                    Type = UserTransactionType.TransferOut,
                    DetailId = transfer.Id.ToString(),
                    Status = transfer.Status,
                    Timestamp = dtCreateTime,
                    Amount = transfer.Amount,
                    OrderNo = transfer.OrderNo
                });

                utDAC.Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = transfer.ToUserAccountId,
                    CryptoId = transfer.CoinId,
                    CryptoCode = transfer.CoinCode,
                    Type = UserTransactionType.TransferIn,
                    DetailId = transfer.Id.ToString(),
                    Status = transfer.Status,
                    Timestamp = dtCreateTime,
                    Amount = transfer.Amount,
                    OrderNo = transfer.OrderNo
                });

                uwDAC.Decrease(fromWallet.Id, transfer.Amount);
                uwDAC.Increase(toWallet.Id, transfer.Amount);

                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = fromWallet.Id,
                    Action = UserWalletStatementAction.TansferOut,
                    Amount = -transfer.Amount,
                    Balance = fromWallet.Balance - transfer.Amount,
                    FrozenAmount = 0,
                    FrozenBalance = fromWallet.FrozenBalance,
                    Timestamp = dtCreateTime
                });

                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = toWallet.Id,
                    Action = UserWalletStatementAction.TansferIn,
                    Amount = transfer.Amount,
                    Balance = toWallet.Balance + transfer.Amount,
                    FrozenAmount = 0,
                    FrozenBalance = toWallet.FrozenBalance,
                    Timestamp = dtCreateTime
                });

                scope.Complete();
            }

            RabbitMQSender.SendMessage("UserTransferOutFiiiPay", transfer.Id);
            RabbitMQSender.SendMessage("UserTransferIntoFiiiPay", transfer.Id);
            //pushComponent.PushTransferOut(transfer.Id);
            //pushComponent.PushTransferInto(transfer.Id);

            return new TransferOM
            {
                Timestamp = dtCreateTime.ToUnixTime().ToString(),
                TracingId = transfer.Id,
                TracingNo = transfer.OrderNo,
                AccountName = country.PhoneCode + " " + toAccount.Cellphone
            };
        }

        private string CreateOrderno()
        {
            return DateTime.Now.ToUnixTime() + new Random().Next(0, 100).ToString("00");
        }

        private decimal GetExchangeRate(int countryId, string fiatCurrency, string coinCode)
        {
            var agent = new MarketPriceComponent();
            var price = agent.GetMarketPrice(fiatCurrency, coinCode);
            return price?.Price ?? 0;
        }
    }
}
