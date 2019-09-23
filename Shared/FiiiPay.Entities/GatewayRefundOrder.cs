using System;

namespace FiiiPay.Entities
{
    public class GatewayRefundOrder
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public string RefundTradeNo { get; set; }

        public DateTime Timestamp { get; set; }
        public RefundStatus Status { get; set; }
        public string Remark { get; set; }
    }
}
