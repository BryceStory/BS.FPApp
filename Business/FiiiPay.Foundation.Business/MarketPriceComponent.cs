using System.Collections.Generic;
using System.Linq;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;

namespace FiiiPay.Foundation.Business
{
    public class MarketPriceComponent
    {
        public MarketPriceComponent()
        {
            
        }

        public List<MarketPriceInfo> GetMarketPrice(string currency)
        {
            var dac = new PriceInfoDAC();

            var list = dac.GetPrice(currency).Select(s => new MarketPriceInfo { CryptoName = s.CryptoName, Currency = s.Currency, Price = s.Price }).ToList();

            return list;
        }

        public MarketPriceInfo GetMarketPrice(string currency, string cryptoName)
        {
            var dac = new PriceInfoDAC();
            var price = dac.GetPrice(currency, cryptoName).Select(s =>
            {
                if (s != null)
                    return new MarketPriceInfo
                    {
                        CryptoName = s.CryptoName,
                        Currency = s.Currency,
                        Price = s.Price
                    };
                return null;
            }).FirstOrDefault() ?? new MarketPriceInfo
            {
                CryptoName = cryptoName,
                Currency = currency,
                MarkupPrice = 0.0001M,
                Price = 0.0001M
            };

            return price;
        }
    }
}
