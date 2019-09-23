using FiiiPay.Business;
using FiiiPay.Framework;
using System.Web.Http;
using FiiiPay.API.Models;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.FeedbackController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Feedback")]
    public class FeedbackController : ApiController
    {
        /// <summary>
        /// 意见反馈
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Feedback")]
        public ServiceResult<bool> Feedback(FeedbackIM im)
        {
            new FeedbackComponent().Feedback(this.GetUser().Id, im.Content, "FiiiPay " + (this.IsAndroid() ? "Android" : "Ios"));
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }
    }
}
