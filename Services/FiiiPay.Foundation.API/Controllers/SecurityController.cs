using System.Web.Http;

namespace FiiiPay.Foundation.API.Controllers
{
    [RoutePrefix("Security")]
    [Authorize]
    public class SecurityController : ApiController
    {
        [HttpGet]
        [Route("Check")]
        public string Check()
        {
            return "OK";
        }
    }
}
