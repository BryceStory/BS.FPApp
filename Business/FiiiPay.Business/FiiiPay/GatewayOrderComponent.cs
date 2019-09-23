using FiiiPay.Data;
using FiiiPay.DTO.GatewayOrder;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using Newtonsoft.Json;
using System;
using FiiiPay.Business.Properties;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Queue;
using FiiiPay.DTO;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework.Component.Enums;

namespace FiiiPay.Business
{
    public class GatewayOrderComponent : BaseComponent
    {
        public GatewayOrderDetailOM Detail(Guid orderId, UserAccount user, bool isZH)
        {
            var order = new GatewayOrderDAC().GetByOrderId(orderId);
            if ((order == null || !order.UserAccountId.HasValue || order.UserAccountId != user.Id))
            {
                throw new ApplicationException(MessageResources.OrderNotFound);
            }
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            var er = order.ExchangeRate;
            var cer = GetExchangeRate(user.CountryId, order.FiatCurrency, coin);
            var iRate = ((cer - er) / er) * 100;
            var om = new GatewayOrderDetailOM
            {
                Code = coin.Code,
                CryptoAmount = order.ActualCryptoAmount.ToString(coin.DecimalPlace),
                FiatAmount = order.ActualFiatAmount?.ToString(2) ?? string.Empty,
                ExchangeRate = $"1 {coin.Code} = {order.ExchangeRate.ToString(4)} {order.FiatCurrency}",
                FiatCurrency = order.FiatCurrency,
                MarkUp = $"{(order.Markup * 100).ToString(2)}%",
                MerchantName = order.MerchantName,
                Id = order.Id,
                Status = new UserStatementComponent().GetStatusStr((int)BillType.GatewayOrder, (int)order.Status, isZH),
                Timestamp = order.Timestamp.ToUnixTime().ToString(),
                Type = Resources.BillTypeNetOrder,
                OrderNo = order.OrderNo,
                TradeNo = order.TradeNo,
                RefundTimestamp = order.Status == GatewayOrderStatus.Refunded ? new GatewayRefundOrderDAC().GetByOrderId(order.Id)?.Timestamp.ToUnixTime().ToString() : "",
                CurrentExchangeRate = $"1 {coin.Code} = {cer.ToString(4)} {order.FiatCurrency}",
                IncreaseRate = iRate > 0 ? $"+{iRate.ToString(2)}" : iRate.ToString(2)
            };
            return om;
        }

        private decimal GetExchangeRate(int countryId, string fiatCurrency, Cryptocurrency crypto)
        {
            var agent = new MarketPriceComponent();
            var price = agent.GetMarketPrice(fiatCurrency, crypto.Code);
            if (price == null) return 0M;
            return price.Price;
        }

        public GatewayOrderInfoOM PrePay(string code, UserAccount account)
        {
            var codeEntity = QRCode.Deserialize(code);

            if (codeEntity.SystemPlatform != SystemPlatform.Gateway || codeEntity.QrCodeEnum != QRCodeEnum.GateWayPay)
                throw new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode);

            var orderDetail = GetGatewayOrderDetail(codeEntity.QRCodeKey);
            
            if(orderDetail == null)
                throw new CommonException(ReasonCode.INVALID_QRCODE, MessageResources.InvalidQRCode);


            var cryptoCurrency = new CryptocurrencyDAC().GetByCode(orderDetail.Crypto);
            if (cryptoCurrency == null || cryptoCurrency?.Id == null)
                throw new CommonException(ReasonCode.CRYPTO_NOT_EXISTS, "Error: Invalid Cryptocurrency");

            var goDAC = new GatewayOrderDAC();
            var gatewayOrder = goDAC.GetByTradeNo(orderDetail.Id.ToString());
            if (gatewayOrder != null)
            {
                if(gatewayOrder.Status!= GatewayOrderStatus.Pending)
                {
                    throw new CommonException(ReasonCode.ORDER_HAD_COMPLETE, MessageResources.OrderComplated);
                }
            }
            else
            {
                gatewayOrder = new GatewayOrder()
                {
                    OrderNo = IdentityHelper.OrderNo(),
                    TradeNo = orderDetail.Id.ToString(),
                    Id = Guid.NewGuid(),
                    CryptoId = cryptoCurrency.Id,
                    MerchantAccountId = orderDetail.MerchantAccountId,
                    FiatAmount = orderDetail.FiatAmount,
                    MerchantName = orderDetail.MerchantName,
                    UserAccountId = account.Id,
                    CryptoAmount = orderDetail.CryptoAmount,
                    ActualCryptoAmount = orderDetail.ActualCryptoAmount,
                    FiatCurrency = orderDetail.FiatCurrency,
                    Markup = orderDetail.MarkupRate,
                    ActualFiatAmount = orderDetail.ActualFiatAmount,
                    Status = GatewayOrderStatus.Pending,
                    ExchangeRate = orderDetail.ExchangeRate,
                    ExpiredTime = orderDetail.ExpiredTime,
                    TransactionFee = orderDetail.TransactionFee,
                    Timestamp = DateTime.UtcNow,
                    Remark = orderDetail.Remark
                };
                goDAC.Insert(gatewayOrder);
            }
            
