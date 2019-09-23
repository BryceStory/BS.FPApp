using System;
using System.Collections.Generic;
using System.Configuration;
using System.Timers;
using System.Transactions;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Queue;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.MallPayment
{
    internal class PaymentProcess
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(PaymentProcess));
        private readonly string _notificationUrl = ConfigurationManager.AppSettings.Get("NotificationUrl");

        private readonly int _span = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("NotificationSpan"))
            ? 180000
            : int.Parse(ConfigurationManager.AppSettings.Get("NotificationSpan"));

        private readonly object _lock = new object();
        //private readonly Dictionary<string, int> _notificationErrorCount = new Dictionary<string, int>();

        public void Payment(string message)
        {
            var orderModel = JsonConvert.DeserializeObject<MallPaymentOrder>(message);

            var userWalletDac = new UserWalletDAC();
            var walletStatementDac = new UserWalletStatementDAC();
            var gatewayOrderDac = new GatewayOrderDAC();
            var mallDac = new MallPaymentOrderDAC();
            var cryptoDac = new CryptocurrencyDAC();

            var extsis = gatewayOrderDac.GetByOrderNo(orderModel.OrderId);
            if (extsis != null && extsis.Status == GatewayOrderStatus.Completed)
            {
                _log.Info("Order message " + message + " hased Payment.");
                return;
            }

            var cryptoFiii = cryptoDac.GetByCode("FIII");
            var fiiiWallet = userWalletDac.GetByAccountId(orderModel.UserAccountId, cryptoFiii.Id);

            if (fiiiWallet.Balance < orderModel.CryptoAmount)
            {
                _log.ErrorFormat("message {0}, Insufficient balance", message);
            }

            try
            {
                var fiiiPrice = GetMarketPrice("USD", "FIII");
                var fiiiFiatAmount = Math.Round(orderModel.CryptoAmount * fiiiPrice, 2);

                var trdeNo = NumberGenerator.GenerateUnixOrderNo();
                var id = Guid.NewGuid();
                using (var scope = new TransactionScope())
                {
                    userWalletDac.Decrease(fiiiWallet.Id, orderModel.CryptoAmount);
                    walletStatementDac.Insert(new UserWalletStatement
                    {
                        WalletId = fiiiWallet.Id,
                        Action = UserWalletStatementAction.Consume,
                        Amount = -orderModel.CryptoAmount,
                        Balance = fiiiWallet.Balance - orderModel.CryptoAmount,
                        FrozenAmount = 0,
                        FrozenBalance = fiiiWallet.FrozenBalance,
                        Timestamp = DateTime.UtcNow
                    });

                    var gatewayFiiiOrder = new GatewayOrder
                    {
                        Id = id,
                        OrderNo = orderModel.OrderId,
                        MerchantAccountId = Guid.Empty,
                        MerchantName = "FiiiShop",
                        CryptoId = cryptoFiii.Id,
                        CryptoCode = "FIII",
                        FiatAmount = fiiiFiatAmount,
                        FiatCurrency = "USD",
                        Status = GatewayOrderStatus.Completed,
                        ExpiredTime = DateTime.UtcNow.AddMinutes(30),
                        Markup = 0M,
                        ExchangeRate = fiiiPrice,
                        PaymentTime = DateTime.UtcNow,
                        Timestamp = DateTime.UtcNow,
                        UserAccountId = orderModel.UserAccountId,
                        ActualCryptoAmount = orderModel.CryptoAmount,
                        ActualFiatAmount = fiiiFiatAmount,
                        CryptoAmount = orderModel.CryptoAmount,
                        TransactionFee = 0,
                        Remark = null,
                        TradeNo = trdeNo
                    };
                    gatewayOrderDac.Insert(gatewayFiiiOrder);

                    mallDac.UpdateStatus(orderModel.Id, (byte)OrderStatus.Completed);
                    mallDac.UpdateTradeNo(orderModel.Id, trdeNo);

                    scope.Complete();
                }

                RabbitMQSender.SendMessage("ShopPayment", id);
                SendNotificationMessage(TradeType.Payment, orderModel.Id, orderModel.OrderId, trdeNo, "success");
            }
            catch (Exception exception)
            {
                //SendNotificationMessage(TradeType.Payment, orderModel.Id, orderModel.OrderId, string.Empty, "error");
                _log.Error("Payment error, exception : " + exception.Message);
            }
        }

        public void Refund(string message)
        {
            var orderModel = JsonConvert.DeserializeObject<MallPaymentOrder>(message);

            var userWalletDac = new UserWalletDAC();
            var walletStatementDac = new UserWalletStatementDAC();
            var gatewayOrderDac = new GatewayOrderDAC();
            var mallDac = new MallPaymentOrderDAC();
            var refundDac = new GatewayRefundOrderDAC();

            var gatewayOrder = gatewayOrderDac.GetByTradeNo(orderModel.TradeNo);
            if (gatewayOrder.Status == GatewayOrderStatus.Pending)
            {
                _log.Error("Order message " + message + " not payment.");
                return;
            }
            if (gatewayOrder.Status == GatewayOrderStatus.Refunded)
            {
                _log.Info("Order message " + message + " has refund.");
                return;
            }

            var fiiiWallet = userWalletDac.GetByCryptoCode(orderModel.UserAccountId, "FIII");

            try
            {
                var id = Guid.NewGuid();
                var refundTradeNo = NumberGenerator.GenerateUnixOrderNo();
                using (var scope = new TransactionScope())
                {
                    mallDac.UpdateStatus(orderModel.Id, (byte)OrderStatus.Refunded);
                    mallDac.UpdateRefundTradeNo(orderModel.Id, refundTradeNo);

                    gatewayOrderDac.UpdateStatus(gatewayOrder.Id, (byte)OrderStatus.Refunded);

                    userWalletDac.Increase(fiiiWallet.Id, gatewayOrder.CryptoAmount);
                    walletStatementDac.Insert(new UserWalletStatement
                    {
                        WalletId = fiiiWallet.Id,
                        Action = UserWalletStatementAction.Refund,
                        Amount = orderModel.CryptoAmount,
                        Balance = fiiiWallet.Balance + gatewayOrder.CryptoAmount,
                        FrozenAmount = 0,
                        FrozenBalance = fiiiWallet.FrozenBalance,
                        Timestamp = DateTime.UtcNow
                    });

                    refundDac.Insert(new GatewayRefundOrder
                    {
                        Id = id,
                        OrderId = gatewayOrder.Id,
                        Remark = "",
                        Status = RefundStatus.Completed,
                        Timestamp = DateTime.UtcNow,
                        RefundTradeNo = refundTradeNo
                    });

                    scope.Complete();
                }

                RabbitMQSender.SendMessage("ShopPaymentRefund", id);
                SendNotificationMessage(TradeType.Refund, orderModel.Id, orderModel.OrderId, refundTradeNo, "success");
            }
            catch (Exception exception)
            {
                _log.Error("Refund error, exception : " + exception.Message);

                //SendNotificationMessage(TradeType.Refund, orderModel.Id, orderModel.OrderId, string.Empty, "error");
            }
        }

        public void ReNotification()
        {
            var time = new Timer(_span);
            time.Elapsed += TimeOnElapsed;
            time.Start();
        }

        private void TimeOnElapsed(object sender, ElapsedEventArgs e)
        {
            var mallDac = new MallPaymentOrderDAC();
            var payment = mallDac.GetNotificationError();
            var refund = mallDac.GetRefundNotifitaionError();

            foreach (var mallPaymentOrder in payment)
            {
                var model = new NotificationModel
                {
                    MallId = mallPaymentOrder.Id,
                    OrderId = mallPaymentOrder.OrderId,
                    Status = "success",
                    TradeNo = mallPaymentOrder.TradeNo,
                    Type = TradeType.Payment
                };
                Notification(JsonConvert.SerializeObject(model), true);
            }

            foreach (var mallPaymentOrder in refund)
            {
                var model = new NotificationModel
                {
                    MallId = mallPaymentOrder.Id,
                    OrderId = mallPaymentOrder.OrderId,
                    Status = "success",
                    TradeNo = mallPaymentOrder.RefundTradeNo,
                    Type = TradeType.Refund
                };
                Notification(JsonConvert.SerializeObject(model), true);
            }
        }

        public void Notification(string message, bool reNotification = false)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(_notificationUrl))
                {
                    _log.Error("Invalid Noification URL");
                    return;
                }

                var notificationModel = JsonConvert.DeserializeObject<NotificationModel>(message);

                var mallDac = new MallPaymentOrderDAC();
                if (notificationModel.Type == TradeType.Payment)
                {
                    var status = mallDac.HasNotifiation(notificationModel.MallId);

                    if (status) return;
                }
                else
                {
                    var status = mallDac.RefundHasNotifiation(notificationModel.MallId);

                    if (status) return;
                }

                var sendNotification = JsonConvert.SerializeObject(new
                {
                    notificationModel.OrderId,
                    Type = Enum.GetName(typeof(TradeType), notificationModel.Type),
                    notificationModel.TradeNo,
                    notificationModel.Status
                });

                var result = "";
                try
                {
                    result = RestUtilities.PostJson(_notificationUrl,
                        new Dictionary<string, string>
                            {{"FiiiPay", ConfigurationManager.AppSettings.Get("CallbackKey")}}, sendNotification);
                    _log.InfoFormat("Notification {0} params {1},result {2}", notificationModel.OrderId,
                        sendNotification, result);
                }
                catch (Exception exception)
                {
                    _log.ErrorFormat("Notification error {0}, param {1}", exception.Message, message);

                    mallDac.UpdateNotificationSource(notificationModel.MallId, message);
                }

                if (result.StartsWith("ok"))
                {
                    if (notificationModel.Type == TradeType.Payment)
                    {
                        mallDac.UpdateNotification(notificationModel.MallId);
                    }
                    else if (notificationModel.Type == TradeType.Refund)
                    {
                        mallDac.UpdateRefundNotification(notificationModel.MallId);
                    }
                }
                else
                {
                    if (!reNotification)
                        mallDac.UpdateNotificationSource(notificationModel.MallId, message);
                }
            }
        }

        private static decimal GetMarketPrice(string currency, string cryptoName)
        {
            var dac = new PriceInfoDAC();
            return dac.GetPriceByName(currency, cryptoName);
        }

        private static void SendNotificationMessage(TradeType type, Guid mallId, string orderId, string tradeNo, string result)
        {
            RabbitMQSender.SendMessage("PaymentNotification", new NotificationModel
            {
                Type = type,
                OrderId = orderId,
                MallId = mallId,
                TradeNo = tradeNo,
                Status = result
            });
        }
    }

    internal class NotificationModel
    {
        public TradeType Type { get; set; }

        public Guid MallId { get; set; }

        /// <summary>
        /// FiiiShop Order Id
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public string OrderId { get; set; }

        public string TradeNo { get; set; }

        public string Status { get; set; }
    }

    internal enum TradeType : byte
    {
        Payment = 1,
        Refund
    }
}
