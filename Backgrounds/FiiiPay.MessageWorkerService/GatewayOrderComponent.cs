using System;
using System.Collections.Generic;
using System.Configuration;
using FiiiPay.Data;
using FiiiPay.DTO.GatewayOrder;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Queue;
using FiiiPay.MessageWorkerService.Properties;
using Newtonsoft.Json;

namespace FiiiPay.MessageWorkerService
{
    internal class GatewayOrderComponent
    {
        /// <summary>
        /// 支付网关订单查询失败
        /// </summary>
        public const int GATEWAY_ORDER_NOT_EXISTS = 10140;

        /// <summary>
        /// 加密币不存在
        /// </summary>
        public const int CRYPTO_NOT_EXISTS = 10016;

        /// <summary>
        /// 余额不足
        /// </summary>
        public const int INSUFFICIENT_BALANCE = 10030;

        /// <summary>
        /// 订单已完成或者已退款
        /// </summary>
        public const int ORDER_HAD_COMPLETE = 10150;

        /// <summary>
        /// 支付网关发起支付
        /// </summary>
        /// <param name="tradeId">支付网关订单号</param>
        public GatewayOrderInfoOM GatewayPay(string tradeId)
        {
            var orderDetail = GetGatewayOrderDetail(tradeId);

            if (orderDetail == null)
                throw new CommonException(GATEWAY_ORDER_NOT_EXISTS, Resources.EMGatewayOrderNotExist);

            if (!orderDetail.UserAccountId.HasValue)
            {
                throw new CommonException(GATEWAY_ORDER_NOT_EXISTS, Resources.EMGatewayOrderNotExist);
            }

            var id = Guid.NewGuid();
            ExcutePay(orderDetail, id);
            RabbitMQSender.SendMessage("FiiiPay_Gateway_PayCompleted", tradeId);

            return new GatewayOrderInfoOM()
            {
                Timestamp = DateTime.UtcNow.ToUnixTime().ToString(),
                OrderId = orderDetail.Id,
                MerchantName = orderDetail.MerchantName,
                CryptoCode = orderDetail.Crypto,
                ActurlCryptoAmount = orderDetail.ActualCryptoAmount
            };
        }

        private GatewayOrder ExcutePay(GatewayOrderOM orderDetail, Guid id)
        {
            var cryptoCurrency = new CryptocurrencyDAC().GetByCode(orderDetail.Crypto);
            if (cryptoCurrency == null || cryptoCurrency?.Id == null)
                throw new CommonException(CRYPTO_NOT_EXISTS, "Error: Invalid Cryptocurrency");
            //var uwComponent = new UserWalletComponent();
            var userWallet = new UserWalletDAC().GetUserWallet(orderDetail.UserAccountId.Value, cryptoCurrency.Id);
            if (userWallet.Balance < orderDetail.ActualCryptoAmount)
                throw new CommonException(INSUFFICIENT_BALANCE, Resources.余额不足);
            var uwDAC = new UserWalletDAC();
            var uwsDAC = new UserWalletStatementDAC();
            var goDAC = new GatewayOrderDAC();

            bool needAddOrder = true;
            var gatewayOrder = goDAC.GetByTradeNo(orderDetail.Id.ToString());
            if (gatewayOrder != null)
            {
                if (gatewayOrder.Status != GatewayOrderStatus.Pending)
                {
                    throw new CommonException(ORDER_HAD_COMPLETE, Resources.订单已完成或者已退款);
                }
                needAddOrder = false;
            }
            else
            {
                gatewayOrder = new Entities.GatewayOrder()
                {
                    OrderNo = CreateOrderno(),
                    TradeNo = orderDetail.Id.ToString(),
                    Id = id,
                    CryptoId = cryptoCurrency.Id,
                    MerchantAccountId = orderDetail.MerchantAccountId,
                    FiatAmount = orderDetail.FiatAmount,
                    MerchantName = orderDetail.MerchantName,
                    UserAccountId = orderDetail.UserAccountId,
                    CryptoAmount = orderDetail.CryptoAmount,
                    ActualCryptoAmount = orderDetail.ActualCryptoAmount,
                    FiatCurrency = orderDetail.FiatCurrency,
                    Markup = orderDetail.MarkupRate,
                    ActualFiatAmount = orderDetail.ActualFiatAmount,
                    Status = GatewayOrderStatus.Completed,
                    ExchangeRate = orderDetail.ExchangeRate,
                    ExpiredTime = orderDetail.ExpiredTime,
                    TransactionFee = orderDetail.TransactionFee,
                    Timestamp = DateTime.UtcNow,
                    PaymentTime = DateTime.UtcNow,
                    Remark = orderDetail.Remark
                };
            }

            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                if (needAddOrder)
                    goDAC.Insert(gatewayOrder);
                uwDAC.Decrease(userWallet.Id, orderDetail.ActualCryptoAmount);
                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = userWallet.Id,
                    Action = UserWalletStatementAction.TansferOut,
                    Amount = -orderDetail.ActualCryptoAmount,
                    Balance = userWallet.Balance - orderDetail.ActualCryptoAmount,
                    FrozenAmount = 0,
                    FrozenBalance = userWallet.FrozenBalance,
                    Timestamp = DateTime.UtcNow
                });

                scope.Complete();
            }

            return gatewayOrder;
        }

        private GatewayOrderOM GetGatewayOrderDetail(string id)
        {
            var json = GetToGateway("/api/Order/GetOrderDetail?orderId=" + id);
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

            var userWallet = new UserWalletDAC().GetUserWallet(orderDetail.UserAccountId.Value, orderDetail.CryptoId);
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
                rgoDAC.Insert(new GatewayRefundOrder()
                {
                    OrderId = orderDetail.Id,
                    Timestamp = DateTime.UtcNow,
                    Status = RefundStatus.Completed
                });

                scope.Complete();
            }
        }

        private string CreateOrderno()
        {
            return DateTime.Now.ToUnixTime() + new Random().Next(0, 100).ToString().PadLeft(2, '0');
        }

        public static string GetToGateway(string apiName)
        {
            string baseUrl = ConfigurationManager.AppSettings["Gateway_URL"];
            string clientKey = ConfigurationManager.AppSettings["GatewayClientKey"];
            string secretKey = ConfigurationManager.AppSettings["GatewaySecretKey"];

            string strHttpUrl = $"{baseUrl.TrimEnd('/')}/{apiName.TrimStart('/')}";

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Authorization", "bearer " + GenerateToken(clientKey, secretKey));

            string result = RestUtilities.GetJson(strHttpUrl, header);
            return result;
        }

        public string PostToGateway(string apiName, Dictionary<string, string> dicParams)
        {
            string baseUrl = ConfigurationManager.AppSettings["Gateway_URL"];
            string clientKey = ConfigurationManager.AppSettings["GatewayClientKey"];
            string secretKey = ConfigurationManager.AppSettings["GatewaySecretKey"];

            string strParams = JsonConvert.SerializeObject(dicParams);
            string strHttpUrl = $"{baseUrl.TrimEnd('/')}/{apiName.TrimStart('/')}";

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-Type", "application/json");
            header.Add("Authorization", "bearer " + GenerateToken(clientKey, secretKey));

            string result = RestUtilities.PostJson(strHttpUrl, header, strParams);
            return result;
        }

        private static string GenerateToken(string clientKey, string secretKey)
        {
            string password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + clientKey;
            string token = AES128.Encrypt(password, secretKey);

            return token;
        }
    }
}
