using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Transfer;
using FiiiPay.Framework;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 转账
    /// </summary>
    [RoutePrefix("Transfer")]
    public class TransferController : ApiController
    {
        /// <summary>
        /// 验证用户是否可以转账
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("CheckTransferAble")]
        public ServiceResult<bool> CheckTransferAble()
        {
            var account = this.GetUser();
            return new ServiceResult<bool>
            {
                Data = !account.IsAllowTransfer.HasValue || account.IsAllowTransfer.Value
            };
        }
        /// <summary>
        /// 转账详情
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<TransferDetailOM> Detail(GetDetailByIdIM<long> im)
        {
            return new ServiceResult<TransferDetailOM>
            {
                Data = new TransferComponent().Detail(this.GetUser().Id, im.Id)
            };
        }
        /// <summary>
        /// 准备转账
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("PreTransfer")]
        public ServiceResult<PreTransferOM> PreTransfer(PreTransferIM im)
        {
            return new ServiceResult<PreTransferOM>
            {
                Data = new TransferComponent().PreTransfer(this.GetUser(), im)
            };
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Transfer")]
        public ServiceResult<TransferOM> Transfer(TransferIM im)
        {
            return new ServiceResult<TransferOM>
            {
                Data = new TransferComponent().Transfer(this.GetUser(), im)
            };
        }
    }
}
