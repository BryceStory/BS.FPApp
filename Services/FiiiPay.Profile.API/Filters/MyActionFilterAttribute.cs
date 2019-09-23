
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FiiiPay.Profile.API.Filters
{
    public class MyActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var errors = actionContext.ModelState.Values.FirstOrDefault()?.Errors;
            if (errors != null && errors.Count > 0)
            {
                var errorMessage = errors.FirstOrDefault().ErrorMessage;
                errorMessage = string.IsNullOrEmpty(errorMessage) ? "不合法的数据格式" : errorMessage;
                throw new ApplicationException(errorMessage);
            }
        }
    }
}