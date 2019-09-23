using System;
using System.Collections.Generic;
using FiiiPay.Business;
using FiiiPay.Data;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.Verification;
using FiiiPOS.Business.Properties;

namespace FiiiPOS.Business.FiiiPOS
{
    public class SecurityComponent : BaseComponent
    {
        public void FiiiPOSSendSecurityValidateCellphoneCode(Guid merchantId,string code)
        {
            MerchantAccount merchant = new MerchantAccountDAC().GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            string fullCellphone = $"{merchant.PhoneCode}{merchant.Cellphone}";
            SecurityVerify.SendCode(new MandatoryCellphoneVerifier(), SystemPlatform.FiiiPOS, code + merchant.Id.ToString(), fullCellphone);
        }

        public string FiiiPOSVerfiyPinReturnToken(Guid accountId, string pin)
        {
            MerchantAccount merchant = new MerchantAccountDAC().GetById(accountId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            FiiiPOSVerifyPin(merchant, pin);
            return new SecurityVerification(SystemPlatform.FiiiPOS).GenegeToken(accountId, SecurityMethod.Pin);
        }

        public void VerifyResetPinCombine(Guid accountId, string idNumber, string smsCode, string googleCode)
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
            MerchantProfile profile = new MerchantProfileAgent().GetMerchantProfile(accountId);
            if (profile != null && profile.L1VerifyStatus == FiiiPay.Entities.Enums.VerifyStatus.Certified)
            {
                options.Add(new CombinedVerifyOption { AuthType = (byte)ValidationFlag.IDNumber, Code = idNumber });
                userSecrets.IdentityNo = profile.IdentityDocNo;
            }

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, accountId.ToString(), userSecrets, options);

            var model = new ResetPinVerify { CombinedVerified = true };
            SecurityVerify.SetModel(new CustomVerifier("ResetPin"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void ResetPIN(Guid accountId, string pin)
        {
            SecurityVerify.Verify<ResetPinVerify>(new CustomVerifier("ResetPin"), SystemPlatform.FiiiPOS, accountId.ToString(), (model) =>
            {
                return model.CombinedVerified;
            });

            MerchantAccountDAC mad = new MerchantAccountDAC();
            MerchantAccount account = mad.GetById(accountId);
            if (PasswordHasher.VerifyHashedPassword(account.PIN, pin))
                throw new CommonException(ReasonCode.PIN_MUST_BE_DIFFERENT, Resources.新PIN不能与原PIN一样);
            MerchantAccount ma = new MerchantAccount()
            {
                Id = accountId,
                PIN = PasswordHasher.HashPassword(pin)
            };
            mad.UpdatePIN(ma);
        }

        public void FiiiPOSVerifyPin(MerchantAccount account, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), account.PIN, pin);
        }
    }
}
