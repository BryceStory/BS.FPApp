using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FiiiPay.Business.FiiiPay;
using FiiiPay.Business.Properties;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.API.Filters
{
    public class BillerForbiddenAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.ControllerDescriptor.GetFilters()
                .Any(item => item is BillerForbiddenAttribute))
            {
                if (!new BillerComponent().IsBillerForbbiden())
                {
                    throw new CommonException(ReasonCode.BillerForbidden, Resources.BillerForbidden);
                }
            }
            
        }
    }
}