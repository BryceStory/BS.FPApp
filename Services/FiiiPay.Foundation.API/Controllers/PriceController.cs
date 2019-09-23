using System.Web.Http;
using FiiiPay.Foundation.Business;

namespace FiiiPay.Foundation.API.Controllers
{
    [RoutePrefix("Price")]
    public class PriceController : ApiController
    {
        /// <summary>
        /// Updates the market price to database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("UpdateMarketPriceToDB")]
        [AllowAnonymous]
        public string UpdateMarketPriceToDB()
        {
            var priceinfocomponent = new PriceInfoComponent();
            var currencyconverterlist = priceinfocomponent.CurrencyConvertList();

            priceinfocomponent.GetPriceInfo(currencyconverterlist);
            priceinfocomponent.GetCMBPriceInfo(currencyconverterlist);
            return "Success";
        }

        [HttpGet]
        [Route("GetPrice")]
        [AllowAnonymous]
        public decimal GetPrice(string currency, string crypto)
        {
            return new PriceInfoComponent().GetPrice(currency, crypto);
        }
    }
}
