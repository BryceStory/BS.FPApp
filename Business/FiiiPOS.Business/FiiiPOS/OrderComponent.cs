using System;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.DTO.Order;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;
using Newtonsoft.Json;

namespace FiiiPOS.Business.FiiiPOS
{
    public partial class OrderComponent
    {
        public string CreateOrder(Guid merchantAccountId, string fiatCurrency, int cryptoId, decimal amount, PaymentType paymentType, string userToken, string clientIP)
        {
            var accountDAC = new MerchantAccountDAC();

            var account = accountDAC.GetById(merchantAccountId);

            if (!account.IsAllowAcceptPayment)
                throw new CommonException(ReasonCode.Not_Allow_Withdrawal, Resources.禁止收款);

            var coin = new CryptocurrencyDAC().GetById(cryptoId);
            if (!coin.Status.HasFlag(CryptoStatus.Pay))
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, Resources.CurrencyForbidden);

            if (coin.Enable == (byte)CurrencyStatus.Forbidden)
                throw new CommonException(ReasonCode.CURRENCY_FORBIDDEN, Resources.CurrencyForbidden);

            var order = new RedisOrderDTO()
            {
                FiatAmount = amount,
                CryptoId = cryptoId,
                FiatCurrency = fiatCurrency,
                MerchantGuid = account.Id,
                OrderNo = NumberGenerator.GenerateUnixOrderNo(),
                CountryId = account.CountryId,
                Markup = account.Markup,
                Type = paymentType,
                UserId = Guid.Empty
            };

            Guid? userAccountId = null;
            if (!string.IsNullOrEmpty(userToken))
            {
                if (!userToken.StartsWith(Constant.PAYMENT_CODE_PREFIX))
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.无效的用户Token);

