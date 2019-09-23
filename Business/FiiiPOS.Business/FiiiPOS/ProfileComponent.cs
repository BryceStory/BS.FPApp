using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;
using log4net;

namespace FiiiPOS.Business.FiiiPOS
{
    public class ProfileComponent
    {
        public ProfileDTO GetProfile(Guid accountId)
        {
            var dac = new MerchantAccountDAC();
            var agent = new MerchantProfileAgent();

            var account = dac.GetById(accountId);
            var profile = agent.GetMerchantProfile(accountId);
            var pos = new POSDAC().GetById(account.POSId.Value);
            var country = new CountryComponent().GetById(account.CountryId);

            var result = new ProfileDTO
            {
                MerchantAccount = account.Username,
                LastName = profile.LastName,
                FirstName = profile.FirstName,
                IdentityDocNo = profile.IdentityDocNo,
                IdentityDocType = (profile.IdentityDocType != IdentityDocType.IdentityCard && profile.IdentityDocType != IdentityDocType.Passport) ? IdentityDocType.IdentityCard : profile.IdentityDocType,
                FrontIdentityImage = profile.FrontIdentityImage,
                BackIdentityImage = profile.BackIdentityImage,
                HandHoldWithCard = profile.HandHoldWithCard,
                MerchantName = account.MerchantName,
                CompanyName = profile?.CompanyName,
                Email = account.Email,
                Cellphone = $"{account.PhoneCode} {account.Cellphone}",
                PosSn = pos.Sn,
                Country = country.Name,
                L1VerifyStatus = (int)(profile?.L1VerifyStatus ?? 0),
                L2VerifyStatus = (int)(profile?.L2VerifyStatus ?? 0),
                Address1 = profile?.Address1,
                Address2 = profile?.Address2,
                Postcode = profile?.Postcode,
                City = profile?.City,
                State = profile?.State
            };
            return result;
        }

        public void SendSetEmailCode(Guid accountId, string email)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            if (!string.IsNullOrEmpty(account.Email))
            {
                throw new ApplicationException();
            }
            if (new MerchantAccountDAC().GetByEmail(email) != null)
            {
                throw new CommonException(ReasonCode.EMAIL_BINDBYOTHER, Resources.此邮箱已经绑定到其他邮箱);
            }

            string subject = Resources.验证码邮箱标题;

