using System.Web.Http;
using FiiiPOS.Web.API.Base;

namespace FiiiPOS.Web.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.Filters.Add(new TokenAuthenticationFilter("Bearer"));
            config.Filters.Add(new APIExceptionFilterAttribute());
            
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
