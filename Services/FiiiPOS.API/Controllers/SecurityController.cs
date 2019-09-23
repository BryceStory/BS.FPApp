using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using System.Web.Http;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.Business.Properties;
using FiiiPOS.API.Models;
using System.Linq;
using System;
using FiiiPOS.API.Filters;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// FiiiPOS安全验证
    /// </summary>
    [RoutePrefix("Security")]
    public class SecurityController : ApiController
    {
        /// <summary>
        /// 获取安全验证的手机的验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetSecurityCellphoneCode")]
        public ServiceResult<bool> GetSecurityCellphoneCode(GetSecurityCellphoneCodeModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new SecurityComponent().FiiiPOSSendSecurityValidateCellphoneCode(this.GetMerchantAccountId(), model?.DivisionCode);

            result.Data = true;
            result.Message = Resources.发送成功;
            return result;
        }

        /// <summary>
        /// 重置PIN码时，综合验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyResetPinCombine")]
        public ServiceResult<string> VerifyResetPinCombine(CombineVerifyModel model)
        {
            ServiceResult<string> result = new ServiceResult<string>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();
            new SecurityComponent().VerifyResetPinCombine(accountId,model.IdNumber, model.SMSCode, model.GoogleCode);

            return result;
        }

        /// <summary>
        /// Reset PIN 
        /// </summary>
        /// <param name="im">The im.</param>
        /// <returns></returns>
        [HttpPost, Route("ResetPIN")]
        public ServiceResult ResetPIN(ResetPINModel im)
        {
            var result = new ServiceResult();
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
                result.Message = Resources.PIN格式不正确;
                return result;
            }

            new SecurityComponent().ResetPIN(this.GetMerchantAccountId(), pin);
            result.Success();
            return result;
        }
    }
}
