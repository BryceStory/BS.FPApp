using FiiiPay.ShopPayment.API.Filters;
using System.Web.Http;

namespace FiiiPay.ShopPayment.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new ParamValidationAttribute());
            config.Filters.Add(new WebApiExceptionFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
        }
    }
}
