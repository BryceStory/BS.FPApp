using FiiiPay.Framework;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FiiiPay.CryptoCurrency.API.Filters
{
    /// <summary>
    /// 参数过滤器
    /// </summary>
    public class ParameterValidationFilter : ActionFilterAttribute
    {
        public const int MISSING_REQUIRED_FIELDS = 10001;

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                ServiceResult<string> result = new ServiceResult<string>();
                result.Data = null;
                result.Code = MISSING_REQUIRED_FIELDS;
                foreach (string error in actionContext.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
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