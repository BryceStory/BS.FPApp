using FiiiPay.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class APIControllerActionInvoker: ApiControllerActionInvoker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {

            var result = base.InvokeActionAsync(actionContext, cancellationToken);

            if (result.Exception != null && result.Exception.GetBaseException() != null)
            {
                var baseException = result.Exception.InnerExceptions[0];

                List<string> errors = new List<string>();
                string msg = baseException.Message;

                if (baseException.InnerException != null)
                {
                    baseException = baseException.InnerException;
                }

                errors.Add(baseException.ToString());
                msg = baseException.Message;


                return Task.Run<HttpResponseMessage>(() =>
                    actionContext.Request.CreateResponse(HttpStatusCode.OK,new ServiceResult<string>()
                    {
                        Code =500,
                        Message = "System error",
                        Data = "System error"

                    }));
            }
            return result;
        }
    }
}