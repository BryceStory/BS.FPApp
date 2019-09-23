using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.Profile.API.Filters;

namespace FiiiPay.Profile.API
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
