using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Framework.Component.Exceptions;

namespace FiiiPay.Business
{
    public class CryptocurrencyAgent
    {
        /// <summary>
        /// 获取单个加密币兑法币的汇率
        /// </summary>
        /// <param name="countryId">调用者国家ID</param>
        /// <param name="fiatCurrency">法币代码</param>
        /// <param name="cryptoCode">加密币代码</param>
        /// <returns></returns>
        [Obsolete]
        public MarketPriceInfo GetMarketPrice(int countryId, string fiatCurrency, string cryptoCode)
        {
            string cryptoPriceKey = "Setting:CryptoCurrency:MarketPrice" + fiatCurrency;

            if (RedisHelper.KeyExists(cryptoPriceKey))
            {
                List<MarketPriceInfo> infoList = JsonConvert.DeserializeObject<List<MarketPriceInfo>>(RedisHelper.StringGet(cryptoPriceKey));
                var priceInfo = infoList.FirstOrDefault(t => t.CryptoName == cryptoCode);

                if (priceInfo == null)
                {
                    var agent = new MarketPriceComponent();
                    return agent.GetMarketPrice(fiatCurrency, cryptoCode);
                }

                return priceInfo;
            }

            var allPriceInfo = GetAllMarketPrice(countryId, fiatCurrency);
            return allPriceInfo.First(t => t.CryptoName == cryptoCode);
        }

        /// <summary>
        /// 获取所有加密币兑法币的汇率
        /// </summary>
        /// <param name="countryId">调用者国家ID</param>
        /// <param name="fiatCurrency">法币代码</param>
        /// <returns></returns>
        [Obsolete]
        public List<MarketPriceInfo> GetAllMarketPrice(int countryId, string fiatCurrency)
        {
            string cryptoPriceKey = "Setting:CryptoCurrency:MarketPrice" + fiatCurrency;

            if (RedisHelper.KeyExists(cryptoPriceKey))
            {
                List<MarketPriceInfo> infoList = JsonConvert.DeserializeObject<List<MarketPriceInfo>>(RedisHelper.StringGet(cryptoPriceKey));
                return infoList;
            }

            var agent = new MarketPriceComponent();
            var result = agent.GetMarketPrice(fiatCurrency);

            if (result == null)
                throw new FiiiFinanceException(10000, "");

            RedisHelper.StringSet(cryptoPriceKey, JsonConvert.SerializeObject(result), new TimeSpan(0, Constant.CACHE_CRYPTO_PRICE_TIME, 0));

            return result;
        }
    }
}
