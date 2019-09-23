using System;
using System.Linq;
using System.Web.Http;
using FiiiPay.API.Filters;
using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.Business.Properties;
using FiiiPay.DTO.Security;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.SecurityController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Security")]
    public class SecurityController : ApiController
    {
        /// <summary>
        /// 设置二级密码，客户端首先根据登录返回的SimpleUserInfo的HasSetPin字段判断是否已经设置过Pin
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("SetPin")]
        public ServiceResult<bool> SetPin(SetPinIM im)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            new SecurityComponent().SetPin(im.Pin, this.GetUser(), deviceNumber);

            result.Data = true;
            
            return result;
        }

        /// <summary>
        /// 发送忘记密码的短信
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetForgotPasswordCode"), AllowAnonymous]
        public ServiceResult<bool> SendResetPasswordSMSCode(SendResetPasswordSMSCodeModel im)
        {
            new SecurityComponent().SendForgotPasswordCode(im.CountryId, im.Cellphone);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 验证忘记密码的验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyForgotPasswordCode"), AllowAnonymous]
        public ServiceResult<bool> VerifyForgotPasswordCode(CheckResetPasswordSMSCodeModel im)
        {
            new SecurityComponent().VerifyForgotPasswordCode(im.CountryId, im.Cellphone, im.Code);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 重置密码通过忘记密码的接口
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ResetPasswordByForgotPasswordCode")]
        [AllowAnonymous]
        public ServiceResult<bool> ResetPasswordByForgotPasswordCode(ResetPasswordModel im)
        {
            new SecurityComponent().ForgotPassword(im.CountryId, im.Cellphone, im.Password);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 修改手机号时，验证pin码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateCellphonePin")]
        public ServiceResult<bool> VerifyUpdateCellphonePin(GetUpdateCellphoneCodeIM im)
        {
            new SecurityComponent().VerifyUpdateCellphonePin(this.GetUser(), im.Pin);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 准备验证新的手机号，这个接口会提供用户的国家地区码，以及国家名
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreGetVerifyNewCellphoneCode")]
        public ServiceResult<PreGetVerifyNewCellphoneCodeOM> PreGetVerifyNewCellphoneCode()
        {
            var country = new CountryComponent().GetById(this.GetUser().CountryId);

            return new ServiceResult<PreGetVerifyNewCellphoneCodeOM>
            {
                Data = new PreGetVerifyNewCellphoneCodeOM
                {
                    CountryName = this.IsZH() ? country.Name_CN : country.Name,
                    PhoneCode = country.PhoneCode
                }
            };
        }

        /// <summary>
        /// 获取验证新手机号的验证码（新手机）
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetUpdateCellphoneNewCode")]
        public ServiceResult<bool> GetUpdateCellphoneNewCode(GetVerifyNewCellphoneCodeIM im)
        {
            new SecurityComponent().SendUpdateCellphoneNewCode(this.GetUser(), im.NewCellphone);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 修改手机时，验证新手机号的验证码（新手机）
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateCellphoneNewCode")]
        public ServiceResult<bool> VerifyUpdateCellphoneNewCode(VerifyUpdateCellphoneNewCodeIM im)
        {
            new SecurityComponent().VerifyUpdateCellphoneNewCode(this.GetUser(), im);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 修改手机号时, 综合验证
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateCellphoneCombine")]
        public ServiceResult<bool> VerifyUpdateCellphoneCombine(BaseCombineVerifyModel im)
        {
            new SecurityComponent().VerifyUpdateCellphoneCombine(this.GetUser(), im.SMSCode, im.GoogleCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 更换手机号
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateCellphone")]
        public ServiceResult<bool> UpdateCellphone(UpdateCellphoneIM im)
        {
            new SecurityComponent().UpdateCellphone(this.GetUser(), im.NewCellphone);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 修改Pin时，验证旧的PIN码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdatePinPin")]
        public ServiceResult<bool> VerifyUpdatePinPin(GetUpdatePinCodeIM im)
        {
            new SecurityComponent().VerifyUpdatePinPin(this.GetUser(), im.Pin);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 修改Pin时，综合验证
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdatePinCombine")]
        public ServiceResult<bool> VerifyUpdatePinCombine(CombineVerifyModel im)
        {
            new SecurityComponent().VerifyUpdatePinCombine(this.GetUser(), im.SMSCode, im.GoogleCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 修改Pin
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdatePin")]
        public ServiceResult<bool> UpdatePin(UpdatePinIM im)
        {
            new SecurityComponent().UpdatePin(this.GetUser(), im.OldPin, im.NewPin);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 找回Pin前使用此接口查看用户是否已认证身份LV1
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreFindPinBack")]
        public ServiceResult<PreFindPinBackOM> PreFindPinBack()
        {
            return new ServiceResult<PreFindPinBackOM>
            {
                Data = new SecurityComponent().PreFindPinBack(this.GetUser())
            };
        }

        /// <summary>
        /// 修改密码，验证PIN
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdatePasswordPin")]
        public ServiceResult<bool> VerifyUpdatePasswordPin(VerifyUpdatePasswordModel im)
        {
            new SecurityComponent().VerifyUpdatePasswordPin(this.GetUser(), im.Pin);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 获取安全验证的手机的验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetSecurityCellphoneCode")]
        public ServiceResult<bool> GetSecurityCellphoneCode(GetSecurityCellphoneCodeModel key)
        {
            new SecurityComponent().SendSecurityValidateCellphoneCode(this.GetUser(),key?.DivisionCode);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }
        
        /// <summary>
        /// 修改Password
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdatePassword")]
        public ServiceResult<bool> UpdatePassword(UpdatePasswordIM im)
        {
            new SecurityComponent().UpdatePassword(this.GetUser(), im.Password, im.OldPassword);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 重置PIN码时，综合验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyResetPinCombine")]
        public ServiceResult<bool> VerifyResetPinCombine(CombineVerifyModel model)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetUser().Id;
            new SecurityComponent().VerifyResetPinCombine(accountId, model.IdNumber, model.SMSCode, model.GoogleCode);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// Reset PIN 
        /// </summary>
        /// <param name="im">The im.</param>
        /// <returns></returns>
        [HttpPost, Route("ResetPIN")]
        public ServiceResult<bool> ResetPIN(ResetPINModel im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;
                return result;
            }
            if (!PinProcessor.TryParse(im.PIN, out string pin))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                result.Message = MessageResources.PINInvalidFormat;
                return result;
            }
            new SecurityComponent().ResetPIN(this.GetUser().Id, pin);
            result.Success();
            result.Data = true;
            return result;
        }
    }
}