using FiiiPOS.Web.Business;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class APIMessageHandler: DelegatingHandler
    {
        /// <summary>
        /// 
        /// </summary>
        protected ILog Logger = WebLog.GetInstance();

        /// <summary>
        /// 重写发送HTTP请求到内部处理程序的方法
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <param name="cancellationToken">取消操作的标记</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 记录请求内容
            string method = request.Method.Method;
            string url = request.RequestUri.ToString();
            string data = null;
            if (!request.Content.IsMimeMultipartContent() && request.Content != null)
            {
                data = request.Content.ReadAsStringAsync().Result;
            }
            IEnumerable<string> tokens = null;
            string token = null;
            if (request.Headers.TryGetValues("Authorization", out tokens))
            {
                token = tokens != null ? (string)tokens.First() : null;
            }

            string returnmsg = null;

            try
            {
                // 发送HTTP请求到内部处理程序，在异步处理完成后记录响应内容
                return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(
                (task) =>
                {
                    // 记录响应内容
                    if (task.Result.Content != null)
                    {
                        returnmsg = task.Result.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        returnmsg = task.Result + "";
                    }

                    if (!string.IsNullOrEmpty(returnmsg) && returnmsg.Length > 10240) returnmsg = returnmsg.Substring(0, 10240);

                    string msg = null; //
                    if (method == "POST")
                    {
                        msg = String.Format(@" curl -i -H ""Authorization:{0}"" -d ""{1}"" ""{2}"" result: {3} ", token, data, url, returnmsg);
                    }
                    else
                    {
                        msg = String.Format(@"curl -i -H ""Authorization:{0}""   ""{1}""   result: {2} ", token, url, returnmsg);
                    }


                    //log request data
                    Logger.Info(msg);

                    return task.Result;
                }
                );
            }
            finally
            {

            }
        }
    }
}