using System;
using System.Linq;
using System.Web;
using System.Web.Http;
#pragma warning disable 1591

namespace FiiiPOS.API.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 获取商户Id
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Guid GetMerchantAccountId(this ApiController controller)
        {
            if (controller.User.Identity == null)
                throw new ArgumentNullException("identity");

            var identity = (WebIdentity)controller.User.Identity;

            WebContext context = identity.WebContext;

            return context.Id;
        }

        public static int GetMerchantCountId(this ApiController controller)
        {
            if (controller.User.Identity == null)
                throw new ArgumentNullException("identity");

            var identity = (WebIdentity)controller.User.Identity;

            WebContext context = identity.WebContext;

            return context.CountrtId;
        }


        // ReSharper disable once InconsistentNaming
        public static string GetClientIPAddress(this ApiController controller)
        {
            HttpRequest request = HttpContext.Current.Request;
            string userHostAddress = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(userHostAddress) && request.ServerVariables["HTTP_VIA"] != null)
                userHostAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0].Trim();

            if (string.IsNullOrEmpty(userHostAddress))
                userHostAddress = HttpContext.Current.Request.UserHostAddress;

            return !string.IsNullOrEmpty(userHostAddress) ? userHostAddress : "127.0.0.1";
        }

        public static bool IsZH(this ApiController controller)
        {
            var firstLng = controller.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            return !string.IsNullOrEmpty(firstLng) && firstLng.Contains("zh");
        }
    }
}