using System;

namespace FiiiPay.Entities
{
    public class RedPocket
    {
        public long Id { get; set; }

        public Guid AccountId { get; set; }

        public RedPocketStatus Status { get; set; }

        public string PassCode { get; set; }

        public int CryptoId { get; set; }

        public string CryptoCode { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public int Count { get; set; }

        public int RemainCount { get; set; }

        public string Message { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTime Timestamp { get; set; }

        public string OrderNo { get; set; }

        public decimal FiatAmount { get; set; }
    }

    public enum RedPocketStatus : byte
    {
        Actived = 1,
        Complate,
        Refund,
        FullRefund
    }
}
