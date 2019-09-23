using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiiiPay.Business.Properties;
using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Data.Agents.JPush.Model;
using FiiiPay.DTO;
using FiiiPay.DTO.Account;
using FiiiPay.DTO.Merchant;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Business.Model;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPay.Framework.MongoDB;
using MongoDB.Bson;
using static System.String;

namespace FiiiPay.Business
{
    public class UserAccountComponent : BaseComponent
    {
        public UserAccount GetById(Guid id)
        {
            return new UserAccountDAC().GetById(id);
        }

        /// <summary>
        /// 发送注册验证码
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="cellphone"></param>
        public void SendRegisterCode(int countryId, string cellphone)
        {
            if (!AccountUseable(countryId, cellphone))
            {
                throw new CommonException(ReasonCode.ACCOUNT_EXISTS, Format(MessageResources.AccountAlreadyExist, cellphone));
            }
            var country = new CountryComponent().GetById(countryId);

            SecurityVerify.SendCode(new RegisterCellphoneVerifier(), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", $"{country.PhoneCode}{cellphone}");
        }

        private bool AccountUseable(int countryId, string cellphone)
        {
            var readUserAccountDAC = new UserAccountDAC();
            var existedUserAccount = readUserAccountDAC.GetByCountryIdAndCellphone(countryId, cellphone);
            if (existedUserAccount != null)
            {
                return false;
            }
            return true;
        }

        public bool CheckRegisterSMSCode(int countryId, string cellphone, string code)
        {
            SecurityVerify.Verify(new RegisterCellphoneVerifier(), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", code, true);
            return true;
        }

        public bool H5Register(H5RegisterIM im)
        {
            if (im.Cellphone.StartsWith("170") || im.Cellphone.StartsWith("171"))
            {
                throw new CommonException(ReasonCode.PhoneNumber_Invalid, MessageResources.InvalidCellphone);
            }

            var verifier = new RegisterCellphoneVerifier();
            SecurityVerify.Verify(verifier, SystemPlatform.FiiiPay, $"{im.CountryId}:{im.Cellphone}", im.SMSCode, true);

            var accountDAC = new UserAccountDAC();

            if (!IsNullOrEmpty(im.InviterCode) && !accountDAC.ExistInviterCode(im.InviterCode))
            {
                throw new CommonException(ReasonCode.INVITORCODE_NOT_EXISTS, MessageResources.InvalidInvitation);
            }
            if (!AccountUseable(im.CountryId, im.Cellphone))
            {
                throw new CommonException(ReasonCode.ACCOUNT_EXISTS, Format(MessageResources.AccountAlreadyExist, im.Cellphone));
            }

            bool result = Register(im.CountryId, im.Cellphone, im.Password, im.InviterCode);

            if (result)
                SecurityVerify.InvalidateCode(verifier, SystemPlatform.FiiiPay, $"{im.CountryId}:{im.Cellphone}");

            return result;
        }

        public bool AppRegister(RegisterIM im)
        {
            if (im.Cellphone.StartsWith("170") || im.Cellphone.StartsWith("171"))
            {
                throw new CommonException(ReasonCode.PhoneNumber_Invalid, MessageResources.InvalidCellphone);
            }

            var verifier = new RegisterCellphoneVerifier();
            SecurityVerify.Verify(verifier, SystemPlatform.FiiiPay, $"{im.CountryId}:{im.Cellphone}", im.SMSCode, true);

            var accountDAC = new UserAccountDAC();

            if (!IsNullOrEmpty(im.InviterCode) && !accountDAC.ExistInviterCode(im.InviterCode))
            {
                throw new CommonException(ReasonCode.INVITORCODE_NOT_EXISTS, MessageResources.InvalidInvitation);
            }
            if (!AccountUseable(im.CountryId, im.Cellphone))
            {
                throw new CommonException(ReasonCode.ACCOUNT_EXISTS, Format(MessageResources.AccountAlreadyExist, im.Cellphone));
            }

            bool result = Register(im.CountryId, im.Cellphone, im.Password, im.InviterCode);

            if (result)
                SecurityVerify.InvalidateCode(verifier, SystemPlatform.FiiiPay, $"{im.CountryId}:{im.Cellphone}");

            return result;
        }

        private bool Register(int countryId, string cellphone, string password, string inviterCode)
        {
            var country = new CountryComponent().GetById(countryId);
            var accountId = Guid.NewGuid();

            var userAccount = new UserAccount
            {
                Id = accountId,
                PhoneCode = country.PhoneCode,
                Cellphone = cellphone,
                CountryId = countryId,
                IsAllowExpense = true,
                Email = null,
                IsAllowWithdrawal = true,
                IsVerifiedEmail = false,
                IsAllowTransfer = true,
                Password = PasswordHasher.HashPassword(password),
                Photo = null,
                Pin = null,
                RegistrationDate = DateTime.UtcNow,
                SecretKey = accountId.ToString().ToUpper(),
                Status = 1,
                FiatCurrency = country.FiatCurrency,
                InvitationCode = GenerateInvitationCode(),
                InviterCode = inviterCode,
                Nickname = GenerateNickname(),
                ValidationFlag = (byte)ValidationFlag.Cellphone
            };

            var userProfile = new UserProfile
            {
                Country = countryId,
                LastName = "F" + RandomAlphaNumericGenerator.GenerateCode(8),
                UserAccountId = userAccount.Id,
                Cellphone = cellphone,
                L1VerifyStatus = VerifyStatus.Uncertified,
                L2VerifyStatus = VerifyStatus.Uncertified
            };

            var accountDAC = new UserAccountDAC();
            var agent = new UserProfileAgent();
            var profileResult = agent.AddProfile(userProfile);

            if (profileResult)
            {
                try
                {
                    accountDAC.Insert(userAccount);
                }
                catch
                {
                    agent.RemoveProfile(userProfile);
                    throw;
                }

                //if (!string.IsNullOrEmpty(inviterCode))
                //{
                //    try
                //    {
                //        new InviteComponent().InsertRecord(new DTO.Invite.InviteRecordIM
                //        {
                //            InvitationCode = inviterCode,
                //            BeInvitedAccountId = userAccount.Id,
                //            Type = SystemPlatform.FiiiPay
                //        });
                //    }
                //    catch (Exception ex)
                //    {
                //        agent.RemoveProfile(userProfile);
                //        accountDAC.RemoveById(userAccount.Id);
                //        Error($"InviteComponent.InsertRecord faild:BeInvitedAccountId={userAccount.Id},InvitationCode={inviterCode},Type={SystemPlatform.FiiiPay.ToString()}", ex);
                //        throw ex;
                //    }
                //}
                return true;
            }

            throw new CommonException(ReasonCode.GENERAL_ERROR, MessageResources.NetworkError);
        }

        private string GenerateInvitationCode(string code = null)
        {
            if (IsNullOrEmpty(code))
                code = RandomAlphaNumericGenerator.GenerateCode(5);

            var accountDAC = new UserAccountDAC();
            if (accountDAC.ExistInviterCode(code))
            {
                var newCode = RandomAlphaNumericGenerator.GenerateCode(5);
                code = GenerateInvitationCode(newCode);
                return code;
            }

            return code;
        }

        public string GenerateNickname(string code = null)
        {
            string prevStr = "user_";
            if (IsNullOrEmpty(code))
                code = RandomAlphaNumericGenerator.GenerateCode(10);
            
            string nickname = prevStr + code;

            var accountDAC = new UserAccountDAC();
            if (accountDAC.ExistInviterCode(nickname))
            {
                var newCode = RandomAlphaNumericGenerator.GenerateCode(10);
                nickname = GenerateNickname(newCode);
                return nickname;
            }

            return nickname;
        }

        public LoginOM Login(LoginIM im, string deviceNumber, string ip)
        {
            var user = CheckUser(im.CountryId, im.Cellphone, im.Password);
            var isNeedGoogleVerify =
                ValidationFlagComponent.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator);

            var deviceList = new UserDeviceDAC().GetUserDeviceByAccountId(user.Id);

            var isNewDevice = deviceList.All(item => item.DeviceNumber != deviceNumber);
            if (!deviceList.Any())
            {
                if (!string.IsNullOrEmpty(user.Pin) && !user.IsBindingDevice)
                {
                    new UserDeviceDAC().Insert(new UserDevice() { DeviceNumber = deviceNumber, Name = " ", UserAccountId = user.Id, LastActiveTime = DateTime.UtcNow });

                    new UserAccountDAC().UpdateIsBindingDevice(user.Id);
                    isNewDevice = false;
                }

            }
            if ((isNewDevice && !string.IsNullOrEmpty(user.Pin)) || isNeedGoogleVerify)
            {
                return new LoginOM() { IsNeedGoogleVerify = isNeedGoogleVerify, IsNewDevice = isNewDevice, UserInfo = GetUserVerifyItems(user) };
            }

            Task.Factory.StartNew(() =>
            {
                var model = new UserLoginLog
                {
                    UserAccountId = user.Id,
                    IP = ip,
                    Timestamp = DateTime.UtcNow,
                };
                new UserLoginLogDAC().Insert(model);
            });

            return IssueAccessToken(user);
        }

        public LoginOM ValidateAuthenticator(ValidateAuthCodeIM im, string deviceNumber)
        {
            var user = CheckUser(im.CountryId, im.Cellphone, im.Password);

            var deviceList = new UserDeviceDAC().GetUserDeviceByAccountId(user.Id);

            if (!deviceList.Any())
            {
                new ApplicationException();
            }
            if (deviceList.All(item => item.DeviceNumber != deviceNumber))
            {
                new ApplicationException();
            }

            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.AuthSecretKey, im.GoogleCode);

            return IssueAccessToken(user);
        }

