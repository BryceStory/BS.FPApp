using System;

namespace FiiiPay.Foundation.Entities
{
    public class PriceInfo
    {
        public short ID { get; set; }
        
        public int CryptoID { get; set; }

        public short CurrencyID { get; set; }

        public decimal Price { get; set; }

        public decimal MarketPrice { get; set; }

        public DateTime LastUpdateDate { get; set; }
    }
}
