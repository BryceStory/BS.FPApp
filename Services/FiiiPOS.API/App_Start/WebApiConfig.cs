using System.Web.Http;
using FiiiPOS.API.Filters;

namespace FiiiPOS.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.Filters.Add(new TokenBasedAuthenticationFilter());
            config.Filters.Add(new WebApiExceptionFilterAttribute());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
