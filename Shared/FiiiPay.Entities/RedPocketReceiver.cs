using System;

namespace FiiiPay.Entities
{
    public class RedPocketReceiver
    {
        public long Id { get; set; }

        public long PocketId { get; set; }

        public Guid SendAccountId { get; set; }

        public Guid AccountId { get; set; }

        public string CryptoCode { get; set; }

        public decimal Amount { get; set; }

        public bool IsBestLuck { get; set; }

        public DateTime Timestamp { get; set; }

        public string OrderNo { get; set; }

        public decimal FiatAmount { get; set; }
    }
}
