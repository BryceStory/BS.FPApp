using System.Linq;
using System.Web;
using System.Web.Http;
using FiiiPay.Entities;

namespace FiiiPay.FiiiCoinWork.API.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 获取用户Id
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static int GetAccountId(this ApiController controller)
        {
            return GetUser(controller).Id;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static InvestorAccount GetUser(this ApiController controller)
        {
            UserIdentity identity = (UserIdentity)controller.User.Identity;
            return identity.User;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 是否中文
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static bool IsZH(this ApiController controller)
        {
            var firstLng = controller.Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            return !string.IsNullOrEmpty(firstLng) && firstLng.Contains("zh");
        }
    }
}