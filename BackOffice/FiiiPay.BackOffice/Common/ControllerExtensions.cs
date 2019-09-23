using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Common
{
    public static class ControllerExtensions
    {
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