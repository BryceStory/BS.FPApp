using System;

namespace FiiiPay.Entities
{
    public class OrderStatusTracking
    {
        public long Id { get; set; }
        public Guid OrderId { get; set; }
        public byte OriginalStatus { get; set; }
        public byte Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}
