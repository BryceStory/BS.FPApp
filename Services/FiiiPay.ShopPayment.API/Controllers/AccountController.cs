using System;
using System.Threading.Tasks;
using System.Web.Http;
using FiiiPay.Framework.Exceptions;
using FiiiPay.ShopPayment.API.Components;
using FiiiPay.ShopPayment.API.Models;
using log4net;

namespace FiiiPay.ShopPayment.API.Controllers
{
    /// <summary>
    /// Class Account
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Account")]
    public class AccountController : ApiController
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(AccountController));

        /// <summary>
        /// 获取登录二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetLoginQRCode")]
        public ResultDto<string> GetLoginQRCode()
        {
            ResultDto<string> result = new ResultDto<string>();
            result.Data = new AccountComponent().GetLoginQRCode();
            return result;
        }

        /// <summary>
        /// 获取二维码扫码登录状态，登陆成功之后，成功查询后不能再次用此二维码登录和查询。
        /// 10090 无效的二维码，
        /// 30001 扫码未登陆，
        /// 30002 扫码登录中。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, Route("GetLoginStatus")]
        public async Task<ResultDto<LoginDto>> GetLoginStatus(string code)
        {
            var result = new ResultDto<LoginDto>();
            result.Data = await new AccountComponent().GetLoginStatus(code);
            return result;
        }

        /// <summary>
        /// 发送登录短信码,测试环境中短信码固定为123456,且不会真实发送短信,
        /// 10012 账户不存在,
        /// 10021 账户已被禁用,
        /// 10072 短信验证码错误多次,
        /// 10083 短信发送次数超出限制,
        /// 10084 未知错误
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SendLoginSMSCode")]
        [HttpPost]
        public ResultDto<bool> SendLoginSMSCode(SendLoginSMSCodeVo model)
        {
            var result = new ResultDto<bool>();

            try
            {
                result.Data = new AccountComponent().SendLoginCode(model.CountryPhoneCode, model.Cellphone);
            }
            catch (CommonException exception)
            {
                result.Data = false;
                result.Code = exception.ReasonCode;
                result.Message = exception.Message;

                _log.Error(exception.Message);
            }
            catch (Exception exception)
            {
                result.Data = false;
                result.Code = 10084;
                result.Message = exception.Message;

                _log.Error(exception.Message);
            }

            return result;
        }

        /// <summary>
        /// 短信验证码登录。
        /// 10012 用户不存在，
        /// 10021 用户已禁用，
        /// 10080 短信验证码错误，
        /// 10072 短信验证码错误多次
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("SMSCodeLogin")]
        [HttpPost]
        public async Task<ResultDto<LoginDto>> Login(LoginVo model)
        {
            ResultDto<LoginDto> result = new ResultDto<LoginDto>();
            result.Data = await new AccountComponent().Login(model, "");
            return result;
        }

        /// <summary>
        /// 校验用户PIN码, 错误次数在Message中返回。
        /// 10014 Pin输入错误，
        /// 10013 Pin错误多次,
        /// 10084 未知错误
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CheckPin")]
        [HttpPost]
        public ResultDto<bool> CheckPin(CheckPinVo model)
        {
            ResultDto<bool> result = new ResultDto<bool>();
            try
            {
                result.Data = new AccountComponent().CheckPin(model.UserId, model.Pin);
            }
            catch (CommonException exception)
            {
                result.Data = false;
                result.Code = exception.ReasonCode;
                result.Message = exception.Message;
            }
            catch (Exception exception)
            {
                result.Data = false;
                result.Code = 10084;
                result.Message = exception.Message;
            }
            
            return result;
        }

        /// <summary>
        /// 获取加密币数量
        /// </summary>
        /// <returns></returns>
        [Route("GetFiiiCryptoWallet")]
        [HttpGet]
        public ResultDto<CryptoWalletDto> GetFiiiCryptoWallet(Guid userId)
        {
            ResultDto<CryptoWalletDto> result = new ResultDto<CryptoWalletDto>();
            result.Data = new AccountComponent().GetFiiiBalance(userId);
            return result;
        }
    }
}
