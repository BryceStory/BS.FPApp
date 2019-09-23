using System;

namespace FiiiPay.Entities
{
    public class MallPaymentOrder
    {
        public Guid Id { get; set; }

        public string OrderId { get; set; }

        public string TradeNo { get; set; }

        public Guid UserAccountId { get; set; }

        public decimal CryptoAmount { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime ExpiredTime { get; set; }

        public DateTime Timestamp { get; set; }

        public string RefundTradeNo { get; set; }

        public bool HasNotification { get; set; }

        public bool RefundHasNotification { get; set; }

        public string NotificationSource { get; set; }

        public string Remark { get; set; }
    }
}
