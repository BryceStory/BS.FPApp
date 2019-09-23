using System.Web.Mvc;

namespace FiiiPay.ShopPayment.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "Home Page";

            return RedirectToAction("Index", "Help");
        }
    }
}
