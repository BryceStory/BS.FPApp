using System;

namespace FiiiPay.Entities
{
    public class RedPocketRefund
    {
        public long Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string OrderNo { get; set; }

        public decimal Amount { get; set; }
    }
}
