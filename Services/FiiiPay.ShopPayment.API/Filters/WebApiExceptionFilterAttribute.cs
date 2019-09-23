using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using FiiiPay.ShopPayment.API.Models;
using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;

namespace FiiiPay.ShopPayment.API.Filters
{
    /// <summary>
    /// Class WebApiExceptionFilterAttribute
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
            if (exception is UnauthorizedException)
            {
                var resultData = new ResultDto<string>
                {
                    Code = (exception as UnauthorizedException).ReasonCode,
                    Message = exception.Message
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ResultDto<string>>(resultData, new JsonMediaTypeFormatter())
                };
                return;
            }
            if (exception is CommonException)
            {
                var resultData = new ResultDto<string>
                {
                    Code = (exception as CommonException).ReasonCode,
                    Message = exception.Message
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ResultDto<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
            else if (exception is ApplicationException)
            {
                var resultData = new ResultDto<string>
                {
                    Code = ReasonCode.GENERAL_ERROR,
                    Message = exception.Message
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ResultDto<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
            else
            {
                _logger.Error(exception.Message + "\r\n" + exception.StackTrace);

                var resultData = new ResultDto<string>
                {
                    Code = ReasonCode.GENERAL_ERROR,
                    Message = "Net error"
                };
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<ResultDto<string>>(resultData, new JsonMediaTypeFormatter())
                };
            }
        }
    }
}