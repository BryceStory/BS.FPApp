using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FiiiPay.Framework;

namespace FiiiPay.API.Filters
{
    /// <summary>
    /// Class FiiiPay.API.Filters.MyActionFilterAttribute
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    public class MyActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var result = new ServiceResult<string>
                {
                    Data = null, Code = ReasonCode.MISSING_REQUIRED_FIELDS
                };
                foreach (var error in actionContext.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + System.Environment.NewLine;

                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult>(result, new System.Net.Http.Formatting.JsonMediaTypeFormatter())
                };
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}