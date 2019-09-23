using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.API.Models;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// Class FiiiPOS.API.Controllers.ProfileController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api/Profile")]
    public class ProfileController : ApiController
    {
        /// <summary>
        /// 获取商家profile信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route]
        public ServiceResult<ProfileDTO> Get()
        {
            var result = new ServiceResult<ProfileDTO>();
            var accountId = this.GetMerchantAccountId();

            result.Data = new ProfileComponent().GetProfile(accountId);
            
            return result;
        }

        /// <summary>
        /// 首次设置邮箱，发送邮箱验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SendSetEmailCode")]
        public ServiceResult<bool> SendSetEmailCode(SendVerifyEmailModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().SendSetEmailCode(this.GetMerchantAccountId(), model.EmailAddress);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 首次设置邮箱，验证邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifySetEmailCode")]
        public ServiceResult<bool> VerifySetEmailCode(VerifyNewEmailCode im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().VerifySetEmailCode(this.GetMerchantAccountId(), im.Code);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 首次设置邮箱，验证PIN
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifySetEmailPin")]
        public ServiceResult<bool> VerifySetEmailPin(GetPINTokenModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().VerifySetEmailPin(this.GetMerchantAccountId(), model.PIN);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 设置邮箱（首次设置）
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SetEmail")]
        public ServiceResult<bool> SetEmail()
        {
            var result = new ServiceResult<bool>();

            new ProfileComponent().SetEmail(this.GetMerchantAccountId());

            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改邮箱，发送原邮箱验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SendUpdateOriginalEmailCode")]
        public ServiceResult<bool> SendUpdateOriginalEmailCode(SendVerifyEmailModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().SendUpdateOriginalEmailCode(this.GetMerchantAccountId(), model.EmailAddress);

            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改邮箱，验证原邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateOriginalEmail")]
        public ServiceResult<bool> VerifyUpdateOriginalEmail(VerifyOriginalEmailIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().VerifyUpdateOriginalEmail(this.GetMerchantAccountId(), im.Email, im.Code);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改邮箱，发送新邮箱验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("SendUpdateNewEmailCode")]
        public ServiceResult<bool> SendUpdateNewEmailCode(SendVerifyEmailModel im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().SendUpdateNewEmailCode(this.GetMerchantAccountId(), im.EmailAddress);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改邮箱，验证新邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateNewEmailCode")]
        public ServiceResult<bool> VerifyUpdateNewEmailCode(VerifyNewEmailCode im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().VerifyNewEmail(this.GetMerchantAccountId(), im.Code);

            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改邮箱，验证PIN
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateEmailPin")]
        public ServiceResult<bool> VerifyUpdateEmailPin(GetPINTokenModel model)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            new ProfileComponent().VerifyUpdateEmailPin(this.GetMerchantAccountId(), model.PIN);

            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("UpdateEmail")]
        public ServiceResult<bool> UpdateEmail()
        {
            var result = new ServiceResult<bool>();
            
            new ProfileComponent().UpdateEmail(this.GetMerchantAccountId());

            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改手机号时，验证PIN码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("VerifyModifyCellphonePIN")]
        public ServiceResult VerifyModifyCellphonePIN(GetPINTokenModel model)
        {
            var result = new ServiceResult();
            new ProfileComponent().VerifyModifyCellphonePIN(this.GetMerchantAccountId(), model.PIN);

            return result;
        }

        /// <summary>
        /// 修改手机号时，新手机号发送验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("SendModifyCellphoneSMS")]
        public ServiceResult SendModifyCellphoneSMS(CellphoneSMSIM model)
        {
            var result = new ServiceResult();
            new ProfileComponent().SendModifyCellphoneSMS(this.GetMerchantAccountId(), model.Cellphone);

            return result;
        }

        /// <summary>
        /// 修改手机号时，新手机号校验验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyModifyCellphoneSMS")]
        public ServiceResult<string> VerifyModifyCellphoneSMS(GetSMSTokenModel model)
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
            new ProfileComponent().VerifyModifyCellphoneSMS(accountId, model.Code);

            return result;
        }

        /// <summary>
        /// 修改手机号时，新手机号校验验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns>token</returns>
        [HttpPost, Route("VerifyModifyCellphoneCombine")]
        public ServiceResult<string> VerifyModifyCellphoneCombine(CombineVerifyModel model)
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
            new ProfileComponent().VerifyModifyCellphoneCombine(accountId, model.SMSCode, model.GoogleCode);

            return result;
        }

        /// <summary>
        /// 验证验证码，修改手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ModifyCellphone")]
        public ServiceResult ModifyCellphone(ModifyMobileModel model)
        {

            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            new ProfileComponent().ModifyCellphone(accountId, model.Cellphone);

            return result;
        }
        
        /// <summary>
        /// 修改地址1
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ModifyAddress1")]
        public ServiceResult ModifyAddress1(ModifyAddressModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();
            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                Address1 = model.Address1,
                Postcode = model.Postcode,
                City = model.City,
                State = model.State
            };

            new ProfileComponent().ModifyAddress1(profile);

            
            return result;
        }

        ///// <summary>
        ///// 修改地址2
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost, Route("ModifyAddress2")]
        //public ServiceResult ModifyAddress2(ModifyAddress2Model model)
        //{
        //    var result = new ServiceResult();
        //    if (!ModelState.IsValid)
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
        //            result.Message += error + Environment.NewLine;

        //        return result;
        //    }

        //    var accountId = this.GetMerchantAccountId();
        //    var profile = new MerchantProfile
        //    {
        //        MerchantId = accountId,
        //        Address2 = model.Address2
        //    };

        //    new ProfileComponent().ModifyAddress2(profile);

        //    
        //    return result;
        //}

        /// <summary>
        /// 提交营业执照
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("CommitBusinessLicense")]
        public ServiceResult CommitBusinessLicense(CommitBusinessLicenseModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();
            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                LicenseNo = model.Number,
                BusinessLicenseImage = model.Image,
                CompanyName = model.Name
            };

            new ProfileComponent().CommitBusinessLicense(profile);

            
            return result;
        }
        /// <summary>
        /// 修改姓名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ModifyFullname")]
        public ServiceResult ModifyFullname(ModifyNameModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();
            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            new ProfileComponent().ModifyFullname(profile);

            
            return result;
        }

        /// <summary>
        /// 修改姓名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ModifyFullname")]
        public ServiceResult ModifyIdentity(ModifyIdentityModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();
            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                IdentityDocNo = model.IdentityDocNo,
                IdentityDocType = model.IdentityDocType
            };

            new ProfileComponent().ModifyIdentity(profile);

            
            return result;
        }

        /// <summary>
        /// 提交商家个人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("CommitIdentityImage")]
        public ServiceResult CommitIdentityImage(CommitIdentityModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                BackIdentityImage = model.BackIdentityImage,
                FirstName = model.FirstName,
                FrontIdentityImage = model.FrontIdentityImage,
                HandHoldWithCard = model.HandHoldWithCard,
                IdentityDocNo = model.IdentityDocNo,
                IdentityDocType = model.IdentityDocType,
                LastName = model.LastName,
                L1VerifyStatus = VerifyStatus.UnderApproval
            };

            new ProfileComponent().CommitIdentityImage(profile);

            
            return result;
        }
    }
}
