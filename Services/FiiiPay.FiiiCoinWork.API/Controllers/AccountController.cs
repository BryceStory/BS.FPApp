using System.Web.Http;
using FiiiPay.DTO.Investor;
using FiiiPay.FiiiCoinWork.API.Models;
using FiiiPay.Framework;
using FiiiPay.Business;
using FiiiPay.FiiiCoinWork.API.Extensions;

namespace FiiiPay.FiiiCoinWork.API.Controllers
{
    /// <summary>
    /// 用户相关
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("Signon")]
        public ServiceResult<SignonDTO> Signon(SignonModel model)
        {
            var result = new ServiceResult<SignonDTO>();
            result.Data = new InvestorAccountComponent().Login(model.Username, model.Password);
            result.Successful();
            return result;
        }

        /// <summary>
        /// 获取帐号信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("AccountInfo")]
        public ServiceResult<AccountInfoDTO> AccountInfo()
        {
            var result = new ServiceResult<AccountInfoDTO>();
            var user = this.GetUser();
            result.Data = new InvestorAccountComponent().GetUserInfo(user);
            result.Successful();
            return result;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ModifyPassword")]
        public ServiceResult<bool> ModifyPassword(ModifyPasswordModel model)
        {
            var result = new ServiceResult<bool>();

            new InvestorAccountComponent().ModifyPassword(this.GetUser(), model.OldPassword, model.NewPassword);
            result.Data = true;
            result.Successful();
            return result;
        }

        /// <summary>
        /// 修改PIN
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ModifyPIN")]
        public ServiceResult<bool> ModifyPIN(ModifyPINModel model)
        {
            var result = new ServiceResult<bool>();
            new InvestorAccountComponent().ModifyPIN(this.GetUser(), model.OldPIN, model.NewPIN);
            result.Data = true;
            result.Successful();
            return result;
        }

        /// <summary>
        /// 验证PIN，获取PIN Token
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyPIN")]
        public ServiceResult<string> VerifyPIN(VerifyPINModel model)
        {
            var result = new ServiceResult<string>();
            result.Data = new InvestorAccountComponent().VerifyPIN(this.GetUser(), model.EncryptPIN);
            result.Successful();
            return result;
        }
    }
}
