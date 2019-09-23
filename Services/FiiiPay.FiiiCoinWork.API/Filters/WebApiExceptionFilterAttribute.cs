using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Properties;
using log4net;

#pragma warning disable 1591

namespace FiiiPay.FiiiCoinWork.API.Filters
{
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(WebApiExceptionFilterAttribute));
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            if (exception is CommonException)
            {
                var ex = exception as CommonException;
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
                _logger.Error("System Error", exception);
                var result = new ServiceResult<string>
                {
                    Code = 10000,
                    Message = R.网络异常_请重新尝试
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ObjectContent<ServiceResult<string>>(result, new JsonMediaTypeFormatter())
                };
            }
        }
    }
}