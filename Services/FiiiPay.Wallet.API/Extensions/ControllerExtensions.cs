using System.Web;
using System.Web.Http;

namespace FiiiPay.Wallet.API.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControllerExtensions
    {


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
        
    }
}