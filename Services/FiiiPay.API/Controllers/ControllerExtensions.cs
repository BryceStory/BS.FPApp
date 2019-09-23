using System;
using FiiiPay.Entities;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    public static class ControllerExtensions
    {
        public static UserAccount GetUser(this ApiController controller)
        {
            if (controller.Request.Properties.TryGetValue("User", out object user))
            {
                return user as UserAccount;
            }
            return null;
        }

        public static bool IsAndroid(this ApiController controller)
        {
            var ua = controller.Request.Headers.UserAgent.ToString().ToLower();

            //安卓使用了这个网络请求框架
            return ua.Contains("okhttp");
        }

        public static string GetClientIPAddress(this ApiController controller)
        {
            HttpRequest request = HttpContext.Current.Request;
            
            string userHostAddress = request.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(userHostAddress))
                userHostAddress = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(userHostAddress) && request.ServerVariables["HTTP_VIA"] != null)
                userHostAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0].Trim();

            if (string.IsNullOrEmpty(userHostAddress))
                userHostAddress = HttpContext.Current.Request.UserHostAddress;

            return !string.IsNullOrEmpty(userHostAddress) ? userHostAddress : "127.0.0.1";
        }

        public static string GetDeviceNumber(this ApiController controller)
        {
            if (controller.Request.Headers.Contains("DeviceNumber"))
            {
                var version = controller.Request.Headers.GetValues("DeviceNumber").FirstOrDefault();
                return version;
            }
            return null;
        }

        public static string AppVersion(this ApiController controller)
        {
            if (controller.Request.Headers.Contains("AppVersion"))
            {
                var version = controller.Request.Headers.GetValues("AppVersion").FirstOrDefault();
                return version;
            }
            return null;
        }

        public static Guid? UserId(this ApiController controller)
        {
            if (controller.Request.Properties.TryGetValue("User", out var user))
            {
                return (user as UserAccount).Id;
            }

            return null;
        }

        public static string Language(this ApiController controller)
        {
            var firstLng = controller.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            if (string.IsNullOrWhiteSpace(firstLng)) return "en-US";

            if (firstLng.Contains("en")) return "en-US";
            if (firstLng.Contains("zh")) return "zh-CN";

            return "en-US";
        }

        public static bool IsZH(this ApiController controller)
        {
            var firstLng = controller.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            return !string.IsNullOrEmpty(firstLng) && firstLng.Contains("zh");
        }
    }
}