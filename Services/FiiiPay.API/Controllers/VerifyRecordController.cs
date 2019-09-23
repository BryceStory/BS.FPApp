using FiiiPay.Business;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPay.DTO.Messages;
using System.Threading.Tasks;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 用于显示审核结果详情
    /// </summary>
    [RoutePrefix("VerifyRecord")]
    public class VerifyRecordController : ApiController
    {
        /// <summary>
        /// Details the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<GetNewsOM> Detail(long id)
        {
            var result = new ServiceResult<GetNewsOM>
            {
                Data = new VerifyRecordComponent().GetRecord(id)
            };

            result.Success();
            return result;
        }

        /// <summary>
        /// fiiipay merchant 审核消息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("FiiipayMerchantVerifyDetail")]
        public async Task<ServiceResult<VerifyMessageOM>> FiiipayMerchantVerifyDetail(long id)
        {
            var result = new ServiceResult<VerifyMessageOM>
            {
                Data = await new VerifyRecordComponent().GetFiiipayMerchantMessageAsync(id)
            };

            result.Success();
            return result;
        }
    }
}