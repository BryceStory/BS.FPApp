using System;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.DTO.FiiiExTransfer;
using FiiiPay.DTO.Investor;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;

namespace FiiiPay.Business
{
    public partial class FiiiEXTransferComponent
    {
        public string FiiiPayTransferInto(UserAccount account, Cryptocurrency coin, decimal amount)
        {
            var walletDac = new UserWalletDAC();
            var wallet = walletDac.GetByAccountId(account.Id, coin.Id);
            UserExTransferOrder order;
            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = new UserWalletComponent().GenerateWallet(account.Id, coin.Id);

                walletDac.Increase(wallet.Id, amount);

                order = new UserExTransferOrder
                {
                    Timestamp = DateTime.UtcNow,
                    OrderNo = CreateOrderNo(),
                    OrderType = ExTransferType.FromEx,
                    AccountId = account.Id,
                    WalletId = wallet.Id,
                    CryptoId = coin.Id,
                    CryptoCode = coin.Code,
                    Amount = amount,
                    Status = 1,
                    Remark = null,
                    ExId = null
                };

                new UserExTransferOrderDAC().Create(order);

                scope.Complete();
            }

            try
            {
                FiiiEXTransferMSMQ.PubUserTransferFromEx(order.Id, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Info("PubUserTransferFromEx - error", ex);
            }
            return order.OrderNo;
        }

        public TransferResult FiiiPayTransferToEx(UserAccount account, int cryptoId, decimal amount, string pin)
        {
            new SecurityComponent().VerifyPin(account, pin);

            var openAccountDac = new OpenAccountDAC();
            var openAccount = openAccountDac.GetOpenAccount(FiiiType.FiiiPay, account.Id);
            if (openAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.FiiiExAccountNotExist);

            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            if (crypto == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.CurrencyForbidden);

            var walletDac = new UserWalletDAC();
            var wallet = walletDac.GetByAccountId(account.Id, crypto.Id);

            if (wallet.Balance < amount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.余额不足);
            //10091=参数不符合要求 10013=用户信息不存在 10020=币不存在 0=成功
            int result = FiiiExCoinIn(openAccount.OpenId, crypto.Code, amount, out string recordId);
            if (result != 0)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.划转到FiiiEx失败);
            UserExTransferOrder order;
            using (var scope = new TransactionScope())
            {
                walletDac.Decrease(wallet.Id, amount);
                order = new UserExTransferOrder
                {
                    Timestamp = DateTime.UtcNow,
                    OrderNo = CreateOrderNo(),
                    OrderType = ExTransferType.ToEx,
                    AccountId = account.Id,
                    WalletId = wallet.Id,
                    CryptoId = crypto.Id,
                    CryptoCode = crypto.Code,
                    Amount = amount,
                    Status = 1,
                    Remark = null,
                    ExId = recordId
                };

                order = new UserExTransferOrderDAC().Create(order);
                scope.Complete();
            }

            try
            {
                FiiiEXTransferMSMQ.PubUserTransferToEx(order.Id, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Info("PubUserTransferToEx - error", ex);
            }

            return new TransferResult
            {
                Id = order.Id,
                Amount = order.Amount.ToString(crypto.DecimalPlace),
                OrderNo = order.OrderNo,
                Timestamp = order.Timestamp.ToUnixTime(),
                CryptoCode = crypto.Code
            };
        }
        public TransferResult FiiiPayTransferFromEx(UserAccount account, int cryptoId, decimal amount, string pin)
        {
            new SecurityComponent().VerifyPin(account, pin);

            var openAccountDac = new OpenAccountDAC();
            var openAccount = openAccountDac.GetOpenAccount(FiiiType.FiiiPay, account.Id);
            if (openAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.FiiiExAccountNotExist);

            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            if (crypto == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.CurrencyForbidden);

            var balance = this.FiiiExBalance(FiiiType.FiiiPay, account.Id, crypto);
            if (balance < amount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.余额不足);

            //10091=参数不符合要求 10013=用户信息不存在 10024=用户币不存在 10025=用户币种余额不足 0=成功
            int result = FiiiExCoinOut(openAccount.OpenId, crypto.Code, amount, out string recordId);
            if (result == 10025)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.余额不足);
            if (result != 0)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.从FiiiEx划转失败);

            var walletDac = new UserWalletDAC();
            var wallet = walletDac.GetByAccountId(account.Id, crypto.Id);
            UserExTransferOrder order;
            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = new UserWalletComponent().GenerateWallet(account.Id, crypto.Id);

                walletDac.Increase(wallet.Id, amount);
                order = new UserExTransferOrder
                {
                    Timestamp = DateTime.UtcNow,
                    OrderNo = CreateOrderNo(),
                    OrderType = ExTransferType.FromEx,
                    AccountId = account.Id,
                    WalletId = wallet.Id,
                    CryptoId = crypto.Id,
                    CryptoCode = crypto.Code,
                    Amount = amount,
                    Status = 1,
                    Remark = null,
                    ExId = recordId
                };

                order = new UserExTransferOrderDAC().Create(order);
                scope.Complete();
            }


            try
            {
                FiiiEXTransferMSMQ.PubUserTransferFromEx(order.Id, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Info("PubUserTransferFromEx - error", ex);
            }
            return new TransferResult
            {
                Id = order.Id,
                Amount = order.Amount.ToString(crypto.DecimalPlace),
                OrderNo = order.OrderNo,
                Timestamp = order.Timestamp.ToUnixTime(),
                CryptoCode = crypto.Code
            };
        }


        public UserExTransferOrderOM FiiiPayTransferDetail(UserAccount account, long id)
        {
            var dac = new UserExTransferOrderDAC();
            var order = dac.GetById(id);
            if (order == null)
            {
                return null;
            }

            if (order.AccountId != account.Id)
            {
                return null;
            }

            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            return new UserExTransferOrderOM
            {
                Id = order.Id,
                Amount = order.Amount.ToString(coin.DecimalPlace),
                ExTransferType = order.OrderType,
                ExTransferTypeStr = GetExTransferTypeString(order.OrderType),
                CryptoCode = coin.Code,
                Status = order.Status,
                StatusStr = R.OrderCompleted,
                Timestamp = order.Timestamp.ToUnixTime(),
                OrderNo = order.OrderNo
            };
        }

        private string GetExTransferTypeString(ExTransferType orderType)
        {
            switch (orderType)
            {
                case ExTransferType.FromEx:
                    return R.从FiiiEX划入;
                case ExTransferType.ToEx:
                    return R.划出到FiiiEX;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null);
            }
        }

        public TransferFiiiExConditionOM FiiiPayTransferFiiiExCondition(UserAccount account, int cryptoId)
        {
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            var walletDac = new UserWalletDAC();
            var wallet = walletDac.GetByAccountId(account.Id, crypto.Id);

            var fiiiExBalance = this.FiiiExBalance(FiiiType.FiiiPay, account.Id, crypto);

            var minQuantity =
                (1M / (decimal)Math.Pow(10, crypto.DecimalPlace)).ToString(crypto.DecimalPlace);
            return new TransferFiiiExConditionOM
            {
                Balance = (wallet?.Balance ?? 0M).ToString(crypto.DecimalPlace),
                MinQuantity = minQuantity,
                FiiiExBalance = fiiiExBalance.ToString(crypto.DecimalPlace),
                FiiiExMinQuantity = minQuantity
            };
        }
    }
}