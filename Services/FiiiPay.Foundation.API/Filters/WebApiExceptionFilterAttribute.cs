using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using FiiiPay.Foundation.Business.Properties;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;

namespace FiiiPay.Foundation.API.Filters
{
    /// <summary>
    /// Class FiiiPay.API.Filters.WebApiExceptionFilterAttribute
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ExceptionFilterAttribute" />
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(WebApiExceptionFilterAttribute));

        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
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
                _logger.Error(exception.Message + "\r\n" + exception.StackTrace);

                var resultData = new ServiceResult<string>
                {
                    Code = 10000,
                    Message = Resources.SystemError
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ServiceResult<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
        }
    }
}