            return new GatewayOrderInfoOM()
            {
                Timestamp = DateTime.UtcNow.ToUnixTime().ToString(),
                OrderId = gatewayOrder.Id,
                MerchantName = gatewayOrder.MerchantName,
                CryptoCode = cryptoCurrency.Code,
                ActurlCryptoAmount = gatewayOrder.ActualCryptoAmount
            };
        }
        
        /// <summary>
        /// FiiiPay扫描第三方二维码支付
        /// </summary>
        /// <param name="code">二维码字符</param>
        /// <returns></returns>
        public GatewayOrderInfoOM Pay(GatewayPayIM im, UserAccount account)
        {
            new SecurityComponent().VerifyPin(account, im.Pin);

            var gatewayOrder = new GatewayOrderDAC().GetByOrderId(im.OrderId);
            if (gatewayOrder.Status != GatewayOrderStatus.Pending)
            {
                throw new CommonException(ReasonCode.ORDER_HAD_COMPLETE, MessageResources.OrderComplated);
            }
            var cryptoCurrency = new CryptocurrencyDAC().GetById(gatewayOrder.CryptoId);
            if (cryptoCurrency == null || cryptoCurrency?.Id == null)
                throw new CommonException(ReasonCode.CRYPTO_NOT_EXISTS, "Error: Invalid Cryptocurrency");
            var uwComponent = new UserWalletComponent();
            var userWallet = uwComponent.GetUserWallet(gatewayOrder.UserAccountId.Value, gatewayOrder.CryptoId);
            if (userWallet == null)
            {
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            }
            if (userWallet.Balance < gatewayOrder.CryptoAmount)
            {
                throw new CommonException(ReasonCode.INSUFFICIENT_BALANCE, MessageResources.InsufficientBalance);
            }
            var uwDAC = new UserWalletDAC();
            var uwsDAC = new UserWalletStatementDAC();
            var goDAC = new GatewayOrderDAC();
            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                uwDAC.Decrease(userWallet.Id, gatewayOrder.ActualCryptoAmount);
                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = userWallet.Id,
                    Action = UserWalletStatementAction.TansferOut,
                    Amount = -gatewayOrder.ActualCryptoAmount,
                    Balance = userWallet.Balance - gatewayOrder.ActualCryptoAmount,
                    FrozenAmount = 0,
                    FrozenBalance = userWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });
                gatewayOrder.Status = GatewayOrderStatus.Completed;
                gatewayOrder.PaymentTime = DateTime.UtcNow;
                goDAC.Update(gatewayOrder);

                new UserTransactionDAC().Insert(new UserTransaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = userWallet.UserAccountId,
                    CryptoId = cryptoCurrency.Id,
                    CryptoCode = cryptoCurrency.Code,
                    Type = UserTransactionType.Order,
                    DetailId = gatewayOrder.Id.ToString(),
                    Status = (byte)gatewayOrder.Status,
                    Timestamp = DateTime.UtcNow,
                    Amount = gatewayOrder.CryptoAmount,
                    OrderNo = gatewayOrder.OrderNo,
                    MerchantName = gatewayOrder.MerchantName
                });
                scope.Complete();
            }
            RabbitMQSender.SendMessage("FiiiPay_Gateway_PayCompleted", gatewayOrder.TradeNo);
            return new GatewayOrderInfoOM()
            {
                Timestamp = gatewayOrder.PaymentTime?.ToUnixTime().ToString(),
                OrderId = gatewayOrder.Id,
                MerchantName = gatewayOrder.MerchantName,
                CryptoCode = cryptoCurrency.Code,
                ActurlCryptoAmount = gatewayOrder.ActualCryptoAmount
            };
        }

        private GatewayOrderOM GetGatewayOrderDetail(string id)
        {
            var json = GatewayAgent.GetToGateway("/api/Order/GetOrderDetail?orderId=" + id);
            ServiceResult<GatewayOrderOM> result = JsonConvert.DeserializeObject<ServiceResult<GatewayOrderOM>>(json);

            if (result.Code == (int)System.Net.HttpStatusCode.OK || result.Code == 0)
            {
                return result.Data;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 支付网关发起退款
        /// </summary>
        /// <param name="tradeId"></param>
        public void GatewayRefund(string tradeId)
        {
            //获取订单详情
            var orderDetail = new GatewayOrderDAC().GetByTradeNo(tradeId);

            if (orderDetail == null)
                throw new Exception("Have no record of this order:tradeId=" + tradeId);
            if (orderDetail.Status != GatewayOrderStatus.Completed)
                throw new Exception($"The status of this order {tradeId} is not completed");

            var uwComponent = new UserWalletComponent();
            var userWallet = uwComponent.GetUserWallet(orderDetail.UserAccountId.Value, orderDetail.CryptoId);
            var goDAC = new GatewayOrderDAC();
            var rgoDAC = new GatewayRefundOrderDAC();
            var uwDAC = new UserWalletDAC();
            var uwsDAC = new UserWalletStatementDAC();

            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                orderDetail.Status = GatewayOrderStatus.Refunded;
                goDAC.Update(orderDetail);
                
                uwDAC.Increase(userWallet.Id, orderDetail.ActualCryptoAmount);
                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = userWallet.Id,
                    Action = UserWalletStatementAction.TansferIn,
                    Amount = orderDetail.ActualCryptoAmount,
                    Balance = userWallet.Balance + orderDetail.ActualCryptoAmount,
                    FrozenAmount = 0,
                    FrozenBalance = userWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });
                rgoDAC.Insert(new GatewayRefundOrder
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderDetail.Id,
                    Timestamp = DateTime.UtcNow,
                    Status = RefundStatus.Completed
                });

                scope.Complete();
            }
        }
        /// <summary>
        /// 支付网关成功
        /// </summary>
        /// <param name="id"></param>
        public MessageGatewayPaymentOM MessagePaymentSuccess(UserAccount account, Guid id)
        {
            var order = new GatewayOrderDAC().GetByOrderId(id);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(order.CryptoId);
            var currentExchangeRate = GetExchangeRate(account.CountryId, order.FiatCurrency, cryptoCurrency);
            return new MessageGatewayPaymentOM()
            {
                TradeStatus = 0,
                CryptoCode = cryptoCurrency.Code,
                CurrentExchangeRate = currentExchangeRate,
                FiatCode = order.FiatCurrency,
                IncreaseRate = (currentExchangeRate - order.ExchangeRate) / order.ExchangeRate,
                Markup = order.Markup,
                MerchantName = order.MerchantName,
                MerchantOrderNo = order.OrderNo,
                OrderNo = order.TradeNo,
                PaymentCryptoAmount = order.ActualCryptoAmount,
                PaymentFiatAmount = order.ActualFiatAmount ?? 0m,
                TradeExchangeRate = order.ExchangeRate,
                TradeTime = order.Timestamp.ToUnixTime().ToString()
            };
        }

        /// <summary>
        /// 支付网关退款成功
        /// </summary>
        /// <param name="id"></param>
        public MessageGatewayPaymentOM MessagePaymentRefund(UserAccount account, Guid id)
        {
            var refundOrder = new GatewayRefundOrderDAC().GetById(id);
            var order = new GatewayOrderDAC().GetByOrderId(refundOrder.OrderId);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(order.CryptoId);
            var currentExchangeRate = GetExchangeRate(account.CountryId, order.FiatCurrency, cryptoCurrency);
            return new MessageGatewayPaymentOM()
            {
                TradeStatus = 1,
                CryptoCode = cryptoCurrency.Code,
                CurrentExchangeRate = currentExchangeRate,
                FiatCode = order.FiatCurrency,
                IncreaseRate = (currentExchangeRate - order.ExchangeRate) / order.ExchangeRate,
                Markup = order.Markup,
                MerchantName = order.MerchantName,
                MerchantOrderNo = order.OrderNo,
                OrderNo = order.TradeNo,
                PaymentCryptoAmount = order.ActualCryptoAmount,
                PaymentFiatAmount = order.ActualFiatAmount ?? 0m,
                TradeExchangeRate = order.ExchangeRate,
                TradeTime = order.Timestamp.ToUnixTime().ToString(),
                RefundTime = refundOrder.Timestamp.ToUnixTime().ToString()
            };
        }
    }
}
