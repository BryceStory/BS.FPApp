using System;

namespace FiiiPay.Entities
{
    public class Refund
    {
        public long Id { get; set; }
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public RefundStatus Status { get; set; }
        public string Remark { get; set; }
    }

    public enum RefundStatus
    {
        Completed
    }
}
