using System;
using System.Linq;
using System.Web.Http;
using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Account;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.AccountController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Account")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// 发送注册验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SendRegisterSMSCode")]
        [AllowAnonymous]
        public ServiceResult<bool> SendRegisterSMSCode(GetRegisterSMSCodeModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();

            new UserAccountComponent().SendRegisterCode(model.CountryId, model.Cellphone);

            result.Data = true;
            

            return result;
        }

        /// <summary>
        /// 验证注册验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("CheckRegisterSMSCode")]
        [AllowAnonymous]
        public ServiceResult<bool> CheckRegisterSMSCode(ChekRegisterSMSCodeModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();

            var checkResult = new UserAccountComponent().CheckRegisterSMSCode(model.CountryId, model.Cellphone, model.SMSCode);

            result.Data = checkResult;
            

            return result;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Register")]
        [AllowAnonymous]
        public ServiceResult<bool> Register(RegisterModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();

            var im = new RegisterIM
            {
                CountryId = model.CountryId,
                Cellphone = model.Cellphone,
                Password = model.Password,
                SMSCode = model.SMSCode,
                InviterCode = model.InviterCode
            };

            result.Data = new UserAccountComponent().AppRegister(im);
            

            return result;
        }

        /// <summary>
        /// H5注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("H5Register")]
        [AllowAnonymous]
        public ServiceResult<bool> H5Register(RegisterModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();

            var im = new H5RegisterIM
            {
                CountryId = model.CountryId,
                Cellphone = model.Cellphone,
                Password = model.Password,
                SMSCode = model.SMSCode,
                InviterCode = model.InviterCode
            };

            result.Data = new UserAccountComponent().H5Register(im);
            

            return result;
        }

        /// <summary>
        /// 返回邀请码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetInvitationCode")]
        public ServiceResult<string> GetInvitationCode()
        {
            return new ServiceResult<string>
            {
                Data = this.GetUser().InvitationCode
            };
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Login")]
        [AllowAnonymous]
        public ServiceResult<LoginOM> Login(LoginIM im)
        {
            ServiceResult<LoginOM> result = new ServiceResult<LoginOM>();

            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            result.Data = new UserAccountComponent().Login(im, deviceNumber, this.GetClientIPAddress());

            return result;
        }

        /// <summary>
        /// 登录-二级验证
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ValidateLogin")]
        [AllowAnonymous]
        public ServiceResult<LoginOM> ValidateLogin(ValidateAuthCodeIM im)
        {
            ServiceResult<LoginOM> result = new ServiceResult<LoginOM>();

            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            result.Data = new UserAccountComponent().ValidateAuthenticator(im, deviceNumber);

            return result;
        }

        /// <summary>
        /// 新设备登录
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("NewDeviceLogin")]
        [AllowAnonymous]
        public ServiceResult<LoginOM> NewDeviceLogin(NewDeviceLoginIM im)
        {
            ServiceResult<LoginOM> result = new ServiceResult<LoginOM>();

            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            result.Data = new UserAccountComponent().NewDeviceLogin(im, deviceNumber);

            return result;
        }

        /// <summary>
        /// 绑定设备到推送服务
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("BindNoticeRegId")]
        public ServiceResult<bool> BindNoticeRegId(BindNoticeRegIdIM im)
        {
            var result = new ServiceResult<bool>();

            new UserAccountComponent().BindNoticeRegId(this.GetUser().Id, im);
            result.Data = true;

            return result;
        }

        /// <summary>
        /// 解除推送服务绑定，此操作成功后，客户端将不再受到推送
        /// 想要收到推送的话需要重新调用BindNoticeRegId接口
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("UnBindNoticeRegId")]
        public ServiceResult<bool> UnBindNoticeRegId()
        {
            new UserAccountComponent().RemoveTagsByUserId(this.GetUser().Id);
            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 退出登录接口，此接口将解除设备的推送服务绑定
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Logout")]
        public ServiceResult<bool> Logout()
        {
            new UserAccountComponent().Logout(this.GetUser());
            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 提供一个单独的接口获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Info")]
        public ServiceResult<SimpleUserInfoOM> Info()
        {
            return new ServiceResult<SimpleUserInfoOM>
            {
                Data = new UserAccountComponent().GetSimpleUserInfoOM(this.GetUser())
            };
        }

        /// <summary>
        /// 获取登录验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetLoginSMSCode")]
        [AllowAnonymous]
        public ServiceResult<bool> GetLoginSMSCode(GetSMSCodeIM im)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();

            new UserAccountComponent().SendLoginCode(im.CountryId, im.Cellphone);

            result.Data = true;
            

            return result;
        }

        /// <summary>
        /// 通过短信验证码登录，如果用户未注册，code = 10012，此时客户端应该提示用户注册
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("LoginBySMSCode")]
        [AllowAnonymous]
        public ServiceResult<LoginOM> LoginBySMSCode(LoginBySMSCodeIM im)
        {
            ServiceResult<LoginOM> result = new ServiceResult<LoginOM>();
            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            result.Data = new UserAccountComponent().LoginBySMSCode(im.CountryId, im.Cellphone, im.SMSCode, deviceNumber);
            return result;
        }

        /// <summary>
        /// 通过短信验证码登录第二步，验证google authencator
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ValidateLoginBySMSCode")]
        [AllowAnonymous]
        public ServiceResult<LoginOM> ValidateLoginBySMSCode(ValidateLoginBySMSCodeIM im)
        {
            ServiceResult<LoginOM> result = new ServiceResult<LoginOM>();
            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            result.Data = new UserAccountComponent().ValidateAuthenticatorBySMSCode(im, deviceNumber);
            return result;
        }

        /// <summary>
        /// 通过短信验证码登录，新设备登录
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("NewDeviceLoginBySMSCode")]
        [AllowAnonymous]
        public ServiceResult<LoginOM> NewDeviceLoginBySMSCode(NewDeviceLoginBySMSCodeIM im)
        {
            ServiceResult<LoginOM> result = new ServiceResult<LoginOM>();
            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            result.Data = new UserAccountComponent().NewDeviceLoginBySMSCode(im, deviceNumber);
            return result;
        }

        /// <summary>
        /// 验证Pin码，超过五次错误会锁定30分钟，此时code = 10013
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyPin")]
        public ServiceResult<bool> VerifyPin(VerifyPinIM im)
        {
            new SecurityComponent().VerifyPin(this.GetUser(), im.Pin);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }
        /// <summary>
        /// 通过邀请码返回手机号码和区域号
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetPhoneNumByInviteCode")]
        public ServiceResult<CellPhoneOM> GetPhoneNumByInviteCode(InviteCodeIM im)
        {
            return new ServiceResult<CellPhoneOM>
            {
                Data = new UserAccountComponent().GetCellPhoneInfo(im.InviteCode)
            };
        }

        /// <summary>
        /// 设置法币
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SettingFiatCurrency")]
        public ServiceResult<bool> SettingFiatCurrency(SettingFiatCurrencyModel model)
        {
            var result = new ServiceResult<bool>();

            new UserAccountComponent().SettingFiatCurrency(this.GetUser().Id, model.FiatCurrency);

            result.Data = true;

            return result;
        }

        /// <summary>
        /// 查看解绑信息
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, Route("UnBindingMerchantMessage")]
        public ServiceResult<BonusMessageOM> UnBindingMerchantMessage(long id)
        {
            var result = new ServiceResult<BonusMessageOM>()
            {
                Data = new UserAccountComponent().UnBindingMerchantMessage(id)
            };
            return result;
        }

        /// <summary>
        /// 查看解绑信息
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, Route("InviteFiiiposSuccessMessage")]
        public ServiceResult<BonusMessageOM> InviteFiiiposSuccessMessage(long id)
        {
            var result = new ServiceResult<BonusMessageOM>()
            {
                Data = new UserAccountComponent().InviteFiiiposSuccessMessage(id)
            };
            return result;
        }

        /// <summary>
        /// Changes the language.
        /// </summary>
        /// <param name="im">The im.</param>
        /// <returns></returns>
        [HttpPost, Route("ChangeLanguage")]
        public ServiceResult ChangeLanguage(ChangeLanguageModel im)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            if (im.Language.Contains("en") || im.Language.Contains("zh"))
            {
                new UserAccountComponent().ChangeLanguagetoDb(this.GetUser().Id, im.Language);
            }

            result.Success();
            return result;
        }

        /// <summary>
        /// 获取上一次活跃的国家地点
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetLastActiveCountry")]
        public ServiceResult<GetLastActiveCountryOM> GetLastActiveCountry()
        {
            return new ServiceResult<GetLastActiveCountryOM>()
            {
                Data = new UserAccountComponent().GetLastActiveCountry(this.GetUser().Id)
            };
        }
        /// <summary>
        /// 设置当前的活跃国家
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SetActiveCountry")]
        public ServiceResult<bool> SetActiveCountry(SetActiveCountryIM im)
        {
            new UserAccountComponent().SetActiveCountry(this.GetUser().Id, im);
            return new ServiceResult<bool>()
            {
                Data = true
            };
        }
    }
}
