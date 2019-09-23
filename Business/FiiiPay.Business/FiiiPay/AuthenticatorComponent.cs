using FiiiPay.Data;
using FiiiPay.DTO.Authenticator;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using System;
using FiiiPay.Business.Properties;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Verification;
using System.Collections.Generic;
using FiiiPay.Foundation.Data;

namespace FiiiPay.Business
{
    public class AuthenticatorComponent : BaseComponent
    {
        public GetOpenedSecuritiesOM GetUserOpenedSecurities(UserAccount user)
        {
            var country = new CountryDAC().GetById(user.CountryId);
            GetOpenedSecuritiesOM entity = new GetOpenedSecuritiesOM();
            entity.IsOpenedAuthencator = ValidationFlagComponent.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator);
            entity.CellPhone = new UserAccountComponent().GetMaskedCellphone(country.PhoneCode, user.Cellphone);
            return entity;
        }
        public GetStatusOfSecurityOM GetUserStatusOfSecurity(UserAccount user)
        {
            GetStatusOfSecurityOM entity = new GetStatusOfSecurityOM();
            entity.GoogleAuthenticator = new SecurityStatus
            {
                HasBinded = !string.IsNullOrEmpty(user.AuthSecretKey),
                HasOpened = ValidationFlagComponent.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator)
            };
            return entity;
        }

        public SetupCode GenerateUserSecretKey(UserAccount user)
        {
            var country = new CountryDAC().GetById(user.CountryId);
            var username = country.PhoneCode + user.Cellphone + "@FiiiPay";
            var result = new GoogleAuthenticator().GenerateSetupCode(SystemPlatform.FiiiPay, username);
            return result;
        }

        public void VerifyBindPin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);

            var model = new BindGoogleAuth
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifyBindGoogleAuth(Guid accountId, string secretKey, string code)
        {
            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPay, accountId.ToString(), secretKey, code);
            var model = SecurityVerify.GetModel<BindGoogleAuth>(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPay, accountId.ToString());
            model.GoogleVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPay, accountId.ToString(), model);
        }

        public void VerifyBindCombine(UserAccount user, string smsCode)
        {
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = user.ValidationFlag
            };

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPay, user.Id.ToString(), userSecrets, options, null);

            var model = SecurityVerify.GetModel<BindGoogleAuth>(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void BindUserAccount(BindUserAuthIM im, UserAccount user)
        {
            SecurityVerify.Verify<BindGoogleAuth>(new CustomVerifier("BindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.GoogleVerified && model.CombinedVerified;
            });

            var userDAC = new UserAccountDAC();
            if (string.IsNullOrEmpty(user.AuthSecretKey))
            {
                var oldFlag = user.ValidationFlag;
                var newFlag = ValidationFlagComponent.AddValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
                userDAC.UpdateGoogleAuthencator(user.Id, im.SecretKey, newFlag);
            }
            else
                userDAC.SetAuthSecretById(user.Id, im.SecretKey);
        }

        public void VerifyUnBindPin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);

            var model = new UnBindGoogleAuth
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifyUnBindCombine(UserAccount user, string smsCode, string googleCode)
        {
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode },
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.GooogleAuthenticator, Code=googleCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = user.ValidationFlag,
                GoogleAuthSecretKey = user.AuthSecretKey
            };

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPay, user.Id.ToString(), userSecrets, options, null);

            var model = SecurityVerify.GetModel<UnBindGoogleAuth>(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void UnBindUserAccount(UserAccount user)
        {
            SecurityVerify.Verify<UnBindGoogleAuth>(new CustomVerifier("UnBindGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            var userDAC = new UserAccountDAC();
            var oldFlag = user.ValidationFlag;
            var newFlag = ValidationFlagComponent.ReduceValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
            userDAC.UpdateGoogleAuthencator(user.Id, null, newFlag);
        }

        public void VerifyOpenGoogleAuth(UserAccount user,string code)
        {
            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.AuthSecretKey, code);
            var model = new OpenGoogleAuth { GoogleVerified = true };
            SecurityVerify.SetModel(new CustomVerifier("OpenGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void OpenUserAccount(UserAccount user)
        {
            SecurityVerify.Verify<OpenGoogleAuth>(new CustomVerifier("OpenGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.GoogleVerified;
            });

            var userDAC = new UserAccountDAC();
            var oldFlag = user.ValidationFlag;
            var newFlag = ValidationFlagComponent.AddValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
            userDAC.UpdateGoogleAuthencator(user.Id, newFlag);
        }

        public void VerifyClosePin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);

            var model = new CloseGoogleAuth
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifyCloseCombine(UserAccount user, string smsCode, string googleCode)
        {
            List<CombinedVerifyOption> options = new List<CombinedVerifyOption>
            {
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.Cellphone, Code=smsCode },
                new CombinedVerifyOption{ AuthType=(byte)ValidationFlag.GooogleAuthenticator, Code=googleCode }
            };
            UserSecrets userSecrets = new UserSecrets
            {
                ValidationFlag = user.ValidationFlag,
                GoogleAuthSecretKey = user.AuthSecretKey
            };

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPay, user.Id.ToString(), userSecrets, options, null);

            var model = SecurityVerify.GetModel<CloseGoogleAuth>(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void CloseUserAccount(UserAccount user)
        {
            SecurityVerify.Verify<CloseGoogleAuth>(new CustomVerifier("CloseGoogleAuth"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            var userDAC = new UserAccountDAC();
            var oldFlag = user.ValidationFlag;
            var newFlag = ValidationFlagComponent.ReduceValidationFlag(oldFlag, ValidationFlag.GooogleAuthenticator);
            userDAC.UpdateGoogleAuthencator(user.Id, newFlag);
        }

        public string ValidateUserAccount(UserAccount user, string code)
        {
            var secretKey = user.AuthSecretKey;
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new CommonException(ReasonCode.NOT_BIND_AUTHENTICATOR, MessageResources.NotVerifiyGA);
            }
            FiiiPayValidateGoogleAuthencator(user.Id, user.AuthSecretKey, code);
            return new SecurityVerification(SystemPlatform.FiiiPay).GenegeToken(user.Id, SecurityMethod.GoogleAuthencator);
        }

        public void FiiiPayValidateGoogleAuthencator(Guid accountId, string secretKey, string code, SecurityMethod method = SecurityMethod.GoogleAuthencator)
        {
            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPay, accountId.ToString(), secretKey, code);
        }

        public UserAccount Token(string authHeader, string lang)
        {
            var accessToken = AccessTokenGenerator.DecryptToken(authHeader);

            var cacheKeyPrefix = "FiiiPay:Token:";
            var cacheKey = $"{cacheKeyPrefix}{accessToken.Identity}";
            var cacheToken = RedisHelper.StringGet(Constant.REDIS_TOKEN_DBINDEX, cacheKey);

            //var cacheManager = new FiiiPayRedisCacheManager();
            //var cacheToken = cacheManager.GetToken(accessToken.Identity);

            if (string.IsNullOrEmpty(cacheToken))
                throw new AccessTokenExpireException();

            if (authHeader != cacheToken)
            {
                throw new UnauthorizedException();
            }

            var id = Guid.Parse(accessToken.Identity);
            var account = new UserAccountComponent().GetById(id);
            //var account = cacheManager.GetUserAccount(accessToken.Identity);
            if (account == null)
            {
                throw new UnauthorizedException();
            }

            if (account.Status == 0)
            {
                //已经禁用的用户，删除token
                RedisHelper.KeyDelete(Constant.REDIS_TOKEN_DBINDEX, cacheKey);
                //cacheManager.DeleteToken(accessToken.Identity);
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, MessageResources.AccountDisabled);
            }

            RedisHelper.StringSet(Constant.REDIS_TOKEN_DBINDEX, cacheKey, cacheToken,
                TimeSpan.FromSeconds(AccessTokenGenerator.DefaultExpiryTime));
            if (!string.IsNullOrEmpty(lang))
                new UserAccountComponent().ChangeLanguage(account.Id, lang);

            //account.Id = Guid.Parse(accessToken.Identity);
            //account.Language = lang;
            //cacheManager.ExpireToken(accessToken.Identity);
            //cacheManager.ChangeLanguage(accessToken.Identity, lang);

            return account;
        }
    }
}
