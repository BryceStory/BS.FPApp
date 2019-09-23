using System.Linq;
using System.Web.Http;

namespace FiiiPay.Foundation.API.Controllers
{
    public abstract class BaseFoundationController : ApiController
    {
        protected bool IsZH()
        {
            var firstLng = Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            return !string.IsNullOrEmpty(firstLng) && firstLng.Contains("zh");
        }
    }
}