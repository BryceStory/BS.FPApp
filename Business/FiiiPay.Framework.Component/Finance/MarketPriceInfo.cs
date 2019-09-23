namespace FiiiPay.Foundation.Business.Agent
{
    public class MarketPriceInfo
    {
        public string Currency { get; set; }
        public string CryptoName { get; set; }
        public decimal Price { get; set; }

        public decimal MarkupPrice { get; set; }
    }
}