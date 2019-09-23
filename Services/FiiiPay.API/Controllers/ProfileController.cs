using System.Web.Http;
using FiiiPay.DTO.Profile;
using FiiiPay.Framework;
using FiiiPay.Business;
using System.Linq;
using System;
using FiiiPay.API.Models;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.ProfileController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Profile")]
    public class ProfileController : ApiController
    {
        /// <summary>
        /// Informations this instance.
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Info")]
        public ServiceResult<InfoOM> Info()
        {
            var data = new UserProfileComponent().Info(this.GetUser(), this.IsZH());
            data.Nickname = this.GetUser().Nickname;
            return new ServiceResult<InfoOM>
            {
                Data = data
            };
        }

        /// <summary>
        /// Updates the nickname.
        /// </summary>
        /// <param name="im">The im.</param>
        /// <returns></returns>
        [HttpPost, Route("UpdateNickname")]
        public ServiceResult<bool> UpdateNickname(UpdateNicknameModel im)
        {
            new UserAccountComponent().UpdateNickname(this.GetUser().Id, im.Nickname);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// Updates the birthday.
        /// </summary>
        /// <param name="im">The im.</param>
        /// <returns></returns>
        [HttpPost, Route("UpdateBirthday")]
        public ServiceResult<bool> UpdateBirthday(UpdateBirthdayIM im)
        {
            new UserProfileComponent().UpdateBirthday(this.GetUser(), im.Date);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 去认证前，需要展示Lv1和Lv2的认证状态，通过这个接口获取两个状态
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreVerify")]
        public ServiceResult<PreVerifyOM> PreVerify()
        {
            return new ServiceResult<PreVerifyOM>
            {
                Data = new UserProfileComponent().PreVerify(this.GetUser())
            };
        }

        /// <summary>
        /// Pres the verify LV1.
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreVerifyLv1")]
        public ServiceResult<PreVerifyLv1OM> PreVerifyLv1()
        {
            return new ServiceResult<PreVerifyLv1OM>
            {
                Data = new UserProfileComponent().PreVerifyLv1(this.GetUser())
            };
        }

        /// <summary>
        /// 修改Lv1信息
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateLv1Info")]
        public ServiceResult<bool> UpdateLv1Info(UpdateLv1InfoIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Data = false;
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new UserProfileComponent().UpdateLv1Info(this.GetUser(), im);
            result.Data = true;
            
            return result;
        }

        /// <summary>
        /// Pres the verify LV2.
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreVerifyLv2")]
        public ServiceResult<PreVerifyLv2OM> PreVerifyLv2()
        {
            return new ServiceResult<PreVerifyLv2OM>
            {
                Data = new UserProfileComponent().PreVerifyLv2(this.GetUser(), this.IsZH())
            };
        }

        /// <summary>
        /// 修改Lv2信息
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateLv2Info")]
        public ServiceResult<bool> UpdateLv2Info(UpdateLv2InfoIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Data = false;
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new UserProfileComponent().UpdateLv2Info(this.GetUser(), im);

            result.Data = true;
            
            return result;
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateAvatar")]
        public ServiceResult<bool> UpdateAvatar(UpdateAvatarIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new UserProfileComponent().UpdateAvatar(this.GetUser(), im.Avatar);

            result.Data = true;
            return result;
        }
        
        /// <summary>
        /// 首次设置邮箱，发送邮箱验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("SendSetEmailCode")]
        public ServiceResult<bool> SendSetEmailCode(SendSetEmailCodeIM im)
        {
            new UserProfileComponent().SendSetEmailCode(this.GetUser(), im.Email);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 首次设置邮箱，验证邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifySetEmailCode")]
        public ServiceResult<bool> VerifySetEmailCode(VerifyOriginalEmailIM im)
        {
            new UserProfileComponent().VerifySetEmailCode(this.GetUser(), im.Email, im.Code);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 首次设置邮箱，验证PIN
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifySetEmailPin")]
        public ServiceResult<bool> VerifySetEmailPin(VerifyPinModel model)
        {
            new UserProfileComponent().VerifySetEmailPin(this.GetUser(), model.PIN);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 设置邮箱（首次设置）
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("SetEmail")]
        public ServiceResult<bool> SetEmail(SetEmailIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new UserProfileComponent().SetEmail(this.GetUser(), im.Email);

            result.Data = true;
            
            return result;
        }

        /// <summary>
        /// 修改邮箱，发送修改邮箱[原邮箱]的验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("SendUpdateOriginalEmailCode")]
        public ServiceResult<bool> SendUpdateOriginalEmailCode(OriginalEmailIM im)
        {
            new UserProfileComponent().SendUpdateOriginalEmailCode(this.GetUser(), im.Email);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 修改邮箱，验证原邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyOriginalEmail")]
        public ServiceResult<bool> VerifyOriginalEmail(VerifyOriginalEmailIM im)
        {
            new UserProfileComponent().VerifyOriginalEmail(this.GetUser(), im.Email,im.Code);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 修改邮箱，发送修改邮箱[新邮箱]的验证码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("SendUpdateNewEmailCode")]
        public ServiceResult<bool> SendUpdateNewEmailCode(OriginalEmailIM im)
        {
            new UserProfileComponent().SendUpdateNewEmailCode(this.GetUser(), im.Email);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 修改邮箱，验证新邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyNewEmailCode")]
        public ServiceResult<bool> VerifyNewEmailCode(VerifyOriginalEmailIM im)
        {
            new UserProfileComponent().VerifyNewEmail(this.GetUser(), im.Email, im.Code);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 修改邮箱，验证PIN
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("VerifyUpdateEmailPin")]
        public ServiceResult<bool> VerifyUpdateEmailPin(VerifyPinModel model)
        {
            new UserProfileComponent().VerifyUpdateEmailPin(this.GetUser(), model.PIN);
            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateEmail")]
        public ServiceResult<bool> UpdateEmail(UpdateEmailIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            //追加原邮箱验证码验证
            new UserProfileComponent().UpdateEmail(this.GetUser(), im.Email);

            result.Data = true;
            return result;
        }

        /// <summary>
        /// 修改性别
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateGender")]
        public ServiceResult<bool> UpdateGender(UpdateGenderIM im)
        {
            var result = new ServiceResult<bool>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            new UserProfileComponent().UpdateGender(this.GetUser(), im.Gender);

            result.Data = true;
            return result;
        }
    }
}
