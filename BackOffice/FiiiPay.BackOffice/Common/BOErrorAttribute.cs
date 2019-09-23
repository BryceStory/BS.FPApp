using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Common
{
    public class BOErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            log4net.ILog log = log4net.LogManager.GetLogger(typeof(BOErrorAttribute));
            log.Error(filterContext.Exception);

            ErrorMessage msg = new ErrorMessage(filterContext.Exception, "Web Page");
            msg.ShowException = ApplicationException.IsExceptionEnabled();

            //设置为true阻止golbal里面的错误执行
            filterContext.ExceptionHandled = true;
            var rvd = new System.Web.Routing.RouteValueDictionary();
            filterContext.Result = new ViewResult() { ViewName = "/Views/Shared/Error.cshtml", ViewData = new ViewDataDictionary<ErrorMessage>(msg) };

        }
    }

    public class ErrorMessage
    {
        public ErrorMessage()
        {

        }
        public ErrorMessage(Exception ex, string type)
        {
            MsgType = ex.GetType().Name;
            Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            StackTrace = ex.StackTrace;
            Source = ex.Source;
            Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            Assembly = ex.TargetSite.Module.Assembly.FullName;
            Method = ex.TargetSite.Name;
            Type = type;

            DotNetVersion = Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision;
            DotNetBit = (Environment.Is64BitProcess ? "64" : "32") + "位";
            OSVersion = Environment.OSVersion.ToString();
            CPUCount = Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
            CPUType = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            OSBit = (Environment.Is64BitOperatingSystem ? "64" : "32") + "位";

            var request = HttpContext.Current.Request;
            IP = GetIpAddr(request) + ":" + request.Url.Port;
            IISVersion = request.ServerVariables["SERVER_SOFTWARE"];
            UserAgent = request.UserAgent;
            Path = request.Path;
            HttpMethod = request.HttpMethod;
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 异常参数
        /// </summary>
        public string ActionArguments { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// 异常堆栈
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 异常源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 服务器IP 端口
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 客户端浏览器标识
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// .NET解释引擎版本
        /// </summary>
        public string DotNetVersion { get; set; }

        /// <summary>
        ///  应用程序池位数
        /// </summary>
        public string DotNetBit { get; set; }


        /// <summary>
        /// 操作系统类型
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// 操作系统位数
        /// </summary>
        public string OSBit { get; set; }

        /// <summary>
        /// CPU个数
        /// </summary>
        public string CPUCount { get; set; }

        /// <summary>
        /// CPU类型
        /// </summary>
        public string CPUType { get; set; }

        /// <summary>
        /// IIS版本
        /// </summary>
        public string IISVersion { get; set; }

        /// <summary>
        /// 请求地址类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否显示异常界面
        /// </summary>
        public bool ShowException { get; set; }

        /// <summary>
        /// 异常发生时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 异常发生方法
        /// </summary>
        public string Method { get; set; }

        //这段代码用户请求真实IP
        private static string GetIpAddr(HttpRequest request)
        {
            //HTTP_X_FORWARDED_FOR
            string ipAddress = request.ServerVariables["x-forwarded-for"];
            if (!IsEffectiveIP(ipAddress))
            {
                ipAddress = request.ServerVariables["Proxy-Client-IP"];
            }
            if (!IsEffectiveIP(ipAddress))
            {
                ipAddress = request.ServerVariables["WL-Proxy-Client-IP"];
            }
            if (!IsEffectiveIP(ipAddress))
            {
                return "";
            }
            // 对于通过多个代理的情况，第一个IP为客户端真实IP,多个IP按照','分割
            if (ipAddress != null && ipAddress.Length > 15)
            {
                if (ipAddress.IndexOf(",") > 0)
                {
                    ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(","));
                }
            }
            return ipAddress;
        }

        /// <summary>
        /// 是否有效IP地址
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>bool</returns>
        private static bool IsEffectiveIP(string ipAddress)
        {
            return !(string.IsNullOrEmpty(ipAddress) || "unknown".Equals(ipAddress, StringComparison.OrdinalIgnoreCase));
        }
    }
}