        public LoginOM NewDeviceLogin(NewDeviceLoginIM im, string deviceNumber)
        {
            var user = CheckUser(im.CountryId, im.Cellphone, im.Password);
            var customVerifier = new CustomVerifier("NewDeviceLogin");

            SecurityVerify.SetModel(customVerifier, SystemPlatform.FiiiPay, user.Id.ToString(), new NewDeviceLogin());

            SecurityVerify.Verify<NewDeviceLogin>(customVerifier, SystemPlatform.FiiiPay, user.Id.ToString(), (m) =>
            {
                bool result = true;
                if (user.L1VerifyStatus == VerifyStatus.Certified)
                {
                    var identityNo = new UserProfileComponent().PreVerifyLv1(user).IdentityDocNo;
                    result = result && new IDNumberVerifier().Verify(SystemPlatform.FiiiPay, user.Id.ToString(), identityNo, im.IdentityDocNo);
                    if (!result)
                    {
                        var errorCountKey = customVerifier.GetErrorCountKey(SystemPlatform.FiiiPay, user.Id.ToString());
                        var errorCount = SecurityVerify.CheckErrorCount(customVerifier, errorCountKey);
                        new IDNumberVerifier().VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount - 1);
                    }
                }
                if (!string.IsNullOrEmpty(user.Pin))
                {
                    result = result && new PinVerifier().Verify(SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, AES128.Decrypt(im.Pin, AES128.DefaultKey));
                    if (!result)
                    {
                        var errorCountKey = customVerifier.GetErrorCountKey(SystemPlatform.FiiiPay, user.Id.ToString());
                        var errorCount = SecurityVerify.CheckErrorCount(customVerifier, errorCountKey);
                        new PinVerifier().VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount - 1);
                    }
                }
                if (SecurityVerify.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator))
                {
                    var googleVerifier = new GoogleVerifier();
                    if (string.IsNullOrEmpty(im.GoogleCode))
                        result = false;
                    result = result && SecurityVerify.CheckCodeValid(googleVerifier, SystemPlatform.FiiiPay, user.Id.ToString(), im.GoogleCode);
                    result = result && googleVerifier.Verify(user.AuthSecretKey, im.GoogleCode);
                    if (!result)
                    {
                        var errorCountKey = customVerifier.GetErrorCountKey(SystemPlatform.FiiiPay, user.Id.ToString());
                        var errorCount = SecurityVerify.CheckErrorCount(customVerifier, errorCountKey);
                        googleVerifier.VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount - 1);
                    }
                }

