using FiiiPay.Framework;
using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Deposit;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.DepositController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Deposit")]
    public class DepositController : ApiController
    {
        /// <summary>
        /// 充币的详情
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<DetailOM> Detail(GetDetailByIdIM<long> im)
        {
            return new ServiceResult<DetailOM>
            {
                Data = new UserDepositComponent().Detail(this.GetUser(), im.Id, this.IsZH())
            };
        }

        /// <summary>
        /// 充币，获取充币地址
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("PreDeposit")]
        public ServiceResult<PreDepositOM> PreDeposit(GetDetailByIdIM<int> im)
        {
            return new ServiceResult<PreDepositOM>
            {
                Data = new UserDepositComponent().PreDeposit(this.GetUser(), im.Id)
            };
        }
    }
}
