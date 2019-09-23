using FiiiPay.Framework;
using FiiiPay.ShopPayment.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FiiiPay.ShopPayment.API.Filters
{
    /// <summary>
    /// 参数验证
    /// </summary>
    public class ParamValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}