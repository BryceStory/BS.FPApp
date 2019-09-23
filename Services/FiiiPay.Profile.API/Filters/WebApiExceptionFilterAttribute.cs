using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;


namespace FiiiPay.Profile.API.Filters
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILog _log = LogManager.GetLogger("WebApiExceptionFilterAttribute");
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            if (exception is CommonException)
            {
                var resultData = new ServiceResult<string>
                {
                    Code = (exception as CommonException).ReasonCode,
                    Message = exception.Message
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
            else if (exception is ApplicationException)
            {
                var resultData = new ServiceResult<string>
                {
                    Code = 10000,
                    Message = exception.Message
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
            else
            {
                _log.Error($"Url:{actionExecutedContext.Request.RequestUri} ContentLength:{actionExecutedContext.Request.Content.Headers.ContentLength}");
                _log.Error(exception);
                var resultData = new ServiceResult<string>
                {
                    Code = 10000,
                    Message = "网络异常_请重新尝试"
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
        }
    }
}