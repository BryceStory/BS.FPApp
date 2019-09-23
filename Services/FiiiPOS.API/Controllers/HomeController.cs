using System.Web.Mvc;
using FiiiPOS.Business.FiiiPOS;

namespace FiiiPOS.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public string MarketPrice(string fiatcurrency, int countryId = 9)
        {
            return new MerchantWalletComponent().GetMarketPriceString(countryId, fiatcurrency.ToUpper());
        }
    }
}