                return result;
            });

            new UserDeviceDAC().Insert(new UserDevice() { DeviceNumber = deviceNumber, Name = " ", UserAccountId = user.Id, LastActiveTime = DateTime.UtcNow });

            return IssueAccessToken(user);
        }

        private UserAccount CheckUser(int countryId, string cellphone, string password)
        {
            var user = new UserAccountDAC().GetByCountryIdAndCellphone(countryId, cellphone);
            if (user == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, MessageResources.AccountNotFound);
            }
            var securityVerify = new SecurityVerification(SystemPlatform.FiiiPay);
            var loginErrorCountsInt = securityVerify.CheckErrorCount(SecurityMethod.Password, user.Id.ToString());
            if (user.Status == 0)
            {
                throw new CommonException(ReasonCode.ACCOUNT_DISABLED, MessageResources.AccountDisabled);
            }
            if (IsNullOrWhiteSpace(user.Password) || IsNullOrWhiteSpace(password) || !PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                securityVerify.IncreaseErrorCount(SecurityMethod.Password, user.Id.ToString());
            }
            securityVerify.DeleteErrorCount(SecurityMethod.Password, user.Id.ToString());
            return user;
        }

        private LoginOM IssueAccessToken(UserAccount user)
        {
            var keyLoginTokenPrefix = "FiiiPay:Token:";
            var keyLoginToken = $"{keyLoginTokenPrefix}{user.Id}";
            var accessToken = AccessTokenGenerator.IssueToken(user.Id.ToString());
            //new FiiiPayRedisCacheManager().SetToken(user.Id, accessToken);
            RedisHelper.StringSet(Constant.REDIS_TOKEN_DBINDEX, keyLoginToken, accessToken, TimeSpan.FromSeconds(AccessTokenGenerator.DefaultExpiryTime));
            var expiresTime = DateTime.UtcNow.AddSeconds(AccessTokenGenerator.DefaultExpiryTime);

            return new LoginOM
            {
                AccessToken = accessToken,
                ExpiresTime = expiresTime.ToUnixTime().ToString(),
                UserInfo = GetSimpleUserInfoOM(user)
            };
        }

        public void BindNoticeRegId(Guid userId, BindNoticeRegIdIM im)
        {
            var agent = new JPushAgent();

            RemoveTagsByUserId(userId);

            var tags = new List<string> { Constant.JPUSH_TAG };
            var result = agent.UpdateDeviceInfo(im.NoticeRegId, new DevicePayload
            {
                Tags = new Dictionary<string, object> {
                    { "add", tags.ToArray() }
                }
            });

            var redisKey = $"FiiiPay:Notice:UserId:{userId}";
            RedisHelper.StringSet(redisKey, im.NoticeRegId);
        }

        public void RemoveTagsByUserId(Guid userId)
        {
            var redisKey = $"FiiiPay:Notice:UserId:{userId}";
            var regId = RedisHelper.StringGet(redisKey);
            if (IsNullOrEmpty(regId))
            {
                return;
            }
            var agent = new JPushAgent();
            agent.UpdateDeviceInfo(regId, new DevicePayload
            {
                Tags = ""
            });

            RedisHelper.KeyDelete(redisKey);
        }

        public void Logout(UserAccount user)
        {
            RedisHelper.KeyDelete(Constant.REDIS_TOKEN_DBINDEX, "FiiiPay:Token:" + user.Id);
            //new FiiiPayRedisCacheManager().DeleteToken(user.Id);

            RemoveTagsByUserId(user.Id);
        }

        public SimpleUserInfoOM GetSimpleUserInfoOM(UserAccount user)
        {
            //var profile = new UserProfileAgent().GetUserProfile(user.Id);
            var country = new CountryComponent().GetById(user.CountryId);
            var fiatId = new CurrenciesDAC().GetByCode(user.FiatCurrency).ID;

            if (IsNullOrWhiteSpace(user.Nickname))
            {
                user.Nickname = GenerateNickname();
            }

            return new SimpleUserInfoOM
            {
                Avatar = user.Photo,
                Cellphone = GetMaskedCellphone(country.PhoneCode, user.Cellphone),
                //不再读取profile表
                FullName = user.Nickname,//profile == null ? "" : ((IsNullOrEmpty(profile.FirstName) ? "" : "* ") + profile.LastName),
                Nickname = user.Nickname,
                UserId = user.Id.ToString(),
                IsBaseProfileComplated = true,// profile != null,
                IsLV1Verified = user.L1VerifyStatus == VerifyStatus.Certified,
                HasSetPin = !IsNullOrEmpty(user.Pin),
                SecretKey = user.SecretKey,
                InvitationCode = user.InvitationCode,
                FiatId = fiatId,
                FiatCode = user.FiatCurrency
            };
        }

        public SimpleUserInfoOM GetUserVerifyItems(UserAccount user)
        {
            return new SimpleUserInfoOM
            {
                IsLV1Verified = user.L1VerifyStatus == VerifyStatus.Certified,
                HasSetPin = !IsNullOrEmpty(user.Pin)
            };
        }

        public string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            return phoneCode + " *******" + cellphone.Substring(Math.Max(0, cellphone.Length - 4));
        }

        public string GetMaskedEmail(string email)
        {
            if (IsNullOrEmpty(email))
            {
                return email;
            }
            var arr = email.Split('@');
            var str1 = arr[0];
            str1 = str1.Substring(0, Math.Min(2, str1.Length)) + "****";
            return str1 + (arr.Length > 1 ? arr[1] : "");
        }

        /// <summary>
        /// 发送登录验证码
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="cellphone"></param>
        public void SendLoginCode(int countryId, string cellphone)
        {
            if (AccountUseable(countryId, cellphone))
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, MessageResources.AccountNotFound);
            }

            var country = new CountryComponent().GetById(countryId);
            SecurityVerify.SendCode(new LoginCellphoneVerifier(), SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", $"{country.PhoneCode}{cellphone}");
        }

        public LoginOM LoginBySMSCode(int countryId, string cellphone, string code, string deviceNumber)
        {
            var verifier = new LoginCellphoneVerifier();
            SecurityVerify.Verify(verifier, SystemPlatform.FiiiPay, $"{countryId}:{cellphone}", code);

            var user = CheckUser(countryId, cellphone);
            var isNeedGoogleVerify =
                ValidationFlagComponent.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator);

            var deviceList = new UserDeviceDAC().GetUserDeviceByAccountId(user.Id);

            var isNewDevice = deviceList.All(item => item.DeviceNumber != deviceNumber);
            if (!deviceList.Any())
            {
                if (!string.IsNullOrEmpty(user.Pin) && !user.IsBindingDevice)
                {
                    new UserDeviceDAC().Insert(new UserDevice() { DeviceNumber = deviceNumber, Name = " ", UserAccountId = user.Id, LastActiveTime = DateTime.UtcNow });

                    new UserAccountDAC().UpdateIsBindingDevice(user.Id);
                    isNewDevice = false;
                }

            }

            if (isNeedGoogleVerify || (isNewDevice && !string.IsNullOrEmpty(user.Pin)))
            {
                string loginTypeName = isNewDevice ? "NewDeviceLogin" : "LoginBySMS";
                var model = new LoginBySMSVerify
                {
                    CellphoneVerified = true
                };
                SecurityVerify.SetModel(new CustomVerifier(loginTypeName), SystemPlatform.FiiiPay, user.Id.ToString(), model);
                return new LoginOM() { IsNeedGoogleVerify = isNeedGoogleVerify, IsNewDevice = isNewDevice, UserInfo = GetUserVerifyItems(user) };
            }

            return IssueAccessToken(user);
        }

        public LoginOM ValidateAuthenticatorBySMSCode(ValidateLoginBySMSCodeIM im, string deviceNumber)
        {
            var user = CheckUser(im.CountryId, im.Cellphone);
            var prevVerifier = new LoginCellphoneVerifier();
            var hadOpenedGoogleAuth = ValidationFlagComponent.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator);

            var deviceList = new UserDeviceDAC().GetUserDeviceByAccountId(user.Id);
            if (!deviceList.Any())
            {
                new ApplicationException();
            }
            if (deviceList.All(item => item.DeviceNumber != deviceNumber))
            {
                new ApplicationException();
            }

            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPay, user.Id.ToString(), user.AuthSecretKey, im.GoogleCode);

            SecurityVerify.Verify<LoginBySMSVerify>(new CustomVerifier("LoginBySMS"), SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
               {
                   return model.CellphoneVerified;
               });

            var loginOm = IssueAccessToken(user);

            return loginOm;
        }

        public LoginOM NewDeviceLoginBySMSCode(NewDeviceLoginBySMSCodeIM im, string deviceNumber)
        {
            var user = CheckUser(im.CountryId, im.Cellphone);
            var prevVerifier = new LoginCellphoneVerifier();
            var customVerifier = new CustomVerifier("NewDeviceLogin");

            var hadOpenedGoogleAuth = ValidationFlagComponent.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator);

            SecurityVerify.Verify<LoginBySMSVerify>(customVerifier, SystemPlatform.FiiiPay, user.Id.ToString(), (model) =>
            {
                bool result = model.CellphoneVerified;
                if (user.L1VerifyStatus == VerifyStatus.Certified)
                {
                    var identityNo = new UserProfileComponent().PreVerifyLv1(user).IdentityDocNo;
                    result = result && new IDNumberVerifier().Verify(SystemPlatform.FiiiPay, user.Id.ToString(), identityNo, im.IdentityDocNo);
                    if (!result)
                    {
                        var errorCountKey = customVerifier.GetErrorCountKey(SystemPlatform.FiiiPay, user.Id.ToString());
                        var errorCount = SecurityVerify.CheckErrorCount(customVerifier, errorCountKey);
                        new IDNumberVerifier().VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount - 1);
                    }
                }
                if (!string.IsNullOrEmpty(user.Pin))
                {
                    result = result && new PinVerifier().Verify(SystemPlatform.FiiiPay, user.Id.ToString(), user.Pin, AES128.Decrypt(im.Pin, AES128.DefaultKey));
                    if (!result)
                    {
                        var errorCountKey = customVerifier.GetErrorCountKey(SystemPlatform.FiiiPay, user.Id.ToString());
                        var errorCount = SecurityVerify.CheckErrorCount(customVerifier, errorCountKey);
                        new PinVerifier().VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount - 1);
                    }
                }
                if (SecurityVerify.CheckSecurityOpened(user.ValidationFlag, ValidationFlag.GooogleAuthenticator))
                {
                    var googleVerifier = new GoogleVerifier();
                    if (string.IsNullOrEmpty(im.GoogleCode))
                        result = false;
                    result = result && SecurityVerify.CheckCodeValid(googleVerifier, SystemPlatform.FiiiPay, user.Id.ToString(), im.GoogleCode);
                    result = result && googleVerifier.Verify(user.AuthSecretKey, im.GoogleCode);
                    if (!result)
                    {
                        var errorCountKey = customVerifier.GetErrorCountKey(SystemPlatform.FiiiPay, user.Id.ToString());
                        var errorCount = SecurityVerify.CheckErrorCount(customVerifier, errorCountKey);
                        googleVerifier.VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount - 1);
                    }
                }

                return result;
            });

            new UserDeviceDAC().Insert(new UserDevice() { DeviceNumber = deviceNumber, Name = " ", UserAccountId = user.Id, LastActiveTime = DateTime.UtcNow });

            var loginOm = IssueAccessToken(user);
            return loginOm;
        }

        private UserAccount CheckUser(int countryId, string cellphone)
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
            return user;
        }

        public List<UserSecretES> ListAllSecretKeys()
        {
            return new UserAccountDAC().ListAllSecretKeys();
        }

        public void ChangeLanguage(Guid userId, string language = "en")
        {
            RedisHelper.StringSet(Constant.REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{userId}", language);
        }

        public string GetLanguage(Guid userId)
        {
            var lang = RedisHelper.StringGet(Constant.REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{userId}");
            return lang ?? "en";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="im">邀请码</param>
        public CellPhoneOM GetCellPhoneInfo(string im)
        {
            var account = new UserAccountDAC().GetUserAccountByInviteCode(im);
            if (account == null)
            {
                throw new CommonException(ReasonCode.INVITORCODE_NOT_EXISTS, MessageResources.InvalidInvitation);
            }
            return new CellPhoneOM() { PhoneRegion = account.PhoneCode, CellPhone = account.Cellphone };
        }

        public void SettingFiatCurrency(Guid accountId, string fiatCurrency)
        {
            new UserAccountDAC().SettingFiatCurrency(accountId, fiatCurrency);
        }

        public GetListByCodeListOM GetListByCodeList(string codes)
        {
            //LogHelper.Info("GetListByCodeList = " + codes);

            try
            {
                var codeInfoList = new TokenAgent().GetMerchantIntoListByCodes(codes, false);
                var idList = codeInfoList.Select(a => a.MerchantId).ToList();
                var merchantAccountList = new MerchantAccountDAC().GetListByIdList(idList).Where(e => e.POSId != null).ToList();
                //var profileList = GetMerchanstProfileInfo(merchantAccountList);

                var list = new List<GetListByCodeListOMItem>();
                foreach (var merchantAccount in merchantAccountList)
                {
                    //var profile = profileList.FirstOrDefault(e => e.MerchantId == merchantAccount.Id);
                    var codeInfo = codeInfoList.FirstOrDefault(b => b.MerchantId == merchantAccount.Id);
                    list.Add(new GetListByCodeListOMItem
                    {
                        Id = merchantAccount.Id.ToString(),
                        MerchantAccount = GetMasked(merchantAccount.Username),
                        MerchantName = merchantAccount.MerchantName,
                        IconUrl = merchantAccount.Photo,
                        IsAllowAcceptPayment = merchantAccount.IsAllowAcceptPayment && (merchantAccount.Status == AccountStatus.Active),
                        L1VerifyStatus = (byte)merchantAccount.L1VerifyStatus,
                        L2VerifyStatus = (byte)merchantAccount.L2VerifyStatus,
                        RandomCode = codeInfo?.MerchantCode
                    });
                }

                return new GetListByCodeListOM
                {
                    List = list
                };
            }
            catch (Exception exception)
            {
                Error(exception);
            }

            return null;
        }

        private List<MerchantVerifyStatusOM> GetMerchanstProfileInfo(List<MerchantAccount> list)
        {
            var groupIdDic = list.GroupBy(e => e.CountryId).ToDictionary(e => e.Key, e => e.Select(a => a.Id).ToList());

            var resultList = new List<MerchantVerifyStatusOM>();

            foreach (var kv in groupIdDic)
            {
                var result = new UserProfileAgent().GetVerifyStatusListByIds(kv.Key, kv.Value);
                if (result != null)
                {
                    resultList.AddRange(result);
                }
            }
            return resultList;
        }

        private string GetMasked(string str)
        {
            if (IsNullOrEmpty(str))
            {
                return "";
            }

            return str[0] + "*****" + str.Substring(Math.Max(0, str.Length - 4));
        }

        public BonusMessageOM UnBindingMerchantMessage(long id)
        {
            var record = new POSMerchantBindRecordDAC().GetById(id);
            var merchant = new MerchantAccountDAC().GetById(record.MerchantId);

            return new BonusMessageOM()
            {
                Title = Resources.RewardUnbindFiiiposTitle,
                Content = Format(Resources.RewardUnbindFiiiposSubTitle, merchant.MerchantName),
                Timestamp = record.UnbindTime?.ToUnixTime().ToString()
            };
        }

        public BonusMessageOM InviteFiiiposSuccessMessage(long id)
        {
            var record = new POSMerchantBindRecordDAC().GetById(id);
            var merchant = new MerchantAccountDAC().GetById(record.MerchantId);

            return new BonusMessageOM()
            {
                Title = Resources.InviteFiiiposSuccessTitle,
                Content = Format(Resources.InviteFiiiposSuccessSubTitle, merchant.MerchantName),
                Timestamp = record.BindTime.ToUnixTime().ToString()
            };
        }

        public void ChangeLanguagetoDb(Guid accountId, string language)
        {
            new UserAccountDAC().UpdateLanguage(accountId, language);
            new UserAccountComponent().ChangeLanguage(accountId, language);
            //new FiiiPayRedisCacheManager().ChangeLanguage(accountId, language);
        }


        public GetLastActiveCountryOM GetLastActiveCountry(Guid accountId)
        {
            var model = MongoDBHelper.FindSingleIndex<AccountActiveCountry>(item => item.AccountId == accountId);

            return new GetLastActiveCountryOM()
            {
                CountryId = model?.CountryId ?? -1,
                Country_CN = model?.Country_CN,
                Country_EN = model?.Country_EN,
                IsFirst = model == null
            };
        }

        public void SetActiveCountry(Guid accountId, SetActiveCountryIM im)
        {
            var model = MongoDBHelper.FindSingleIndex<AccountActiveCountry>(item => item.AccountId == accountId);
            if (model == null)
            {
                MongoDBHelper.AddSignleObject(new AccountActiveCountry()
                {
                    _id = ObjectId.GenerateNewId(),
                    AccountId = accountId,
                    CountryId = im.CountryId,
                    Country_CN = im.Country_CN,
                    Country_EN = im.Country_EN,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                });
            }
            else
            {
                MongoDBHelper.ReplaceOne<AccountActiveCountry>(item => item.AccountId == accountId,
                    new AccountActiveCountry()
                    {
                        _id = model._id,
                        AccountId = model.AccountId,
                        CountryId = im.CountryId,
                        Country_CN = im.Country_CN,
                        Country_EN = im.Country_EN,
                        CreateTime = model.CreateTime,
                        UpdateTime = DateTime.Now
                    });
            }
        }

        public void UpdateNickname(Guid accountId, string nickname)
        {
            var userAccountDAC = new UserAccountDAC();

            userAccountDAC.UpdateNickname(accountId, nickname);
        }
    }
}
