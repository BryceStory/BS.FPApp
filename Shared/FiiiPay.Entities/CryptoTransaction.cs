using System;

namespace FiiiPay.Entities
{
    public class CryptoTransaction
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public byte AccountType { get; set; }
        public byte TransactionType { get; set; }
        public int CryptoId { get; set; }
        public decimal Amount { get; set; }
        public string CryptoAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}
