using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.Business;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 用于显示审核结果详情
    /// </summary>
    [RoutePrefix("VerifyRecord")]
    public class VerifyRecordController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpGet, Route("Detail")]
        public ServiceResult<GetNewsOM> Detail(long id)
        {
            var result = new ServiceResult<GetNewsOM>
            {
                Data = new VerifyRecordComponent().GetRecord(id)
            };
            
            return result;
        }
    }
}