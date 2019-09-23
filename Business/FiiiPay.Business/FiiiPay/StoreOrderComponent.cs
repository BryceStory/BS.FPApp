using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.DTO;
using FiiiPay.DTO.Order;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Queue;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FiiiPay.Business
{
    public class StoreOrderComponent
    {
        public PrePayOM PrePay(Guid userAccountId, Guid merchantInfoId)
        {
            var merchantInfo = new MerchantInformationDAC().GetById(merchantInfoId);
            if (merchantInfo.AccountType == AccountType.Merchant)
                return GetMerchantPrePayOM(userAccountId,merchantInfo);
            if(merchantInfo.AccountType == AccountType.User)
                return GetUserPrePayOM(userAccountId,merchantInfo);
            return null;
        }

        private PrePayOM GetMerchantPrePayOM(Guid userAccountId, MerchantInformation merchantInfo)
        {
            var merchantAccount = new MerchantAccountDAC().GetById(merchantInfo.MerchantAccountId);
            var userWallets = new UserWalletDAC().GetUserWallets(userAccountId);
            var merchantWallets = new MerchantWalletDAC().GetByAccountId(merchantAccount.Id);
            var coins = new CryptocurrencyDAC().GetAllActived();
            var priceList = new PriceInfoDAC().GetPrice(merchantAccount.FiatCurrency);

            bool showWhiteLable = false;
            if (merchantAccount.POSId.HasValue)
            {
                var pos = new POSDAC().GetById(merchantAccount.POSId.Value);
                if (pos.IsWhiteLabel)
                    showWhiteLable = true;
            }

            if (!showWhiteLable)
            {
                var whilteLabelCryptoCode = new POSDAC().GetWhiteLabelCryptoCode();
                coins.RemoveAll(t => t.Code == whilteLabelCryptoCode);
            }

            return new PrePayOM
            {
                FiatCurrency = merchantAccount.FiatCurrency,
                MarkupRate = merchantAccount.Markup.ToString(CultureInfo.InvariantCulture),
                WaletList = coins.Select(a =>
                {
                    var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);
                    decimal rate = 0;
                    rate = priceList.Where(t => t.CryptoID == a.Id).Select(t => t.Price).FirstOrDefault();
                    return GetItem(userWallet, a, merchantWallets, rate);
                }).OrderByDescending(a => a.MerchantSupported).ThenBy(a => a.PayRank).Select(a => new WalletItem
                {
                    Code = a.Code,
                    NewStatus = a.NewStatus,
                    ExchangeRate = a.ExchangeRate,
                    FrozenBalance = a.FrozenBalance,
                    IconUrl = a.IconUrl,
                    Id = a.Id,
                    MerchantSupported = a.MerchantSupported,
                    Name = a.Name,
                    UseableBalance = a.UseableBalance,
                    FiatBalance = a.FiatBalance,
                    DecimalPlace = a.DecimalPlace,
                    CryptoEnable = a.CryptoEnable
                }).ToList()
            };
        }

        private WalletItemTemp GetItem(UserWallet model, Cryptocurrency coin, List<MerchantWallet> merchantWallets, decimal exchantRate)
        {
            model = model ?? new UserWallet { FrozenBalance = 0, Balance = 0, PayRank = int.MaxValue };

            return new WalletItemTemp
            {
                Id = coin.Id,
                FrozenBalance = model.FrozenBalance.ToString(coin.DecimalPlace),
                IconUrl = coin.IconURL,
                Code = coin.Code,
                NewStatus = coin.Status,
                UseableBalance = model.Balance.ToString(coin.DecimalPlace),
                PayRank = model.PayRank,
                Name = coin.Name,
                ExchangeRate = exchantRate.ToString(4),
                FiatBalance = (model.Balance * exchantRate).ToString(2),
                MerchantSupported = merchantWallets.Exists(a => a.CryptoId == coin.Id && a.SupportReceipt),
                DecimalPlace = coin.DecimalPlace,
                CryptoEnable = coin.Enable
            };
        }

        class WalletItemTemp : WalletItem
        {
            public int PayRank { get; set; }

        }

        private PrePayOM GetUserPrePayOM(Guid userAccountId, MerchantInformation merchantInfo)
        {
            var userDAC = new UserAccountDAC();
            var merchantDAC = new MerchantInformationDAC();
            var merchantUserAccount = userDAC.GetById(merchantInfo.MerchantAccountId);
            var supportList = new MerchantSupportCryptoDAC().GetList(merchantInfo.Id).ToList();
            var userWallets = new UserWalletDAC().GetUserWallets(userAccountId);
            var coins = new CryptocurrencyDAC().GetAllActived();
            var priceList = new PriceInfoDAC().GetPrice(merchantUserAccount.FiatCurrency);

            var whilteLabelCryptoCode = new POSDAC().GetWhiteLabelCryptoCode();
            coins.RemoveAll(t => t.Code == whilteLabelCryptoCode);
            
            return new PrePayOM
            {
                FiatCurrency = merchantUserAccount.FiatCurrency,
                MarkupRate = merchantInfo.Markup.ToString(CultureInfo.InvariantCulture),
                WaletList = coins.Select(a =>
                {
                    var userWallet = userWallets.FirstOrDefault(b => b.CryptoId == a.Id);
                    decimal rate = 0;
                    rate = priceList.Where(t => t.CryptoID == a.Id).Select(t => t.Price).FirstOrDefault();
                    return GetUserSupportItem(userWallet, a, supportList, rate);
                }).OrderByDescending(a => a.MerchantSupported).ThenBy(a => a.PayRank).Select(a => new WalletItem
                {
                    Code = a.Code,
                    NewStatus = a.NewStatus,
                    ExchangeRate = a.ExchangeRate,
                    FrozenBalance = a.FrozenBalance,
                    IconUrl = a.IconUrl,
                    Id = a.Id,
                    MerchantSupported = a.MerchantSupported,
                    Name = a.Name,
                    UseableBalance = a.UseableBalance,
                    FiatBalance = a.FiatBalance,
                    DecimalPlace = a.DecimalPlace,
                    CryptoEnable = a.CryptoEnable
                }).ToList()
            };
        }

        private WalletItemTemp GetUserSupportItem(UserWallet model, Cryptocurrency coin, List<MerchantSupportCrypto> supportList, decimal exchantRate)
        {
            model = model ?? new UserWallet { FrozenBalance = 0, Balance = 0, PayRank = int.MaxValue };

            return new WalletItemTemp
            {
                Id = coin.Id,
                FrozenBalance = model.FrozenBalance.ToString(coin.DecimalPlace),
                IconUrl = coin.IconURL,
                Code = coin.Code,
                NewStatus = coin.Status,
                UseableBalance = model.Balance.ToString(coin.DecimalPlace),
                PayRank = model.PayRank,
                Name = coin.Name,
                ExchangeRate = exchantRate.ToString(4),
                FiatBalance = (model.Balance * exchantRate).ToString(2),
                MerchantSupported = supportList.Exists(a => a.CryptoId == coin.Id),
                DecimalPlace = coin.DecimalPlace,
                CryptoEnable = coin.Enable
            };
        }

        public async Task<PayOrderOM> PayAsync(UserAccount user, StoreOrderPayIM im)
        {
            #region 验证
            if (im.FiatAmount <= 0)
                throw new ApplicationException();
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, im.Pin);
            if (!user.IsAllowExpense.HasValue || !user.IsAllowExpense.Value)
                throw new CommonException(ReasonCode.Not_Allow_Expense, MessageResources.PaymentForbidden);

            var coin = new CryptocurrencyDAC().GetById(im.CoinId);
            if (!coin.Status.HasFlag(CryptoStatus.Pay) || coin.Enable == 0)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, MessageResources.CurrencyForbidden);

            var merchantInfo = new MerchantInformationDAC().GetById(im.MerchantInfoId);
            if (merchantInfo.Status != Status.Enabled || merchantInfo.VerifyStatus != VerifyStatus.Certified || merchantInfo.IsPublic != Status.Enabled)
                throw new CommonException(ReasonCode.Not_Allow_AcceptPayment, MessageResources.MerchantExceptionTransClose);
            if (!merchantInfo.IsAllowExpense)
                throw new CommonException(ReasonCode.Not_Allow_AcceptPayment, MessageResources.MerchantReceiveNotAllowed);
            if (merchantInfo.AccountType == AccountType.Merchant)
                throw new ApplicationException();

            var storeAccount = new UserAccountDAC().GetById(merchantInfo.MerchantAccountId);
            if (storeAccount.Id == user.Id)
                throw new CommonException(ReasonCode.Not_Allow_AcceptPayment, MessageResources.PaytoSelf);
            if (storeAccount == null || storeAccount.Status.Value != (byte)AccountStatus.Active)
                throw new CommonException(ReasonCode.Not_Allow_AcceptPayment, MessageResources.MerchantFiiipayAbnormal);

            var paySetting = await new StorePaySettingDAC().GetByCountryIdAsync(merchantInfo.CountryId);
            if (paySetting != null)
            {
                if (im.FiatAmount > paySetting.LimitAmount)
                    throw new CommonException(ReasonCode.TRANSFER_AMOUNT_OVERFLOW, string.Format(MessageResources.TransferAmountOverflow, paySetting.LimitAmount, paySetting.FiatCurrency));
            }
            #endregion

            var walletDAC = new UserWalletDAC();
            var statementDAC = new UserWalletStatementDAC();
            var storeOrderDAC = new StoreOrderDAC();
            var utDAC = new UserTransactionDAC();

            #region 计算
            decimal markup = merchantInfo.Markup;
            decimal feeRate = merchantInfo.FeeRate;
            var exchangeRate = new PriceInfoDAC().GetPriceByName(storeAccount.FiatCurrency, coin.Code);

            decimal failTotalAmount = im.FiatAmount + (im.FiatAmount * markup).ToSpecificDecimal(4);
            decimal transactionFiatFee = (im.FiatAmount * feeRate).ToSpecificDecimal(4);
            decimal transactionFee = (transactionFiatFee / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);
            decimal cryptoAmount = (failTotalAmount / exchangeRate).ToSpecificDecimal(coin.DecimalPlace);

            var fromWallet = walletDAC.GetUserWallet(user.Id, im.CoinId);
            if (fromWallet == null || fromWallet.Balance < cryptoAmount)
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);

            var toWallet = walletDAC.GetUserWallet(storeAccount.Id, im.CoinId);
            if (toWallet == null)
                toWallet = new UserWalletComponent().GenerateWallet(storeAccount.Id, im.CoinId);
            #endregion

            #region entity
            DateTime dtNow = DateTime.UtcNow;
            StoreOrder order = new StoreOrder
            {
                Id = Guid.NewGuid(),
                OrderNo = IdentityHelper.OrderNo(),
                Timestamp = dtNow,
                Status = OrderStatus.Completed,
                MerchantInfoId = merchantInfo.Id,
                MerchantInfoName = merchantInfo.MerchantName,
                UserAccountId = user.Id,
                CryptoId = im.CoinId,
                CryptoCode = coin.Code,
                CryptoAmount = cryptoAmount,
                CryptoActualAmount = cryptoAmount - transactionFee,
                ExchangeRate = exchangeRate,
                Markup = markup,
                FiatCurrency = storeAccount.FiatCurrency,
                FiatAmount = im.FiatAmount,
                FiatActualAmount = failTotalAmount,
                FeeRate = feeRate,
                TransactionFee = transactionFee,
                PaymentTime = dtNow
            };
            UserWalletStatement fromStatement = new UserWalletStatement
            {
                WalletId = fromWallet.Id,
                Action = UserWalletStatementAction.StoreOrderOut,
                Amount = -order.CryptoAmount,
                Balance = fromWallet.Balance - order.CryptoAmount,
                FrozenAmount = 0,
                FrozenBalance = fromWallet.FrozenBalance,
                Timestamp = dtNow
            };
            UserWalletStatement toStatement = new UserWalletStatement
            {
                WalletId = toWallet.Id,
                Action = UserWalletStatementAction.StoreOrderIn,
                Amount = order.CryptoActualAmount,
                Balance = toWallet.Balance + order.CryptoActualAmount,
                FrozenAmount = 0,
                FrozenBalance = toWallet.FrozenBalance,
                Timestamp = dtNow
            };
            #endregion

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await storeOrderDAC.CreateAsync(order);
                await utDAC.InsertAsync(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = fromWallet.UserAccountId,
                    CryptoId = order.CryptoId,
                    CryptoCode = order.CryptoCode,
                    Type = UserTransactionType.StoreOrderConsume,
                    DetailId = order.Id.ToString(),
                    Status = (byte)order.Status,
                    Timestamp = order.PaymentTime.Value,
                    Amount = order.CryptoAmount,
                    OrderNo = order.OrderNo,
                    MerchantName = order.MerchantInfoName
                });
                await utDAC.InsertAsync(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = toWallet.UserAccountId,
                    CryptoId = order.CryptoId,
                    CryptoCode = order.CryptoCode,
                    Type = UserTransactionType.StoreOrderIncome,
                    DetailId = order.Id.ToString(),
                    Status = (byte)order.Status,
                    Timestamp = order.Timestamp,
                    Amount = order.CryptoActualAmount,
                    OrderNo = order.OrderNo,
                    MerchantName = order.MerchantInfoName
                });
                walletDAC.Decrease(fromWallet.Id, order.CryptoAmount);
                walletDAC.Increase(toWallet.Id, order.CryptoActualAmount);
                statementDAC.Insert(fromStatement);
                statementDAC.Insert(toStatement);
                scope.Complete();
            }

            var pushObj = new { order.Id, order.UserAccountId, order.CryptoCode };

            RabbitMQSender.SendMessage("StoreOrderPayed", new { order.Id, MerchantInfoId = merchantInfo.MerchantAccountId, order.UserAccountId, order.CryptoCode });

            return new PayOrderOM
            {
                Amount = order.CryptoAmount.ToString(coin.DecimalPlace),
                Currency = coin.Code,
                OrderId = order.Id.ToString(),
                OrderNo = order.OrderNo,
                Timestamp = dtNow.ToUnixTime().ToString()
            };
        }

        public async Task<StoreIncomeDetail> GetIncomeDetailAsync(Guid accountId, Guid orderId, bool isZH)
        {
            var storeOrder = await new StoreOrderDAC().GetByIdAsync(orderId);
            if (storeOrder == null)
                throw new ApplicationException();
            var merchantInfo = new MerchantInformationDAC().GetById(storeOrder.MerchantInfoId);
            var storeUserAccount = new UserAccountDAC().GetById(merchantInfo.MerchantAccountId);
            if (storeUserAccount.Id != accountId)
                throw new ApplicationException();
            var customerAccount = new UserAccountDAC().GetById(storeOrder.UserAccountId);
            var coin = new CryptocurrencyDAC().GetById(storeOrder.CryptoId);
            var priceInfo = new PriceInfoDAC().GetPriceByName(storeOrder.FiatCurrency, storeOrder.CryptoCode);
            var iRate = ((priceInfo - storeOrder.ExchangeRate) / storeOrder.ExchangeRate) * 100;
            return new StoreIncomeDetail
            {
                Id = storeOrder.Id,
                Type = Resources.StoreOrderIncome,
                CryptoActualAmount = storeOrder.CryptoActualAmount.ToString(coin.DecimalPlace),
                CryptoCode = storeOrder.CryptoCode,
                Status = new UserStatementComponent().GetStatusStr(2, (int)storeOrder.Status, isZH),
                MerchantName = storeOrder.MerchantInfoName,
                OrderNo = storeOrder.OrderNo,
                UserAccountName = GetMaskedCellphone(customerAccount.PhoneCode, customerAccount.Cellphone),
                Timestamp = storeOrder.Timestamp.ToUnixTime().ToString(),
                FiatCurrency = storeOrder.FiatCurrency,
                FiatAmount = storeOrder.FiatAmount.ToString(2),
                MarkUp = $"{(storeOrder.Markup * 100).ToString(2)}%",
                TotalFiatAmount = storeOrder.FiatActualAmount.ToString(2),
                ExchangeRate = $"1 {coin.Code} = {storeOrder.ExchangeRate.ToString(4)} {storeOrder.FiatCurrency}",
                CurrentExchangeRate = coin.Enable == 1 ? $"1 {coin.Code} = {priceInfo.ToString(4)} {storeOrder.FiatCurrency}" : "--",
                IncreaseRate = coin.Enable == 1 ? (iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2)) : "--",
                CryptoAmount = storeOrder.CryptoAmount.ToString(coin.DecimalPlace),
                TransactionFee = storeOrder.TransactionFee.ToString(coin.DecimalPlace),
                FeeCryptoCode = storeOrder.CryptoCode
            };
        }
        public async Task<StoreConsumeDetail> GetConsumeDetailAsync(Guid accountId, Guid orderId, bool isZH)
        {
            var storeOrder = await new StoreOrderDAC().GetByIdAsync(orderId);
            var storeUserAccount = new UserAccountDAC().GetById(storeOrder.UserAccountId);
            if (storeUserAccount.Id != accountId)
                throw new ApplicationException();
            var coin = new CryptocurrencyDAC().GetById(storeOrder.CryptoId);
            var priceInfo = new PriceInfoDAC().GetPriceByName(storeOrder.FiatCurrency, storeOrder.CryptoCode);
            var iRate = ((priceInfo - storeOrder.ExchangeRate) / storeOrder.ExchangeRate) * 100;
            return new StoreConsumeDetail
            {
                Id = storeOrder.Id,
                Type = Resources.Payment,
                CryptoActualAmount= storeOrder.CryptoAmount.ToString(coin.DecimalPlace),
                CryptoCode = storeOrder.CryptoCode,
                Status = new UserStatementComponent().GetStatusStr(2, (int)storeOrder.Status, isZH),
                MerchantName = storeOrder.MerchantInfoName,
                FiatCurrency = storeOrder.FiatCurrency,
                FiatAmount = storeOrder.FiatAmount.ToString(2),
                MarkUp = $"{(storeOrder.Markup * 100).ToString(2)}%",
                ExchangeRate = $"1 {coin.Code} = {storeOrder.ExchangeRate.ToString(4)} {storeOrder.FiatCurrency}",
                CurrentExchangeRate = coin.Enable == 1 ? $"1 {coin.Code} = {priceInfo.ToString(4)} {storeOrder.FiatCurrency}" : "--",
                IncreaseRate = coin.Enable == 1 ? (iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2)) : "--",
                Timestamp = storeOrder.Timestamp.ToUnixTime().ToString(),
                OrderNo = storeOrder.OrderNo
            };
        }

        private string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }
    }
}
