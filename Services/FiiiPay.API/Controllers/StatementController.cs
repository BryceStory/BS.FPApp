using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Statement;
using FiiiPay.Framework;
using System.Threading.Tasks;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 用户流水
    /// </summary>
    [RoutePrefix("Statement")]
    public class StatementController : ApiController
    {
        /// <summary>
        /// 获取用户流水列表 所有类型数据
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ListAllType")]
        public async Task<ServiceResult<ListOM>> ListAllType(ListAllTypeIM im)
        {
            return new ServiceResult<ListOM>
            {
                Data = await new UserStatementComponent().ListAllType(this.GetUser(), im, this.IsZH())
            };
        }

        /// <summary>
        /// 获取用户流水列表 单个类型数据
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ListSingleType")]
        public async Task<ServiceResult<ListOM>> ListSingleType(ListSingleTypeIM im)
        {
            return new ServiceResult<ListOM>
            {
                Data = await new UserStatementComponent().ListSingleType(this.GetUser(), im, this.IsZH())
            };
        }
    }
}
