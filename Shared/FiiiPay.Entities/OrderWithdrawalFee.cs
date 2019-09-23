using System;

namespace FiiiPay.Entities
{
    public class OrderWithdrawalFee
    {
        public long Id { get; set; }

        public int CryptoId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; }
        
        public Guid OrderId { get; set; }
    }
}
