using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Filters;
using FiiiPay.Framework.Component;
using log4net;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class APIExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(APIExceptionFilterAttribute));
        /// <summary>
        /// 统一对调用异常信息进行处理，返回自定义的异常信息
        /// </summary>
        /// <param name="actionExecutedContext">HTTP上下文对象</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            if (exception is ApplicationException)
            {
                var result = new ServiceResult<string>
                {
                    Code = 10000,
                    Message = exception.Message
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult<string>>(result, new JsonMediaTypeFormatter())
                };
            }
            else if (exception is CommonException ex)
            {
                var result = new ServiceResult<string>
                {
                    Code = ex.ReasonCode,
                    Message = ex.Message
                };

                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult<string>>(result, new JsonMediaTypeFormatter())
                };
            }
            else
            {
                _logger.Error(actionExecutedContext.Request.RequestUri + "\r\n" + actionExecutedContext.Request.Content.ReadAsStringAsync().Result);
                _logger.Error(exception.Message + "\r\n" + exception.StackTrace);

                var result = new ServiceResult<string>
                {
                    Code = 10000,
                    Message = "网络异常_请重新尝试"
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ObjectContent<ServiceResult<string>>(result, new JsonMediaTypeFormatter())
                };
            }

            //记录关键的异常信息
            _logger.Error("System error", exception);
        }

        private static HttpResponseMessage GetResponse(int code, string message)
        {
            ServiceResult result = new ServiceResult()
            {
                Code = code,
                Message = message
            };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<ServiceResult>(
                     result,
                     new JsonMediaTypeFormatter(),
                     "application/json")
            };
        }

    }
}