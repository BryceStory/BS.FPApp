using FiiiPay.Business;
using FiiiPay.DTO.Authenticator;
using FiiiPay.Framework;
using System.Web.Http;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.API.Models;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 谷歌验证器
    /// </summary>
    [RoutePrefix("Authenticator")]
    public class AuthenticatorController : ApiController
    {
        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GenerateSecretKey")]
        public ServiceResult<GenerateSecretKeyOM> GenerateSecretKey()
        {
            var user = this.GetUser();
            var result = new AuthenticatorComponent().GenerateUserSecretKey(user);

            return new ServiceResult<GenerateSecretKeyOM>
            {
                Data = new GenerateSecretKeyOM
                {
                    SecretKey = result.ManualEntryKey,
                    QrCodeUrl = result.QrCodeSetupUrl
                }
            };
        }

        /// <summary>
        /// 绑定验证谷歌验证码到用户,验证PIN
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyBindPin")]
        public ServiceResult<bool> VerifyBindPin(VerifyPinModel im)
        {
            new AuthenticatorComponent().VerifyBindPin(this.GetUser(), im.PIN);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 绑定验证谷歌验证码到用户,验证谷歌验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyBindGoogleAuth")]
        public ServiceResult<bool> VerifyBindGoogleAuth(ValidateWithSecretIM im)
        {
            new AuthenticatorComponent().VerifyBindGoogleAuth(this.GetUser().Id, im.SecretKey, im.Code);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 绑定验证谷歌验证码到用户,综合验证
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyBindCombine")]
        public ServiceResult<bool> VerifyBindCombine(BaseCombineVerifyModel im)
        {
            new AuthenticatorComponent().VerifyBindCombine(this.GetUser(), im.SMSCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 绑定验证谷歌验证码到用户
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Bind")]
        public ServiceResult<bool> Bind(BindUserAuthIM im)
        {
            new AuthenticatorComponent().BindUserAccount(im, this.GetUser());

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 解绑谷歌验证,验证PIN
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUnBindPin")]
        public ServiceResult<bool> VerifyUnBindPin(VerifyPinModel im)
        {
            new AuthenticatorComponent().VerifyUnBindPin(this.GetUser(), im.PIN);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 解绑谷歌验证,综合验证
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUnBindCombine")]
        public ServiceResult<bool> VerifyUnBindCombine(BaseCombineVerifyModel im)
        {
            new AuthenticatorComponent().VerifyUnBindCombine(this.GetUser(), im.SMSCode, im.GoogleCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 解绑用户的验证谷歌验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("UnBind")]
        public ServiceResult<bool> UnBind()
        {
            new AuthenticatorComponent().UnBindUserAccount(this.GetUser());

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 开启谷歌验证,验证谷歌验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyOpenGoogleAuth")]
        public ServiceResult<bool> VerifyOpenGoogleAuth(ValidateGoogleAuthIM im)
        {
            new AuthenticatorComponent().VerifyOpenGoogleAuth(this.GetUser(), im.GoogleCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// open google authencator
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Open")]
        public ServiceResult<bool> Open()
        {
            new AuthenticatorComponent().OpenUserAccount(this.GetUser());

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 关闭谷歌验证,验证PIN
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyClosePin")]
        public ServiceResult<bool> VerifyClosePin(VerifyPinModel im)
        {
            new AuthenticatorComponent().VerifyClosePin(this.GetUser(), im.PIN);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 关闭谷歌验证,综合验证
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyCloseCombine")]
        public ServiceResult<bool> VerifyCloseCombine(BaseCombineVerifyModel im)
        {
            new AuthenticatorComponent().VerifyCloseCombine(this.GetUser(), im.SMSCode, im.GoogleCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// close google authencator
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Close")]
        public ServiceResult<bool> Close()
        {
            new AuthenticatorComponent().CloseUserAccount(this.GetUser());

            return new ServiceResult<bool>
            {
                Data = true
            };
        }
        /// <summary>
        /// 验证谷歌验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Validate")]
        public ServiceResult<string> Validate(UserValidateIM im)
        {
            var result = new AuthenticatorComponent().ValidateUserAccount(this.GetUser(), im.Code);

            return new ServiceResult<string>
            {
                Data = result
            };
        }

        /// <summary>
        /// 获取密保状态
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetStatusOfSecurity")]
        public ServiceResult<GetStatusOfSecurityOM> GetStatusOfSecurity()
        {
            var result = new AuthenticatorComponent().GetUserStatusOfSecurity(this.GetUser());
            return new ServiceResult<GetStatusOfSecurityOM>
            {
                Data = result
            };
        }

        /// <summary>
        /// 获取用户开启的密保方式
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetOpenedSecurities")]
        public ServiceResult<GetOpenedSecuritiesOM> GetOpenedSecurities()
        {

            var result = new AuthenticatorComponent().GetUserOpenedSecurities(this.GetUser());

            return new ServiceResult<GetOpenedSecuritiesOM>
            {
                Data = result
            };
        }
    }
}
