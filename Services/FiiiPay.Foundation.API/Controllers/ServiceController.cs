using System.Web.Http;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Exceptions;
using log4net;

namespace FiiiPay.Foundation.API.Controllers
{
    [System.Web.Http.RoutePrefix("api/Service")]
    public class ServiceController : ApiController
    {
        /// <summary>
        /// 获取服务器状态
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost, System.Web.Http.Route("GetFiiiPayServiceStatus"), System.Web.Http.AllowAnonymous]
        public ServiceResult GetFiiiPayServiceStatus()
        {
            ServiceResult result = new ServiceResult();
            var IsMaintain = RedisHelper.StringGet($"FiiiPay:API:IsMaintain");

            if (string.IsNullOrWhiteSpace(IsMaintain))
                IsMaintain = "false";

            if (IsMaintain.Equals("True"))
            {
                throw new CommonException(10300, "Maintenance");
            }
            result.Success();
            return result;
        }

        /// <summary> 
        /// 获取服务器状态
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost, System.Web.Http.Route("GetFiiiPosServiceStatus"), System.Web.Http.AllowAnonymous]
        public ServiceResult GetFiiiPosServiceStatus()
        {
            ILog _logger = LogManager.GetLogger("LogicError");
            ServiceResult result = new ServiceResult();
            var IsMaintain = RedisHelper.StringGet($"FiiiPos:API:IsMaintain");

            if (string.IsNullOrWhiteSpace(IsMaintain))
                IsMaintain = "false";

            _logger.Info("----------------------");
            _logger.Info(IsMaintain);
            if (IsMaintain.Equals("True"))
            {
                throw new CommonException(10300, "Maintenance");
            }
            result.Success();
            return result;
        }
    }
}