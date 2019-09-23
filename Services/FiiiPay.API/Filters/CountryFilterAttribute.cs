using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FiiiPay.Business.Properties;

namespace FiiiPay.API.Filters
{
    /// <summary>
    /// 用户所在国家(区域)状态过滤
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CountryFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(!actionContext.Request.Properties.TryGetValue("User", out object user))
                return;
            var account = user as UserAccount;
            if (account == null)
                throw new ApplicationException(MessageResources.NetworkError);
            
            var country = new Foundation.Business.CountryComponent().GetById(account.CountryId);
            if (country == null || !country.Status.HasFlag(Foundation.Entities.Enum.CountryStatus.Enable))
            {
                throw new CommonException(ReasonCode.COUNTRY_FORBBIDEN_REQUEST, MessageResources.CountryRequestForbbiden);
            }
        }
    }
}