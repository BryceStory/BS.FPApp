using System;
using System.Collections.Generic;
using FiiiPay.Business;
using FiiiPay.Data;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;

namespace FiiiPOS.Business.FiiiPOS
{
    public class AuthenticatorComponent
    {
        public SetupCode GenerateMerchantSecretKey(Guid accountId)
        {
            var merchant = new MerchantAccountDAC().GetById(accountId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            var country = new CountryComponent().GetById(merchant.CountryId);
            var username = merchant.Username + "@FiiiPOS";
            var result = new GoogleAuthenticator().GenerateSetupCode(SystemPlatform.FiiiPOS, username);
            return result;
        }

        public void VerifyBindPin(Guid accountId, string pin)
        {
            var merchant = new MerchantAccountDAC().GetById(accountId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, merchant.Id.ToString(), merchant.PIN, pin);

            var model = new BindGoogleAuth
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void VerifyBindGoogleAuth(Guid accountId, string secretKey, string code)
        {
            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), secretKey, code);
            var model = SecurityVerify.GetModel<BindGoogleAuth>(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPOS, accountId.ToString());
            model.GoogleVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void VerifyBindCombine(Guid accountId, string smsCode)
        {
            var merchant = new MerchantAccountDAC().GetById(accountId);
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = merchant.ValidationFlag
            };

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, merchant.Id.ToString(), userSecrets, options, null);

            var model = SecurityVerify.GetModel<BindGoogleAuth>(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void BindMerchantAccount(BindMerchantAuthIM im, Guid merchantId)
        {
            SecurityVerify.Verify<BindGoogleAuth>(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPOS, merchantId.ToString(), (model) =>
            {
                return model.PinVerified && model.GoogleVerified && model.CombinedVerified;
            });

            var mDAC = new MerchantAccountDAC();
            var merchant = mDAC.GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }

            if (string.IsNullOrEmpty(merchant.AuthSecretKey))
            {
                var oldFlag = merchant.ValidationFlag;
                var newFlag = ValidationFlagComponent.AddValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
                mDAC.UpdateGoogleAuthencator(merchant.Id, im.SecretKey, newFlag);
            }
            else
                mDAC.SetAuthSecretById(merchant.Id, im.SecretKey);
        }

        public void VerifyUnBindPin(Guid merchantId, string pin)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, merchant.Id.ToString(), merchant.PIN, pin);

            var model = new UnBindGoogleAuth
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void VerifyUnBindCombine(Guid merchantId, string smsCode, string googleCode)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
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

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, merchant.Id.ToString(), userSecrets, options, null);

            var model = SecurityVerify.GetModel<UnBindGoogleAuth>(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void UnBindMerchantAccount(Guid merchantId)
        {
            SecurityVerify.Verify<UnBindGoogleAuth>(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPOS, merchantId.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            var mDAC = new MerchantAccountDAC();
            var merchant = mDAC.GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }

            string secretKey = merchant.AuthSecretKey;
            var oldFlag = merchant.ValidationFlag;
            var newFlag = ValidationFlagComponent.ReduceValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
            mDAC.UpdateGoogleAuthencator(merchant.Id, null, newFlag);
        }

        public void VerifyOpenGoogleAuth(Guid merchantId, string code)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPOS, merchant.Id.ToString(), merchant.AuthSecretKey, code);
            var model = new OpenGoogleAuth { GoogleVerified = true };
            SecurityVerify.SetModel(new CustomVerifier("OpenGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void OpenMerchantAccount(Guid merchantId)
        {
            SecurityVerify.Verify<OpenGoogleAuth>(new CustomVerifier("OpenGoogleAuth"), SystemPlatform.FiiiPOS, merchantId.ToString(), (model) =>
            {
                return model.GoogleVerified;
            });
            var mDAC = new MerchantAccountDAC();
            var merchant = mDAC.GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }

            var oldFlag = merchant.ValidationFlag;
            var newFlag = ValidationFlagComponent.AddValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
            mDAC.UpdateGoogleAuthencator(merchant.Id, newFlag);
        }

        public void VerifyClosePin(Guid merchantId, string pin)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, merchant.Id.ToString(), merchant.PIN, pin);

            var model = new CloseGoogleAuth
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void VerifyCloseCombine(Guid merchantId, string smsCode, string googleCode)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
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

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, merchant.Id.ToString(), userSecrets, options, null);

            var model = SecurityVerify.GetModel<CloseGoogleAuth>(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void CloseMerchantAccount(Guid merchantId)
        {
            SecurityVerify.Verify<CloseGoogleAuth>(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPOS, merchantId.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            var mDAC = new MerchantAccountDAC();
            var merchant = mDAC.GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }

            var oldFlag = merchant.ValidationFlag;
            var newFlag = ValidationFlagComponent.ReduceValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
            mDAC.UpdateGoogleAuthencator(merchant.Id, newFlag);
        }

        public string ValidateMerchantAccount(Guid merchantId, string code)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            var secretKey = merchant.AuthSecretKey;
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new CommonException(ReasonCode.NOT_BIND_AUTHENTICATOR, Resources.尚未绑定谷歌验证);
            }
            FiiiPOSValidateGoogleAuthencator(merchant.Id, merchant.AuthSecretKey, code);
            return new SecurityVerification(SystemPlatform.FiiiPOS).GenegeToken(merchant.Id, SecurityMethod.GoogleAuthencator);
        }

        public void FiiiPOSValidateGoogleAuthencator(Guid accountId, string secretKey, string code)
        {
            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), secretKey, code);
        }

        public GetOpenedSecuritiesOM GetMerchantOpenedSecurities(Guid merchantId)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            GetOpenedSecuritiesOM entity = new GetOpenedSecuritiesOM();
            entity.IsOpenedAuthencator = ValidationFlagComponent.CheckSecurityOpened(merchant.ValidationFlag, ValidationFlag.GooogleAuthenticator);
            entity.CellPhone = GetMaskedCellphone(merchant.PhoneCode, merchant.Cellphone);
            return entity;
        }
        public GetStatusOfSecurityOM GetMerchantStatusOfSecurity(Guid merchantId)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            GetStatusOfSecurityOM entity = new GetStatusOfSecurityOM();
            entity.GoogleAuthenticator = new SecurityStatus
            {
                HasBinded = !string.IsNullOrEmpty(merchant.AuthSecretKey),
                HasOpened = ValidationFlagComponent.CheckSecurityOpened(merchant.ValidationFlag, ValidationFlag.GooogleAuthenticator)
            };
            return entity;
        }

        public string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }
    }
}
