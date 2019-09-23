using System.Web.Http;

namespace FiiiPay.Profile.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.Profile.API.Controllers.SecurityController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Security")]
    [Authorize]
    public class SecurityController : ApiController
    {
        /// <summary>
        /// Checks this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Check")]
        public string Check()
        {
            return "OK";
        }
    }
}
