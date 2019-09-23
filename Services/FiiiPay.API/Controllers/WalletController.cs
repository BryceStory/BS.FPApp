using FiiiPay.Business;
using FiiiPay.DTO.Wallet;
using FiiiPay.Framework;
using System.Web.Http;
using FiiiPay.DTO;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 用户钱包接口
    /// </summary>
    [RoutePrefix("Wallet")]
    public class WalletController : ApiController
    {
        /// <summary>
        /// 充币以及提币的列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ListForDepositAndWithdrawal")]
        public ServiceResult<ListForDepositOM> ListForDepositAndWithdrawal()
        {
            var user = this.GetUser();
            return new ServiceResult<ListForDepositOM>
            {
                Data = new UserWalletComponent().ListForDepositAndWithdrawal(user, user.FiatCurrency)
            };
        }

        /// <summary>
        /// 准备调整币种交易顺序
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreReOrder")]
        public ServiceResult<WalletPreReOrderOM> PreReOrder()
        {
            return new ServiceResult<WalletPreReOrderOM>
            {
                Data = new UserWalletComponent().PreReOrder(this.GetUser())
            };
        }

        /// <summary>
        /// 排序（功能尚未实现）
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ReOrder")]
        public ServiceResult<bool> ReOrder(ReOrderIM im)
        {
            new UserWalletComponent().ReOrder(this.GetUser(), im.IdList);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }
    }
}