            SecurityVerify.SendCode(new SetEmailVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), email, subject);
            var model = new FiiiPosSetEmailVerify
            {
                Email = email
            };
            SecurityVerify.SetModel(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPOS, account.Id.ToString(), model);
        }

        public void VerifySetEmailCode(Guid accountId, string code)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            if (!string.IsNullOrEmpty(account.Email))
            {
                throw new ApplicationException();
            }

            SecurityVerify.Verify(new SetEmailVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), code);
            var model = SecurityVerify.GetModel<FiiiPosSetEmailVerify>(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPOS, account.Id.ToString());
            model.EmailVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPOS, account.Id.ToString(), model);
        }

        public void VerifySetEmailPin(Guid accountId, string pin)
        {
            var account = new MerchantAccountDAC().GetById(accountId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), account.PIN, pin);
            var model = SecurityVerify.GetModel<FiiiPosSetEmailVerify>(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPOS, account.Id.ToString());
            model.PinVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPOS, account.Id.ToString(), model);
        }

        public void SetEmail(Guid accountId)
        {
            string emailAddress = "";
            SecurityVerify.Verify<FiiiPosSetEmailVerify>(new CustomVerifier("SetEmail"), SystemPlatform.FiiiPOS, accountId.ToString(), (model) =>
            {
                emailAddress = model.Email;
                return model.PinVerified && model.EmailVerified;
            });
            new MerchantAccountDAC().UpdateEmail(accountId, emailAddress);
        }

        /// <summary>
        /// 发送验证原邮箱的验证码
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="emailAddress"></param>
        public void SendUpdateOriginalEmailCode(Guid accountId, string emailAddress)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            if (account.Email != emailAddress)
                throw new CommonException(ReasonCode.EMAIL_NOT_MATCH, Resources.原邮箱地址不正确);

            string subject = Resources.验证码邮箱标题;

            SecurityVerify.SendCode(new UpdateEmailOriginalVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), emailAddress, subject);
        }

        public void VerifyUpdateOriginalEmail(Guid accountId, string emailAddress, string code)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);
            if (account.Email != emailAddress)
                throw new CommonException(ReasonCode.EMAIL_NOT_MATCH, Resources.原邮箱地址不正确);

            SecurityVerify.Verify(new UpdateEmailOriginalVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), code);
            var model = new FiiiPosUpdateEmailVerify
            {
                OriginalEmailVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void SendUpdateNewEmailCode(Guid accountId, string emailAddress)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            if (account.Email == emailAddress)
                throw new CommonException(ReasonCode.ORIGIN_NEW_EMAIL_SAME, Resources.新邮箱不能和原来的邮箱一致);

            var accountByEmail = dac.GetByEmail(emailAddress);
            if (accountByEmail != null && accountByEmail.Id != accountId)
            {
                throw new CommonException(ReasonCode.EMAIL_BINDBYOTHER, Resources.此邮箱已经绑定到其他邮箱);
            }

            string subject = Resources.验证码邮箱标题;
            SecurityVerify.SendCode(new UpdateEmailNewVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), emailAddress, subject);
            var model = SecurityVerify.GetModel<FiiiPosUpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, accountId.ToString());
            model.NewEmail = emailAddress;
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void VerifyNewEmail(Guid accountId, string code)
        {
            SecurityVerify.Verify(new UpdateEmailNewVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), code);
            var model = SecurityVerify.GetModel<FiiiPosUpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, accountId.ToString());
            model.NewEmailVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void VerifyUpdateEmailPin(Guid accountId, string pin)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), account.PIN, pin);
            var model = SecurityVerify.GetModel<FiiiPosUpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, account.Id.ToString());
            model.PinVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, account.Id.ToString(), model);
        }

        public void UpdateEmail(Guid accountId)
        {
            string emailAddress = "";
            SecurityVerify.Verify<FiiiPosUpdateEmailVerify>(new CustomVerifier("UpdateEmail"), SystemPlatform.FiiiPOS, accountId.ToString(), (model) =>
            {
                emailAddress = model.NewEmail;
                return model.PinVerified && model.OriginalEmailVerified && model.NewEmailVerified;
            });

            new MerchantAccountDAC().UpdateEmail(accountId, emailAddress);
        }

        public void VerifyModifyCellphonePIN(Guid accountId, string pin)
        {
            MerchantAccount merchant = new MerchantAccountDAC().GetById(accountId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), merchant.PIN, pin);

            var model = new ModifyCellphoneVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("ModifyCellphone"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }
        public void SendModifyCellphoneSMS(Guid accountId, string cellphone)
        {
            MerchantAccount account = new MerchantAccountDAC().GetById(accountId);
            Country country = new CountryComponent().GetById(account.CountryId);
            //加上区号
            cellphone = $"{country.PhoneCode}{cellphone}";

            SecurityVerify.SendCode(new ModifyCellphoneVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), cellphone);
        }

        public void VerifyModifyCellphoneSMS(Guid accountId, string smsCode)
        {
            SecurityVerify.Verify(new ModifyCellphoneVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), smsCode, true);
            var model = SecurityVerify.GetModel<ModifyCellphoneVerify>(new CustomVerifier("ModifyCellphone"), SystemPlatform.FiiiPOS, accountId.ToString());
            model.NewCellphoneVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("ModifyCellphone"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void VerifyModifyCellphoneCombine(Guid accountId, string smsCode, string googleCode)
        {
            MerchantAccount merchant = new MerchantAccountDAC().GetById(accountId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode },
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.GooogleAuthenticator, Code=googleCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = merchant.ValidationFlag,
                GoogleAuthSecretKey = merchant.AuthSecretKey
            };
            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, accountId.ToString(), userSecrets, options);

            var model = SecurityVerify.GetModel<ModifyCellphoneVerify>(new CustomVerifier("ModifyCellphone"), SystemPlatform.FiiiPOS, accountId.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("ModifyCellphone"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void ModifyCellphone(Guid accountId, string cellphone)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            SecurityVerify.Verify<ModifyCellphoneVerify>(new CustomVerifier("ModifyCellphone"), SystemPlatform.FiiiPOS, account.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.NewCellphoneVerified && model.CombinedVerified;
            });

            if (account.Cellphone == cellphone)
                throw new CommonException(10000, Resources.新手机号码不能与原来的一致);

            //修改手机号
            dac.UpdateCellphone(accountId, cellphone);

            var agent = new MerchantProfileAgent();
            var profile = new MerchantProfile
            {
                MerchantId = accountId,
                Cellphone = cellphone,
                Country = account.CountryId
            };
            agent.UpdateCellphone(profile);

        }

        public void ModifyAddress1(MerchantProfile profile)
        {
            var agent = new MerchantProfileAgent();

            agent.ModifyAddress1(profile);
        }
        public void ModifyFullname(MerchantProfile profile)
        {
            var agent = new MerchantProfileAgent();

            agent.ModifyFullname(profile);
        }
        public void ModifyIdentity(MerchantProfile profile)
        {
            var agent = new MerchantProfileAgent();

            //需求改变，FIIIPOS的KYC无需证件号限制
            //int count = agent.GetCountByIdentityDocNo(profile.MerchantId, profile.IdentityDocNo);
            //if (count >= Constant.IDENTITY_LIMIT)
            //{
            //    throw new CommonException(ReasonCode.IDENTITYNO_USED_OVERLIMIT, Resources.IdentityUsedOverLimit);
            //}

            agent.ModifyIdentity(profile);
        }

        public void CommitBusinessLicense(MerchantProfile profile)
        {
            var agent = new MerchantProfileAgent();
            var sr = agent.CommitBusinessLicense(profile);
            if (sr)
            {
                new MerchantAccountDAC().UpdateL2VerfiyStatus(profile.MerchantId, (byte)VerifyStatus.UnderApproval);
            }
        }
        public void CommitIdentityImage(MerchantProfile profile)
        {
            var agent = new MerchantProfileAgent();
            var sr = agent.CommitIdentityImage(profile);
            if (sr)
            {
                new MerchantAccountDAC().UpdateL1VerfiyStatus(profile.MerchantId, (byte)VerifyStatus.UnderApproval);
            }
        }

        public bool ValidateLv1(Guid MerchantId)
        {
            var agent = new MerchantProfileAgent();
            var account = agent.GetMerchantProfile(MerchantId);
            return account.L1VerifyStatus == VerifyStatus.Certified;
        }
    }
}