                var paymentInfo = RedisHelper.Get<PaymentCodeDTO>(
                    Constant.REDIS_PAYMENT_CODE_DBINDEX,
                    $"{Constant.REDIS_PAYMENT_CODE_PREFIX}{userToken}");
                if (paymentInfo == null)
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.无效的用户Token);
                userAccountId = paymentInfo.UserId;
            }
            if (userAccountId.HasValue)
            {
                order.UserId = userAccountId.Value;

                SendPayOrderModel model = new SendPayOrderModel();
                model.OrderNo = order.OrderNo;
                model.UserAccountId = userAccountId.Value.ToString();

                MerchantMSMQ.PubPayOrder(model, 0);
            }
            RedisHelper.StringSet($"fiiipos:order:{ order.OrderNo}", JsonConvert.SerializeObject(order), TimeSpan.FromMinutes(30));
            return order.OrderNo;
        }

        public void Refund(Guid accountId, string orderNo, string pinToken)
        {
            var merchantAccountDAC = new MerchantAccountDAC();
            var merchantAccount = merchantAccountDAC.GetById(accountId);
            if (merchantAccount == null) return;

            new SecurityVerification(SystemPlatform.FiiiPOS).VerifyToken(merchantAccount.Id, pinToken, SecurityMethod.Pin);
            var orderDac = new OrderDAC();

            var order = orderDac.GetByOrderNo(orderNo);
            if (order == null)
                throw new CommonException(10000, Resources.订单不存在);

            if (merchantAccount.Id != order.MerchantAccountId)
                return;

            if (order.Status != OrderStatus.Completed)
                throw new CommonException(10000, Resources.订单状态异常);

            if (DateTime.UtcNow.AddDays(-3) > order.PaymentTime.Value)
                throw new CommonException(10000, Resources.订单超过三天不能退款);

            var merchantWalletDAC = new MerchantWalletDAC();
            var userWalletDAC = new UserWalletDAC();

            var merchantWallet = merchantWalletDAC.GetByAccountId(order.MerchantAccountId, order.CryptoId);
            if (merchantWallet == null)
                throw new CommonException(10000, Resources.商户不支持的币种);

            if (merchantWallet.Balance < order.ActualCryptoAmount)
                throw new CommonException(10000, Resources.余额不足);

            var userWallet = userWalletDAC.GetByAccountId(order.UserAccountId.Value, order.CryptoId);
            if (userWallet == null)
                throw new CommonException(10000, Resources.用户不支持的币种);
            var orderWithdrawalFee = new OrderWithdrawalFeeDAC().GetByOrderId(order.Id);
            if (orderWithdrawalFee != null && orderWithdrawalFee.Amount > 0)
            {
                var merchantOrderWithdrawalFeeWallet = merchantWalletDAC.GetByAccountId(order.MerchantAccountId, orderWithdrawalFee.CryptoId);
                using (var scope = new TransactionScope())
                {
                    merchantWalletDAC.Decrease(merchantAccount.Id, order.CryptoId, order.ActualCryptoAmount);
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        WalletId = merchantWallet.Id,
                        Action = "REFUND",
                        Amount = -order.ActualCryptoAmount,
                        Balance = merchantWallet.Balance - order.ActualCryptoAmount,
                        Timestamp = DateTime.UtcNow,
                        Remark = $"Refund to order({order.OrderNo})"
                    });

                    merchantWalletDAC.Increase(merchantAccount.Id, orderWithdrawalFee.CryptoId, orderWithdrawalFee.Amount);
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        WalletId = merchantOrderWithdrawalFeeWallet.Id,
                        Action = "REFUND",
                        Amount = orderWithdrawalFee.Amount,
                        Balance = merchantWallet.Balance - orderWithdrawalFee.Amount,
                        Timestamp = DateTime.UtcNow,
                        Remark = $"Refund to order({order.OrderNo})"
                    });

                    userWalletDAC.Increase(order.UserAccountId.Value, order.CryptoId, order.CryptoAmount);
                    new UserWalletStatementDAC().Insert(new UserWalletStatement
                    {
                        WalletId = userWallet.Id,
                        Action = "REFUND",
                        Amount = order.CryptoAmount,
                        Balance = userWallet.Balance + order.CryptoAmount,
                        Timestamp = DateTime.UtcNow,
                        Remark = $"Refund from order({order.OrderNo})"
                    });

                    order.Status = OrderStatus.Refunded;
                    order.Timestamp = DateTime.UtcNow;
                    orderDac.UpdateStatus(order);

                    new RefundDAC().Insert(new Refund
                    {
                        OrderId = order.Id,
                        Status = RefundStatus.Completed,
                        Timestamp = DateTime.UtcNow
                    });

                    scope.Complete();
                }
            }
            else
            {
                using (var scope = new TransactionScope())
                {
                    merchantWalletDAC.Decrease(merchantAccount.Id, order.CryptoId, order.ActualCryptoAmount);
                    new MerchantWalletStatementDAC().Insert(new MerchantWalletStatement
                    {
                        WalletId = merchantWallet.Id,
                        Action = "REFUND",
                        Amount = -order.ActualCryptoAmount,
                        Balance = merchantWallet.Balance - order.ActualCryptoAmount,
                        Timestamp = DateTime.UtcNow,
                        Remark = $"Refund to order({order.OrderNo})"
                    });

                    userWalletDAC.Increase(order.UserAccountId.Value, order.CryptoId, order.CryptoAmount);
                    new UserWalletStatementDAC().Insert(new UserWalletStatement
                    {
                        WalletId = userWallet.Id,
                        Action = "REFUND",
                        Amount = order.CryptoAmount,
                        Balance = userWallet.Balance + order.CryptoAmount,
                        Timestamp = DateTime.UtcNow,
                        Remark = $"Refund from order({order.OrderNo})"
                    });

                    order.Status = OrderStatus.Refunded;
                    //order.Timestamp = DateTime.UtcNow;
                    orderDac.UpdateStatus(order);

                    new RefundDAC().Insert(new Refund
                    {
                        OrderId = order.Id,
                        Status = RefundStatus.Completed,
                        Timestamp = DateTime.UtcNow
                    });

                    scope.Complete();
                }
            }

            MerchantMSMQ.PubRefundOrder(order.OrderNo, 0);

        }

        public OrderStatusDTO GetStatusByOrderNo(Guid merchantAccountId, string orderNo)
        {
            var order = new OrderDAC().GetByOrderNo(orderNo);
            return new OrderStatusDTO()
            {
                OrderStatus = order?.Status ?? OrderStatus.Pending,
                Id = order == null ? String.Empty : order.Id.ToString()
            };
        }

        public OrderDetailDTO GetByOrderNo(Guid merchantAccountId, string orderNo)
        {
            var order = new OrderDAC().GetByOrderNo(orderNo);

            var merchantAccount = new MerchantAccountDAC().GetById(merchantAccountId);

            var pos = new POSDAC().GetById(merchantAccount.POSId.Value);
            if (order == null)
                throw new CommonException(10000, Resources.订单不存在);

            if (order.MerchantAccountId != merchantAccountId)
                throw new CommonException(10000, Resources.只能查看自己的订单);



            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            var er = order.ExchangeRate;
            var cer = GetExchangeRate(merchantAccount.CountryId, order.FiatCurrency, coin);
            var iRate = ((cer - er) / er) * 100;
            // 手续费
            var fee = new OrderWithdrawalFeeDAC().GetByOrderId(order.Id);
            var feeCoin = new Cryptocurrency();
            if (fee != null)
            {
                feeCoin = new CryptocurrencyDAC().GetById(fee.CryptoId);
            }
            var result = new OrderDetailDTO
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                OrderStatus = order.Status,
                Timestamp = order.Timestamp.ToUnixTime(),
                CryptoStatus = coin.Status,
                CryptoCode = coin.Code,
                CryptoAmount = order.CryptoAmount.ToString(coin.DecimalPlace),
                FiatCurrency = order.FiatCurrency,
                FiatAmount = order.FiatAmount.ToString(2),
                Markup = order.Markup,
                ActualFiatAmount = order.ActualFiatAmount.ToString(2),
                TransactionFee = (fee == null || fee.Amount == 0) ? order.TransactionFee.ToString(coin.DecimalPlace) : fee.Amount.ToString(feeCoin.DecimalPlace),
                ActualCryptoAmount = order.ActualCryptoAmount.ToString(coin.DecimalPlace),
                UserAccount = order.UserAccountId.HasValue ? GetUserMastMaskedCellphone(order.UserAccountId.Value) : string.Empty,
                SN = pos.Sn,
                ExchangeRate = er.ToString(4),
                CurrentExchangeRate = coin.Enable == 1 ? $"1 {coin.Code} = {cer.ToString(4)} {order.FiatCurrency}" : "--",
                IncreaseRate = coin.Enable == 1 ? (iRate > 0 ? $"+{iRate.ToString(2)}%" : iRate.ToString(2) + "%") : "--",
                FeeCryptoCode = (fee == null || fee.Amount == 0) ? coin.Code : feeCoin.Code,
                CryptoEnable = coin.Enable
            };

            if (result.OrderStatus == OrderStatus.Refunded)
            {
                var refund = new RefundDAC().GetByOrderId(result.Id);
                if (refund?.Timestamp != null)
                    result.RefundTimestamp = refund.Timestamp.ToUnixTime();
            }
            return result;
        }


        public OrderDetailDTO GetById(Guid merchantAccountId, Guid orderId)
        {
            var order = new OrderDAC().GetById(orderId);

            var merchantAccount = new MerchantAccountDAC().GetById(merchantAccountId);

            var pos = new POSDAC().GetById(merchantAccount.POSId.Value);
            if (order == null)
                throw new CommonException(10000, Resources.订单不存在);

            if (order.MerchantAccountId != merchantAccountId)
                throw new CommonException(10000, Resources.只能查看自己的订单);

            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            var er = order.ExchangeRate;
            var cer = GetExchangeRate(merchantAccount.CountryId, order.FiatCurrency, coin);
            var iRate = ((cer - er) / er) * 100;

            var result = new OrderDetailDTO
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                OrderStatus = order.Status,
                Timestamp = order.Timestamp.ToUnixTime(),
                CryptoStatus = coin.Status,
                CryptoEnable = coin.Enable,
                CryptoCode = coin.Code,
                CryptoAmount = order.CryptoAmount.ToString(coin.DecimalPlace),
                FiatCurrency = order.FiatCurrency,
                FiatAmount = order.FiatAmount.ToString(2),
                Markup = order.Markup,
                ActualFiatAmount = order.ActualFiatAmount.ToString(2),
                TransactionFee = order.TransactionFee.ToString(coin.DecimalPlace),
                ActualCryptoAmount = order.ActualCryptoAmount.ToString(coin.DecimalPlace),
                UserAccount = order.UserAccountId.HasValue ? GetUserMastMaskedCellphone(order.UserAccountId.Value) : string.Empty,
                SN = pos.Sn,
                ExchangeRate = er.ToString(4),
                CurrentExchangeRate = cer.ToString(4),
                IncreaseRate = iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2)
            };

            if (result.OrderStatus == OrderStatus.Refunded)
            {
                var refund = new RefundDAC().GetByOrderId(result.Id);
                if (refund?.Timestamp != null)
                    result.RefundTimestamp = refund.Timestamp.ToUnixTime();
            }
            return result;
        }

        public PrintOrderInfoDTO PrintOrder(Guid merchantAccountId, string orderNo)
        {
            var order = new OrderDAC().GetByOrderNo(orderNo);

            var merchantAccount = new MerchantAccountDAC().GetById(merchantAccountId);

            var pos = new POSDAC().GetById(merchantAccount.POSId.Value);
            if (order == null)
                throw new CommonException(10000, Resources.订单不存在);

            if (order.MerchantAccountId != merchantAccountId)
                throw new CommonException(10000, Resources.只能查看自己的订单);

            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            var result = new PrintOrderInfoDTO
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                OrderStatus = order.Status,
                Timestamp = order.Timestamp.ToUnixTime(),
                CryptoCode = coin.Code,
                CryptoAmount = order.CryptoAmount.ToString(coin.DecimalPlace),
                FiatCurrency = order.FiatCurrency,
                FiatAmount = order.FiatAmount.ToString(2),
                Markup = order.Markup,
                ActualFiatAmount = order.ActualFiatAmount.ToString(2),
                ExchangeRate = $"1 {coin.Code} = {order.ExchangeRate.ToString(4)} {order.FiatCurrency}" ,
                TransactionFee = order.TransactionFee.ToString(coin.DecimalPlace),
                ActualCryptoAmount = order.ActualCryptoAmount.ToString(coin.DecimalPlace),
                UserAccount = order.UserAccountId.HasValue ? GetUserMastMaskedCellphone(order.UserAccountId.Value) : string.Empty,
                SN = pos.Sn,
                AvatarId = merchantAccount.Photo,
                MerchantName = merchantAccount.MerchantName,
                CryptoImage = coin.IconURL
            };

            if (result.OrderStatus == OrderStatus.Refunded)
            {
                var refund = new RefundDAC().GetByOrderId(result.Id);
                if (refund?.Timestamp != null)
                    result.RefundTimestamp = refund.Timestamp.ToUnixTime();
            }
            return result;
        }

        public Order GetOrder(string orderno)
        {
            return new OrderDAC().GetByOrderNo(orderno);
        }

        private string GetUserMastMaskedCellphone(Guid userAccountId)
        {
            var user = new UserAccountDAC().GetById(userAccountId);
            var country = new CountryComponent().GetById(user.CountryId);

            return CellphoneExtension.GetMaskedCellphone(country.PhoneCode, user.Cellphone);
        }

        private decimal GetExchangeRate(int countryId, string fiatCurrency, Cryptocurrency crypto)
        {
            var price = new MarketPriceComponent().GetMarketPrice(fiatCurrency, crypto.Code);
            if (price == null) return 0M;
            return price.Price;
        }
    }
}