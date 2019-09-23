using System.Web.Http;
using FiiiPay.API.Filters;

namespace FiiiPay.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.Filters.Add(new TokenBasedAuthenticationFilter("Bearer"));
            config.Filters.Add(new WebApiExceptionFilterAttribute());
            config.Filters.Add(new CountryFilterAttribute());
            config.Filters.Add(new MyActionFilterAttribute());
            config.Filters.Add(new BillerForbiddenAttribute());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
        }
    }
}
