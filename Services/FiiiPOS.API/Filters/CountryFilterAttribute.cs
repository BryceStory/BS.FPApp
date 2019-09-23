using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Properties;
using FiiiPay.Foundation.Entities.Enum;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.API.Extensions;
using FiiiPOS.Business.FiiiPOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FiiiPOS.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CountryFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var isAuth = actionContext.RequestContext.Principal.Identity.IsAuthenticated;
            if (!isAuth)
                return;

            var identity = (WebIdentity)actionContext.RequestContext.Principal.Identity;
            WebContext context = identity.WebContext;
            var accountId = context.Id;

            var account = new MerchantAccountComponent().GetById(accountId);
            if(account==null)
                throw new ApplicationException(Resources.SystemError);

            var country = new CountryComponent().GetById(account.CountryId);
            if (country == null || !country.Status.HasFlag(CountryStatus.Enable))
            {
                throw new CommonException(ReasonCode.COUNTRY_FORBBIDEN_REQUEST, Resources.SystemError);
            }
        }
    }
}