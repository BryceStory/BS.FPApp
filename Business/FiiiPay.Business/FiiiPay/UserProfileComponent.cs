using FiiiPay.DTO.Profile;
using FiiiPay.Entities;
using FiiiPay.Data;
using System;
using FiiiPay.Business.Properties;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Component.Verification;
using log4net;

namespace FiiiPay.Business
{
    public class UserProfileComponent : BaseComponent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(UserProfileComponent));

        public InfoOM Info(UserAccount user, bool isZH)
        {
            var agent = new UserProfileAgent();

            var profile = agent.GetUserProfile(user.Id);
            var country = new CountryComponent().GetById(user.CountryId);

            if (profile == null)
            {
                var userProfile = new UserProfile
                {
                    Country = user.CountryId,
                    LastName = "F" + RandomAlphaNumericGenerator.GenerateCode(8),
                    UserAccountId = user.Id,
                    Cellphone = user.Cellphone,
                    L1VerifyStatus = VerifyStatus.Uncertified,
                    L2VerifyStatus = VerifyStatus.Uncertified
                };
                var hasCreate = agent.AddProfile(userProfile);
                if (hasCreate)
                {
                    profile = userProfile;
                    _log.Info("Create profile info success. user id = " + user.Id);
                }
                else
                {
                    _log.Error("Create profile info error. user id = " + user.Id);
                }

                _log.Error("get profile info error, user id = " + user.Id);
            }

            return new InfoOM
            {
                Avatar = user.Photo,
                Birthday = profile.DateOfBirth?.ToUnixTime().ToString(),
                Cellphone = new UserAccountComponent().GetMaskedCellphone(country.PhoneCode, user.Cellphone),
                CountryName = isZH ? country.Name_CN : country.Name,
                Email = new UserAccountComponent().GetMaskedEmail(user.Email),
                FullName = (string.IsNullOrEmpty(profile.FirstName) ? "" : "* ") + profile.LastName,
                Gender = profile.Gender,
                VerifiedStatus = GetVerifiedStatus(user)
            };
        }

        public string GetVerifiedStatus(UserAccount user)
        {
            if (user.L1VerifyStatus == VerifyStatus.UnderApproval || user.L2VerifyStatus == VerifyStatus.UnderApproval)
            {
                return Resources.UnderReview;
            }

            if (user.L1VerifyStatus == VerifyStatus.Disapproval)
            {
                return string.Format(Resources.Recertification, "Lv1");
            }

            if (user.L2VerifyStatus == VerifyStatus.Disapproval)
            {
                return string.Format(Resources.Recertification, "Lv2");
            }

            if (user.L1VerifyStatus == VerifyStatus.Uncertified && user.L2VerifyStatus == VerifyStatus.Uncertified)
            {
                return Resources.ToBeVerification;
            }
            if (user.L1VerifyStatus == VerifyStatus.Certified && user.L2VerifyStatus == VerifyStatus.Uncertified)
            {
                return Resources.Lv1_Lv2;
            }
            if (user.L1VerifyStatus == VerifyStatus.Certified && user.L2VerifyStatus == VerifyStatus.Certified)
            {
                return Resources.Verified;
            }
            return Resources.ToBeVerification;
        }

        public void UpdateBirthday(UserAccount user, string date)
        {
            var c = '-';
            if (date.Contains("/"))
            {
                c = '-';
            }
            var arr = date.Split(c);
            var y = int.Parse(arr[0]);
            var m = int.Parse(arr[1]);
            var d = int.Parse(arr[2]);
            var dt = new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc);

            new UserProfileAgent().UpdateBirthday(user, dt);
        }

        public PreVerifyOM PreVerify(UserAccount user)
        {
            var om = new PreVerifyOM();

            om.Lv1Status = user.L1VerifyStatus;
            om.Lv2Status = user.L2VerifyStatus;

            return om;
        }

        public void UpdateAvatar(UserAccount user, Guid? avatar)
        {
            user.Photo = avatar;
            new UserAccountDAC().Update(user);
        }

        public PreVerifyLv1OM PreVerifyLv1(UserAccount user)
        {
            var profile = new UserProfileAgent().GetUserProfile(user.Id);

            return new PreVerifyLv1OM
            {
                BackIdentityImage = profile.BackIdentityImage,
                FrontIdentityImage = profile.FrontIdentityImage,
                Fullname = string.IsNullOrEmpty(profile.FirstName) ? "" : (profile.FirstName + " " + profile.LastName),
                HandHoldWithCard = profile.HandHoldWithCard,
                IdentityDocNo = profile.IdentityDocNo,
                IdentityDocType = (profile.IdentityDocType != IdentityDocType.IdentityCard && profile.IdentityDocType != IdentityDocType.Passport) ? IdentityDocType.IdentityCard : profile.IdentityDocType,
                FirstName = profile.FirstName,
                LastName = string.IsNullOrEmpty(profile.FirstName) ? "" : profile.LastName
            };
        }

        public void UpdateLv1Info(UserAccount user, UpdateLv1InfoIM im)
        {
            if (user.L1VerifyStatus == VerifyStatus.Certified || user.L1VerifyStatus == VerifyStatus.UnderApproval)
            {
                throw new ApplicationException(MessageResources.AccountHasBeenVerification);
            }
            var profileSDK = new UserProfileAgent();

            int count = profileSDK.GetCountByIdentityDocNo(user.Id, im.IdentityDocNo);
            if (count >= Framework.Component.Constant.IDENTITY_LIMIT)
            {
                throw new CommonException(ReasonCode.IDENTITYNO_USED_OVERLIMIT, MessageResources.IdentityUsedOverLimit);
            }

            var sr = profileSDK.UpdateLv1Info(new Lv1Info
            {
                BackIdentityImage = im.BackIdentityImage,
                FirstName = im.FirstName,
                FrontIdentityImage = im.FrontIdentityImage,
                HandHoldWithCard = im.HandHoldWithCard,
                Id = user.Id,
                IdentityDocNo = im.IdentityDocNo,
                IdentityDocType = (byte)((im.IdentityDocType != IdentityDocType.IdentityCard && im.IdentityDocType != IdentityDocType.Passport) ? IdentityDocType.IdentityCard : im.IdentityDocType),
                LastName = im.LastName,
                L1VerifyStatus = VerifyStatus.UnderApproval
            });

            if (sr)
            {
                new UserAccountDAC().UpdateL1VerfiyStatus(user.Id, (byte)VerifyStatus.UnderApproval);
            }
        }

        public PreVerifyLv2OM PreVerifyLv2(UserAccount user, bool isZH)
        {
            var profile = new UserProfileAgent().GetUserProfile(user.Id);
            var country = new CountryComponent().GetById(profile.Country.Value);
            return new PreVerifyLv2OM
            {
                Address1 = profile.Address1,
                Address2 = profile.Address2,
                City = profile.City,
                CountryName = isZH ? country.Name_CN : country.Name,
                Postcode = profile.Postcode,
                State = profile.State,
                ResidentImage = profile.ResidentImage
            };
        }

        public void UpdateLv2Info(UserAccount user, UpdateLv2InfoIM im)
        {
            var profile = new UserProfileAgent().GetUserProfile(user.Id);
            if (user.L1VerifyStatus != VerifyStatus.Certified)
            {
                throw new ApplicationException(Resources.VerificationLv1First);
            }
            if (profile.L2VerifyStatus == VerifyStatus.Certified || profile.L2VerifyStatus == VerifyStatus.UnderApproval)
            {
                throw new ApplicationException(MessageResources.AccountHasBeenVerification);
            }
            var sr = new UserProfileAgent().UpdateLv2Info(new Lv2Info
            {
                Address1 = im.Address1,
                Address2 = im.Address2,
                City = im.City,
                Id = user.Id,
                Postcode = im.Postcode,
                ResidentImage = im.ResidentImage,
                State = im.State,
                L2VerifyStatus = VerifyStatus.UnderApproval,
                Country = profile.Country.Value
            });
            if (sr)
            {
                new UserAccountDAC().UpdateL2VerfiyStatus(user.Id, (byte)VerifyStatus.UnderApproval);
            }
        }

        public void SendSetEmailCode(UserAccount user, string email)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                throw new ApplicationException(MessageResources.EmailAlredaySet);
            }
            if (new UserAccountDAC().GetByEmail(email) != null)
            {
                throw new ApplicationException(MessageResources.EmailHasBind);
            }

            string subject = Resources.VerificationCodoEmailTitle;

            SecurityVerify.SendCode(new SetEmailVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), email, subject);
        }

        public void VerifySetEmailCode(UserAccount user, string email, string code)
        {
            if (!string.IsNullOrEmpty(user.Email))
            {
                throw new ApplicationException(MessageResources.EmailAlredaySet);
            }
            if (new UserAccountDAC().GetByEmail(email) != null)
            {
                throw new ApplicationException(MessageResources.EmailHasBind);
            }

            SecurityVerify.Verify(new SetEmailVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), code);
            var model = new SetEmailVerify
            {
                EmailVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifySetEmailPin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);
            var model = SecurityVerify.GetModel<SetEmailVerify>(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.PinVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }


        /// <summary>
        /// 第一次设置邮箱
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public void SetEmail(UserAccount user, string email)
        {
            SecurityVerify.Verify<SetEmailVerify>(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.EmailVerified;
            });

            if (!string.IsNullOrEmpty(user.Email))
            {
                throw new ApplicationException(MessageResources.EmailAlredaySet);
            }
            if (new UserAccountDAC().GetByEmail(email) != null)
            {
                throw new ApplicationException(MessageResources.EmailHasBind);
            }

            new UserAccountDAC().SetEmailById(user.Id, email);
        }

        #region 验证原邮箱
        /// <summary>
        /// 发送验证原邮箱的验证码
        /// </summary>
        /// <param name="email"></param>
        public void SendUpdateOriginalEmailCode(UserAccount user, string email)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ApplicationException(MessageResources.NotBindMail);
            }
            if (user.Email != email)
            {//必须要相等
                throw new CommonException(ReasonCode.EMAIL_NOT_MATCH, MessageResources.IncorrectOriginalEmailAddress);
            }

            string subject = Resources.VerificationCodoEmailTitle;

            SecurityVerify.SendCode(new UpdateEmailOriginalVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), email, subject);
        }

        /// <summary>
        /// 验证原邮箱
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        public void VerifyOriginalEmail(UserAccount user, string email, string code)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ApplicationException(MessageResources.NotBindMail);
            }
            if (user.Email != email)
            {//必须要相等
                throw new CommonException(ReasonCode.EMAIL_NOT_MATCH, MessageResources.IncorrectOriginalEmailAddress);
            }

            SecurityVerify.Verify(new UpdateEmailOriginalVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), code);
            var model = new UpdateEmailVerify
            {
                OriginalEmailVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        /// <summary>
        /// 发送验证新邮箱的验证码
        /// </summary>
        /// <param name="email"></param>
        public void SendUpdateNewEmailCode(UserAccount user, string email)
        {
            if (user.Email == email)
                throw new CommonException(ReasonCode.ORIGIN_NEW_EMAIL_SAME, MessageResources.NewMailOldSame);

            string subject = Resources.VerificationCodoEmailTitle;
            SecurityVerify.SendCode(new UpdateEmailNewVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), email, subject);
        }

        /// <summary>
        /// 验证新邮箱
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        public void VerifyNewEmail(UserAccount user, string email, string code)
        {
            SecurityVerify.Verify(new UpdateEmailNewVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), code);
            var model = SecurityVerify.GetModel<UpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.NewEmailVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifyUpdateEmailPin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);
            var model = SecurityVerify.GetModel<UpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.PinVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        #endregion


        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="user"></param>
        /// <param name="originalCode">最初的验证码</param>
        /// <param name="email">新邮箱地址</param>
        /// <param name="code">新邮箱验证码</param>
        /// <returns></returns>
        public void UpdateEmail(UserAccount user, string email)
        {
            SecurityVerify.Verify<UpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.NewEmailVerified && model.OriginalEmailVerified;
            });
            if (user.Email == email)
            {
                throw new CommonException(ReasonCode.ORIGIN_NEW_EMAIL_SAME, MessageResources.NewMailOldSame);
            }

            var accountByEmail = new UserAccountDAC().GetByEmail(email);
            if (accountByEmail != null && accountByEmail.Id != user.Id)
            {
                throw new CommonException(ReasonCode.EMAIL_BINDBYOTHER, MessageResources.EmailHasBind);
            }
            new UserAccountDAC().SetEmailById(user.Id, email);
        }

        public void UpdateGender(UserAccount user, byte gender)
        {
            new UserProfileAgent().UpdateGender(user.Id, gender);
        }
    }
}
