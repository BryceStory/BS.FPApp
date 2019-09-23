using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FiiiPay.Framework.Component;

namespace FiiiPay.API
{
    /// <summary>
    /// Class FiiiPay.API.WebApiApplication
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Applications the start.
        /// </summary>
        protected void Application_Start()
        {
#if DEBUG
            AreaRegistration.RegisterAllAreas();
#endif

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
#if DEBUG
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
#endif
            Bootstarp.Start();
        }
    }
}
