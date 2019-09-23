using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.Data.Agents.APP;
using FiiiPay.DTO.Security;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Business
{
    public class SecurityComponent : BaseComponent
    {
        public void SetPin(string pin, UserAccount user, string deviceNumber)
        {
            if (!string.IsNullOrEmpty(user.Pin))
            {
                throw new ApplicationException(MessageResources.PINHasSet);
            }
            var _pin = AES128.Decrypt(pin, AES128.DefaultKey);
            using (var scope = new TransactionScope())
            {
                new UserAccountDAC().SetPinById(user.Id, PasswordHasher.HashPassword(_pin));
                if (new UserDeviceDAC().GetUserDeviceByAccountId(user.Id)
                    .All(item => item.DeviceNumber != deviceNumber))
                {
                    new UserDeviceDAC().Insert(new UserDevice()
                    {
                        DeviceNumber = deviceNumber,
                        LastActiveTime = DateTime.UtcNow,
                        Name = " ",
                        UserAccountId = user.Id
                    });
                }
                new UserAccountDAC().UpdateIsBindingDevice(user.Id);
                scope.Complete();
            }
            
        }

        /// <summary>
        /// 发送忘记密码验证码
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="cellphone"></param>
        public void SendForgotPasswordCode(int countryId, string cellphone)
        {
            var user = new UserAccountDAC().GetByCountryIdAndCellphone(countryId, cellphone);
            if (user == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, MessageResources.AccountNotFound);
            }
            if (user.Status == 0)
            {
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, MessageResources.AccountDisabled);
            }

            var country = new CountryComponent().GetById(countryId);
            SecurityVerify.SendCode(new ForgetPasswordCellphoneVerifier(), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", $"{country.PhoneCode}{cellphone}");
        }

        public void VerifyForgotPasswordCode(int countryId, string cellphone, string code)
        {
            SecurityVerify.Verify(new ForgetPasswordCellphoneVerifier(), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", code);
            var model = new ResetPasswordVerify
            {
                CellphoneVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("ForgotPassword"), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", model);
        }

        public void ForgotPassword(int countryId, string cellphone, string password)
        {
            SecurityVerify.Verify<ResetPasswordVerify>(new CustomVerifier("ForgotPassword"), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", (model) =>
            {
                return model.CellphoneVerified;
            });

            var user = new UserAccountDAC().GetByCountryIdAndCellphone(countryId, cellphone);

            if (PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                throw new CommonException(ReasonCode.PWD_MUST_BE_DIFFERENT, MessageResources.NewPasswordHasUse);
            }
            
            new UserAccountComponent().Logout(user);

            user.Password = PasswordHasher.HashPassword(password);
            new UserAccountDAC().Update(user);
            
            new SecurityVerification(SystemPlatform.FiiiPay).DeleteErrorCount(SecurityMethod.Password, user.Id.ToString());
        }

        public void VerifyUpdatePasswordPin(UserAccount user, string code)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, code);

            var model = new UpdatePasswordVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UpdatePassword"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void UpdatePassword(UserAccount user, string password, string oldPassword)
        {
            SecurityVerify.Verify<UpdatePasswordVerify>(new CustomVerifier("UpdatePassword"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                var result = PasswordHasher.VerifyHashedPassword(user.Password, oldPassword);
                return result && model.PinVerified;
            });

            if (PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                throw new CommonException(ReasonCode.PWD_MUST_BE_DIFFERENT, MessageResources.NewPasswordHasUse);
            }

            new UserAccountDAC().SetPasswordById(user.Id, PasswordHasher.HashPassword(password));
        }

        public void VerifyUpdatePinPin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);

            var model = new UpdatePinVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UpdatePin"),SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifyUpdatePinCombine(UserAccount user, string smsCode, string googleCode)
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

            var model = SecurityVerify.GetModel<UpdatePinVerify>(new CustomVerifier("UpdatePin"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdatePin"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void UpdatePin(UserAccount user, string oldPin, string newPin)
        {
            SecurityVerify.Verify<UpdatePinVerify>(new CustomVerifier("UpdatePin"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            newPin = AES128.Decrypt(newPin, AES128.DefaultKey);
            var _oldPin = AES128.Decrypt(oldPin, AES128.DefaultKey);

            if (_oldPin.Equals(newPin))
            {
                throw new CommonException(ReasonCode.PIN_MUST_BE_DIFFERENT, MessageResources.NewPINOldPINDifferent);
            }
            new UserAccountDAC().SetPinById(user.Id, PasswordHasher.HashPassword(newPin));
        }

        public void SendSecurityValidateCellphoneCode(UserAccount user,string code)
        {
            var country = new CountryComponent().GetById(user.CountryId);
            SecurityVerify.SendCode(new MandatoryCellphoneVerifier(), SystemPlatform.FiiiPay, code + user.Id, $"{country.PhoneCode}{user.Cellphone}");
        }

        public void VerifyUpdateCellphonePin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);
            var model = new UpdateCellphoneVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UpdateCellphone"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void SendUpdateCellphoneNewCode(UserAccount user, string newCellphone)
        {
            if (new UserAccountDAC().GetByCountryIdAndCellphone(user.CountryId, newCellphone) != null)
            {
                throw new CommonException(ReasonCode.PhoneNumber_Exist, MessageResources.MobilePhoneHasReg);
            }

            var country = new CountryComponent().GetById(user.CountryId);
            SecurityVerify.SendCode(new UpdateCellphoneNewVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), $"{country.PhoneCode}{newCellphone}");
        }

        public void VerifyUpdateCellphoneNewCode(UserAccount user, VerifyUpdateCellphoneNewCodeIM im)
        {
            SecurityVerify.Verify(new UpdateCellphoneNewVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), im.Code);
            
            var model = SecurityVerify.GetModel<UpdateCellphoneVerify>(new CustomVerifier("UpdateCellphone"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.NewCellphoneVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdateCellphone"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void VerifyUpdateCellphoneCombine(UserAccount user, string smsCode, string googleCode)
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

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPay, user.Id.ToString(), userSecrets, options);
            
            var model = SecurityVerify.GetModel<UpdateCellphoneVerify>(new CustomVerifier("UpdateCellphone"), SystemPlatform.FiiiPay, user.Id.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UpdateCellphone"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void UpdateCellphone(UserAccount user, string cellphone)
        {
            SecurityVerify.Verify<UpdateCellphoneVerify>(new CustomVerifier("UpdateCellphone"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.NewCellphoneVerified;
            });

            if (new UserAccountDAC().GetByCountryIdAndCellphone(user.CountryId, cellphone) != null)
            {
                throw new ApplicationException(MessageResources.MobilePhoneHasReg);
            }
            
            user.Cellphone = cellphone;
            var sr = new UserProfileAgent().UpdatePhoneNumber(user.Id, cellphone);
            if (sr)
            {
                UserAccountDAC accountDAC = new UserAccountDAC();
                sr = accountDAC.UpdatePhoneNumber(user.Id, cellphone);
            }
            new UserAccountComponent().Logout(user);
        }

        public PreFindPinBackOM PreFindPinBack(UserAccount user)
        {
            //var profile = new UserProfileAgent().GetUserProfile(user.Id);

            return new PreFindPinBackOM
            {
                IsLv1Verified = user.L1VerifyStatus == VerifyStatus.Certified
            };
        }

        public void VerifyPin(UserAccount user, string pin)
        {
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, pin);
        }

        public void VerifyResetPinCombine(Guid accountId, string idNumber, string smsCode, string googleCode)
        {
            UserAccount user = new UserAccountDAC().GetById(accountId);
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
            UserProfile profile = new UserProfileAgent().GetUserProfile(user.Id);
            if (profile != null && profile.L1VerifyStatus == VerifyStatus.Certified)
            {
                options.Add(new CombinedVerifyOption { AuthType = (byte)ValidationFlag.IDNumber, Code = idNumber });
                userSecrets.IdentityNo = profile.IdentityDocNo;
            }

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPay, user.Id.ToString(), userSecrets, options);

            var model = new ResetPinVerify() { CombinedVerified = true };
            SecurityVerify.SetModel(new CustomVerifier("ResetPin"), SystemPlatform.FiiiPay, user.Id.ToString(), model);
        }

        public void ResetPIN(Guid accountId, string pin)
        {
            SecurityVerify.Verify<ResetPinVerify>(new CustomVerifier("ResetPin"), SystemPlatform.FiiiPay, accountId.ToString(), (model) =>
            {
                return model.CombinedVerified;
            });
            UserAccount user = new UserAccountDAC().GetById(accountId);
            if (PasswordHasher.VerifyHashedPassword(user.Pin, pin))
                throw new CommonException(ReasonCode.PIN_MUST_BE_DIFFERENT, MessageResources.NewPINOldPINDifferent);
            UserAccountDAC mad = new UserAccountDAC();
            mad.SetPinById(accountId, PasswordHasher.HashPassword(pin));
            SecurityVerify.DeleteErrorCount(new PinVerifier(),SystemPlatform.FiiiPay, accountId.ToString());
        }
    }
}
