using System.Web.Http;
using FiiiPay.Foundation.API.Filters;

namespace FiiiPay.Foundation.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new TokenBasedAuthenticationFilter("Bearer"));
            config.Filters.Add(new WebApiExceptionFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
