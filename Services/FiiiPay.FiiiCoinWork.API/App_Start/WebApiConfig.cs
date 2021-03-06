﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.FiiiCoinWork.API.Filters;

namespace FiiiPay.FiiiCoinWork.API
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
