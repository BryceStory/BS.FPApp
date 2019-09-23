using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;
using FiiiPOS.DTO;

namespace FiiiPay.Business
{
    public partial class FiiiEXTransferComponent
    {
        public string FiiiPOSTransferInto(MerchantAccount account, Cryptocurrency coin, decimal amount)
        {
            var walletDac = new MerchantWalletDAC();
            var wallet = walletDac.GetByAccountId(account.Id, coin.Id);
            MerchantExTransferOrder order;
            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = new MerchantWalletComponent().GenerateWallet(account.Id, coin.Id);

                walletDac.Increase(wallet.Id, amount);

                order = new MerchantExTransferOrder
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

                new MerchantExTransferOrderDAC().Create(order);

                scope.Complete();
            }

            try
            {
                FiiiEXTransferMSMQ.PubMerchantTransferFromEx(order.Id, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Info("PubMerchantTransferFromEx - error", ex);
            }
            return order.OrderNo;
        }

        public TransferResult FiiiPOSTransferToEx(Guid accountId, int cryptoId, decimal amount, string pinToken)
        {
            var securityVerify = new SecurityVerification(SystemPlatform.FiiiPOS);
            securityVerify.VerifyToken(accountId, pinToken, SecurityMethod.Pin);

            var openAccountDac = new OpenAccountDAC();
            var openAccount = openAccountDac.GetOpenAccount(FiiiType.FiiiPOS, accountId);
            if (openAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.FiiiExAccountNotExist);

            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            if (crypto == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.CurrencyForbidden);

            var walletDac = new MerchantWalletDAC();
            var wallet = walletDac.GetByAccountId(accountId, crypto.Id);

            if (wallet.Balance < amount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.余额不足);
            //10091=参数不符合要求 10013=用户信息不存在 10020=币不存在 0=成功
            int result = FiiiExCoinIn(openAccount.OpenId, crypto.Code, amount, out string recordId);
            if (result != 0)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.从FiiiEx划转失败);
            MerchantExTransferOrder order;
            using (var scope = new TransactionScope())
            {
                walletDac.Decrease(wallet.Id, amount);
                order = new MerchantExTransferOrder
                {
                    Timestamp = DateTime.UtcNow,
                    OrderNo = CreateOrderNo(),
                    OrderType = ExTransferType.ToEx,
                    AccountId = accountId,
                    WalletId = wallet.Id,
                    CryptoId = crypto.Id,
                    CryptoCode = crypto.Code,
                    Amount = amount,
                    Status = 1,
                    Remark = null,
                    ExId = recordId
                };

                order = new MerchantExTransferOrderDAC().Create(order);
                scope.Complete();
            }

            try
            {
                FiiiEXTransferMSMQ.PubMerchantTransferToEx(order.Id, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Info("FiiiPOSTransferToEx - error", ex);
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
        public TransferResult FiiiPOSTransferFromEx(Guid accountId, int cryptoId, decimal amount, string pinToken)
        {
            var securityVerify = new SecurityVerification(SystemPlatform.FiiiPOS);
            securityVerify.VerifyToken(accountId, pinToken, SecurityMethod.Pin);

            var openAccountDac = new OpenAccountDAC();
            var openAccount = openAccountDac.GetOpenAccount(FiiiType.FiiiPOS, accountId);
            if (openAccount == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.FiiiExAccountNotExist);

            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            if (crypto == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, R.CurrencyForbidden);

            var balance = this.FiiiExBalance(FiiiType.FiiiPOS, accountId, crypto);
            if (balance < amount)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.余额不足);

            var walletDac = new MerchantWalletDAC();
            var wallet = walletDac.GetByAccountId(accountId, crypto.Id);

            //10091=参数不符合要求 10013=用户信息不存在 10024=用户币不存在 10025=用户币种余额不足 0=成功
            int result = FiiiExCoinOut(openAccount.OpenId, crypto.Code, amount, out string recordId);
            if (result != 0)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.划转到FiiiEx失败);
            MerchantExTransferOrder order;
            using (var scope = new TransactionScope())
            {
                if (wallet == null)
                    wallet = new MerchantWalletComponent().GenerateWallet(accountId, cryptoId);
                walletDac.Increase(wallet.Id, amount);
                order = new MerchantExTransferOrder
                {
                    Timestamp = DateTime.UtcNow,
                    OrderNo = CreateOrderNo(),
                    OrderType = ExTransferType.FromEx,
                    AccountId = accountId,
                    WalletId = wallet.Id,
                    CryptoId = crypto.Id,
                    CryptoCode = crypto.Code,
                    Amount = amount,
                    Status = 1,
                    Remark = null,
                    ExId = recordId
                };

                new MerchantExTransferOrderDAC().Create(order);
                scope.Complete();
            }
            try
            {
                FiiiEXTransferMSMQ.PubMerchantTransferFromEx(order.Id, 0);
            }
            catch (Exception ex)
            {
                LogHelper.Info("PubMerchantTransferFromEx - error", ex);
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

        public MerchantExTransferOrderDTO FiiiPOSTransferDetail(Guid accountId, long id)
        {
            var dac = new MerchantExTransferOrderDAC();
            var order = dac.GetById(id);
            if (order == null)
            {
                return null;
            }

            if (order.AccountId != accountId)
            {
                return null;
            }

            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            return new MerchantExTransferOrderDTO
            {
                Id = order.Id,
                Amount = order.Amount.ToString(coin.DecimalPlace),
                ExTransferType = order.OrderType,
                CryptoCode = coin.Code,
                Status = order.Status,
                Timestamp = order.Timestamp.ToUnixTime(),
                OrderNo = order.OrderNo
            };
        }

        public List<MerchantExTransferOrderDTO> FiiiPOSTransferList(Guid accountId, int pageIndex, int pageSize)
        {
            var dac = new MerchantExTransferOrderDAC();
            List<MerchantExTransferOrder> list = dac.PagedByAccountId(accountId, pageIndex, pageSize);

            var cryptoList = new CryptocurrencyDAC().GetAll();

            return list.Select(e =>
            {
                var crypto = cryptoList.FirstOrDefault(c => c.Id == e.CryptoId);
                return new MerchantExTransferOrderDTO
                {
                    Id = e.Id,
                    Amount = e.Amount.ToString(crypto?.DecimalPlace ?? 8),
                    CryptoCode = e.CryptoCode,
                    ExTransferType = e.OrderType,
                    OrderNo = e.OrderNo,
                    Status = e.Status,
                    Timestamp = e.Timestamp.ToUnixTime()
                };
            }).ToList();
        }

        public TransferFiiiExConditionDTO FiiiPOSTransferFiiiExCondition(Guid accountId, int cryptoId)
        {
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            var walletDac = new MerchantWalletDAC();
            var wallet = walletDac.GetByAccountId(accountId, crypto.Id);

            var fiiiExBalance = this.FiiiExBalance(FiiiType.FiiiPOS, accountId, crypto);

            var minQuantity =
                (1M / (decimal)Math.Pow(10, crypto.DecimalPlace)).ToString(crypto.DecimalPlace);
            return new TransferFiiiExConditionDTO
            {
                Balance = (wallet?.Balance ?? 0M).ToString(crypto.DecimalPlace),
                MinQuantity = minQuantity,
                FiiiExBalance = fiiiExBalance.ToString(crypto.DecimalPlace),
                FiiiExMinQuantity = minQuantity
            };
        }
    }
}