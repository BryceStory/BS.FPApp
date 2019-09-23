using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.DTO.Investor;
using FiiiPay.FiiiCoinWork.API.Extensions;
using FiiiPay.FiiiCoinWork.API.Models;
using FiiiPay.Framework;

namespace FiiiPay.FiiiCoinWork.API.Controllers
{
    /// <summary>
    /// 钱包相关
    /// </summary>
    [RoutePrefix("api/Wallet")]
    public class WalletController : ApiController
    {

        /// <summary>
        /// 获取FiiiCoin总额
        /// </summary>
        /// <returns></returns>
        //[HttpGet, Route("TotalAssets")]
        //public ServiceResult<string> TotalAssets()
        //{
        //    ServiceResult<string> result = new ServiceResult<string>();
        //    InvestorWalletComponent cpt = new InvestorWalletComponent();
        //    string totalAssets = cpt.GetTotalAssets(this.GetUser());
        //    result.Data = totalAssets;
        //    result.Successful();
        //    return result;
        //}

        /// <summary>
        /// 获取FiiiCoin可用余额
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Banlance")]
        public ServiceResult<string> Banlance()
        {
            ServiceResult<string> result = new ServiceResult<string>();
            InvestorWalletComponent cpt = new InvestorWalletComponent();
            result.Data = cpt.GetBanlance(this.GetUser());
            result.Successful();
            return result;
        }

        /// <summary>
        /// 获取目标帐号信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("TargetAccount")]
        public ServiceResult<TargetAccountDTO> TargetAccount(TargetAccountModel model)
        {
            ServiceResult<TargetAccountDTO> result = new ServiceResult<TargetAccountDTO>();
            InvestorWalletComponent cpt = new InvestorWalletComponent();
            result.Data = cpt.GetTargetAccount(model.CountryId, model.Cellphone);
            result.Successful();
            return result;
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Transfer")]
        public ServiceResult<TransferResult> Transfer(TransferModel model)
        {
            ServiceResult<TransferResult> result = new ServiceResult<TransferResult>();
            InvestorWalletComponent cpt = new InvestorWalletComponent();
            result.Data = cpt.Transfer(this.GetUser(), model.CountryId, model.Cellphone, model.Amount, model.PINToken);
            result.Successful();
            return result;
        }
    }
}
