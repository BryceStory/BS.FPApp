using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Verification;
using FiiiPOS.Web.Business.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.Framework.Component.Verification;

namespace FiiiPOS.Web.Business
{
    public class ProfileComponent
    {
        public async Task SendVerifyEmail(Guid accountId, string emailAddress)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            if (account.Email == emailAddress)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAIL_MUST_BE_DIFFERENT, "新邮箱不能和原来的邮箱一致");

            var accountByEmail = dac.GetByEmail(emailAddress);
            if (accountByEmail != null && accountByEmail.Id != accountId)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAIL_BINDED, "该邮箱已绑定到其他账户");
            }

            string key = $"{RedisKeys.FiiiPOS_WEB_EmailVerification}:{accountId}";
            string code = RandomAlphaNumericGenerator.GenerateAllNumber(6);

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"AccountId", accountId.ToString()},
                {"EmailAddress", emailAddress},
                {"Code", code}
            };

            RedisHelper.Set(key, dic, new TimeSpan(0, Constant.EMAIL_EXPIRED_TIME, 0));

            string subject = Resources.验证码邮箱标题;
            string content = string.Format(Resources.验证码邮箱模版, code, Constant.EMAIL_EXPIRED_TIME);

            EmailAgent agent = new EmailAgent();
            await agent.SendAsync(emailAddress, subject, content, 5);
        }

        /// <summary>
        /// 发送原邮箱验证码 20180523
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="emailAddress"></param>
        public async Task SendVerifyOriginalEmail(Guid accountId, string emailAddress)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            var accountByEmail = dac.GetByEmail(emailAddress);
            if (accountByEmail != null && accountByEmail.Id != accountId)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAIL_BINDED, "该邮箱已绑定到其他账户");
            }

            string key = $"{RedisKeys.FiiiPOS_WEB_EmailVerification}:{accountId}";
            string code = RandomAlphaNumericGenerator.GenerateAllNumber(6);

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                {"AccountId", accountId.ToString()},
                {"EmailAddress", emailAddress},
                {"Code", code}
            };

            RedisHelper.Set(key, dic, new TimeSpan(0, Constant.EMAIL_EXPIRED_TIME, 0));

            string subject = Resources.验证码邮箱标题;
            string content = string.Format(Resources.验证码邮箱模版, code, Constant.EMAIL_EXPIRED_TIME);

            EmailAgent agent = new EmailAgent();
            await agent.SendAsync(emailAddress, subject, content, 5);
        }

        /// <summary>
        /// 验证原邮箱验证码
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="emailAddress"></param>
        /// <param name="code"></param>
        public void VerifyOriginalEmail(Guid accountId, string emailAddress, string code)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            string key = $"{RedisKeys.FiiiPOS_WEB_EmailVerification}:{accountId}";
            Dictionary<string, string> dic = RedisHelper.Get<Dictionary<string, string>>(key);

            if (dic == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAIL_CODE_EXPIRE, "验证码已过期");
            if (dic["Code"] != code || dic["EmailAddress"] != emailAddress)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.WRONG_EMAIL_CODE_ENTERRED, "验证码有误");

        }

        public void ModifyEmail(Guid accountId, string code, string token)
        {
            string originalKey = $"{RedisKeys.FiiiPOS_WEB_EmailVerification}:{accountId}";

            string key = $"{RedisKeys.FiiiPOS_WEB_EmailVerification}:{accountId}";
            Dictionary<string, string> dic = RedisHelper.Get<Dictionary<string, string>>(key);

            if (dic == null)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAIL_CODE_EXPIRE, "验证码已过期");
            if (dic["Code"] != code)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.WRONG_EMAIL_CODE_ENTERRED, "验证码有误");

            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);
            //SecurityVerify.Verify<PinVerifier>(SystemPlatform.FiiiPOSWeb, FiiipayBusiness.Common, accountId.ToString(), token, account.PIN);
            dac.UpdateEmail(accountId, dic["EmailAddress"]);

            RedisHelper.KeyDelete(key);
        }

        public void ModifyCellphone(Guid accountId, string cellphone, string smsToken, string token, string gToken)
        {
            //
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            if (account.Cellphone == cellphone)
                throw new CommonException(ReasonCode.FiiiPosReasonCode.CELLPHONE_MUST_BE_DIFFERENT, "新手机号码不能与原来的一致");

            //验证短信码
            SecurityVerification sv = new SecurityVerification(SystemPlatform.FiiiPOS);
            sv.VerifyToken(accountId, smsToken, SecurityMethod.CellphoneCode);

            //验证pin码
            sv.VerifyToken(accountId, token, SecurityMethod.Pin);

            //验证google token 20180521
            new SecurityVerification(SystemPlatform.FiiiPOS).VerifyToken(accountId, gToken, SecurityMethod.SecurityValidate);

            //修改手机号
            dac.UpdateCellphone(accountId, cellphone);
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
            var merchant = new MerchantAccountDAC().GetById(profile.MerchantId);
            if (merchant.L2VerifyStatus != VerifyStatus.Uncertified && merchant.L2VerifyStatus != VerifyStatus.Disapproval)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.COMMITTED_STATUS, "该状态下不能提交审核。");
            }
            var sr = agent.CommitBusinessLicense(profile);
            if (sr)
            {
                new MerchantAccountDAC().UpdateL2VerfiyStatus(profile.MerchantId, (byte)VerifyStatus.UnderApproval);
            }
        }
        public void CommitIdentityImage(MerchantProfile profile)
        {
            var agent = new MerchantProfileAgent();
            var merchant = new MerchantAccountDAC().GetById(profile.MerchantId);

            if (merchant.L1VerifyStatus != VerifyStatus.Uncertified && merchant.L1VerifyStatus != VerifyStatus.Disapproval)
            {
                throw new CommonException(ReasonCode.FiiiPosReasonCode.COMMITTED_STATUS, "该状态下不能提交审核。");
            }
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
