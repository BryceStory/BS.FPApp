using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Exceptionless;
using FiiiPay.Framework.Component;

namespace FiiiPOS.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
#if DEBUG
            AreaRegistration.RegisterAllAreas();
#endif

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(ExceptionlessClient.Default.RegisterWebApi);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
#if DEBUG
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
#endif
            Bootstarp.Start();
        }
    }
}
