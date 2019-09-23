using FiiiPay.Business;
using FiiiPay.Data;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using FiiiPay.Data.Agents;
using FiiiPay.Data.Agents.JPush.Model;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework.Queue;
using FiiiPay.Framework.Component.Verification;

namespace FiiiPOS.Business.FiiiPOS
{
    public class MerchantAccountComponent
    {
        private const int FiiiPOSTokenDbIndex = 11;

        public bool HasBoundAccount(string sn)
        {
            var result = new MerchantAccountDAC().IsExist(sn);
            return result;
        }

        public MechantSimpleInfoDTO GetMerchantSimpleInfoBySn(string sn)
        {
            MerchantAccount account = new MerchantAccountDAC().GetByPosSn(sn);
            if (account == null)
                return null;
            return new MechantSimpleInfoDTO
            {
                Username = account.Username.Substring(0, 1) + "****",
                Avatar = account.Photo
            };
        }

        public MerchantAccount GetMerchantAccountBySN(string sn)
        {
            MerchantAccount account = new MerchantAccountDAC().GetByPosSn(sn);
            if (account == null)
                return null;

            return account;
        }

        public void SendSignupSMS(string cellphone, int countryId, string possn)
        {
            var country = new CountryComponent().GetById(countryId);
            if (country == null)
                throw new CommonException(10000, Resources.国家不存在);

            var posDac = new POSDAC();

            var pos = posDac.GetBySn(possn);

            if (pos == null)
                throw new GeneralException(Resources.SN码不存在);
            if (pos.Status)
                throw new GeneralException(Resources.POSHasBoundOtherAccount);

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "Cellphone",cellphone},
                { "CountryId",countryId.ToString()},
                { "FiatCurrency",country.FiatCurrency},
                { "PhoneCode",country.PhoneCode}
            };

            var verifier = new FiiiPosRegisterVerifier();
            SecurityVerify.SendCode(verifier, SystemPlatform.FiiiPOS, $"{countryId}{cellphone}", $"{country.PhoneCode}{cellphone}");
            verifier.CacheRegisterModel(SystemPlatform.FiiiPOS, $"{countryId}{cellphone}", dic);
        }

        public void VerificationSMSCode(int countryId, string cellphone, string code)
        {
            SecurityVerify.Verify(new FiiiPosRegisterVerifier(), SystemPlatform.FiiiPOS, $"{countryId}{cellphone}", code);
            var model = new FiiiPosSignUpVerify
            {
                CellphoneVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("FiiiPosSignUp"), SystemPlatform.FiiiPOS, $"{countryId}:{cellphone}", model);
        }

        public void VerifyAccount(string merchantAccount, string merchantName, string invitationCode)
        {
            MerchantAccount account = new MerchantAccountDAC().GetByUsername(merchantAccount);
            if (account != null)
                throw new CommonException(ReasonCode.ACCOUNT_EXISTS, Resources.帐号已存在);
            if (!string.IsNullOrEmpty(invitationCode) && !new UserAccountDAC().ExistInviterCode(invitationCode))
                throw new CommonException(ReasonCode.INVITORCODE_NOT_EXISTS, Resources.邀请码不存在);
        }

        public SignonDTO Signup(int countryId, string cellphone, string merchantAccount, string merchantName, string posSn, string invitationCode, string pin)
        {
            SecurityVerify.Verify<FiiiPosSignUpVerify>(new CustomVerifier("FiiiPosSignUp"), SystemPlatform.FiiiPOS, $"{countryId}:{cellphone}", (model) =>
            {
                return model.CellphoneVerified;
            });

            var country = new CountryComponent().GetById(countryId);
            if (country == null)
                throw new CommonException(10000, Resources.国家不存在);
            var cacheKey = $"{countryId}{cellphone}";
            var verifier = new FiiiPosRegisterVerifier();
            var dic = verifier.GetRegisterModel(SystemPlatform.FiiiPOS, cacheKey, false);

            MerchantAccount excistAccount = new MerchantAccountDAC().GetByUsername(merchantAccount);
            if (excistAccount != null)
                throw new CommonException(ReasonCode.ACCOUNT_EXISTS, Resources.帐号已存在);

            UserAccount inviterAccount = new UserAccount();
            if (!string.IsNullOrEmpty(invitationCode))
            {
                inviterAccount = new UserAccountDAC().GetUserAccountByInviteCode(invitationCode);
                if (inviterAccount == null)
                    throw new CommonException(ReasonCode.INVITORCODE_NOT_EXISTS, Resources.邀请码不存在);
            }

            var posDac = new POSDAC();
            var pos = posDac.GetInactivedBySn(posSn);
            if (pos == null)
                throw new CommonException(ReasonCode.POSSN_ERROR, Resources.SN码不存在);

            var merchantMS = new MasterSettingDAC().SelectByGroup("Merchant");

            Guid beInvitedAccountId = Guid.NewGuid();
            MerchantAccount account = new MerchantAccount
            {
                CountryId = countryId,
                Cellphone = cellphone,
                Username = merchantAccount,
                MerchantName = merchantName,
                PIN = PasswordHasher.HashPassword(pin),
                Id = beInvitedAccountId,
                POSId = pos.Id,
                IsVerifiedEmail = false,
                PhoneCode = dic["PhoneCode"],
                RegistrationDate = DateTime.UtcNow,
                Status = AccountStatus.Active,
                SecretKey = beInvitedAccountId.ToString(),
                FiatCurrency = dic["FiatCurrency"],
                Markup = Convert.ToDecimal(merchantMS.First(e => e.Name == "Merchant_Markup").Value),
                Receivables_Tier = Convert.ToDecimal(merchantMS.First(e => e.Name == "Merchant_TransactionFee").Value),
                //默认开启手机验证
                ValidationFlag = (byte)ValidationFlag.Cellphone,
                InvitationCode = invitationCode
            };

            POSMerchantBindRecord posBindRecord = new POSMerchantBindRecord
            {
                POSId = pos.Id,
                SN = pos.Sn,
                MerchantId = account.Id,
                MerchantUsername = merchantAccount,
                BindTime = DateTime.UtcNow,
                BindStatus = (byte)POSBindStatus.Binded
            };

            MerchantProfile profile = new MerchantProfile
            {
                MerchantId = account.Id,
                Country = account.CountryId,
                Cellphone = account.Cellphone,
                L1VerifyStatus = VerifyStatus.Uncertified,
                L2VerifyStatus = VerifyStatus.Uncertified
            };

            MerchantProfileAgent agent = new MerchantProfileAgent();
            bool addProfileResult = agent.AddMerchant(profile);
            if (!addProfileResult)
                throw new CommonException(ReasonCode.GENERAL_ERROR, "Add merchant profile error.");
            int recordId = default(int);
            try
            {
                using (var scope = new TransactionScope())
                {
                    posDac.ActivePOS(pos);
                    new MerchantAccountDAC().Insert(account);
                    recordId = new POSMerchantBindRecordDAC().Insert(posBindRecord);
                    if (!string.IsNullOrEmpty(invitationCode))
                        BindInviter(posSn, beInvitedAccountId, inviterAccount.Id, invitationCode);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                agent.RemoveMerchantById(profile);

                //LogHelper.Error(ex);
                throw;
            }

            if (!string.IsNullOrEmpty(invitationCode))
                RabbitMQSender.SendMessage("InvitePosBindSuccess", new Tuple<Guid, long>(inviterAccount.Id, recordId));

            verifier.DeleteCacheModel(SystemPlatform.FiiiPOS, cacheKey);

            return GetAccessToken(pos, account);
        }

        public SignonDTO Signon(string possn, string merchantAccount, string pin)
        {
            var account = new MerchantAccountDAC().GetByUsername(merchantAccount);
            // 账号不存在
            if (account == null)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.AccountNotExists);
            // 账号未绑定到POS机
            if (!account.POSId.HasValue)
                throw new CommonException(ReasonCode.ACCOUNT_UNBUNDLED, Resources.AccountNotExists);
            if (account.Status == AccountStatus.Locked)
                throw new CommonException(ReasonCode.ACCOUNT_LOCKED, Resources.帐号已锁定);

            var pos = new POSDAC().GetBySn(possn);
            // 不存在和未激活提示SN异常
            if (pos == null || !pos.Status)
                throw new CommonException(ReasonCode.POSSN_ERROR, Resources.SN码不存在);
            if (account.POSId != pos.Id)
                throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.NoBindRelationship);

            new SecurityComponent().FiiiPOSVerifyPin(account, pin);

            return GetAccessToken(pos, account);
        }

        private SignonDTO GetAccessToken(POS pos, MerchantAccount account)
        {
            MerchantAccessToken token = new MerchantAccessToken
            {
                POSSN = pos.Sn,
                Identity = account.Username
            };

            string accessToken = AccessTokenGenerator.IssueToken(token);
            string key = $"{RedisKeys.FiiiPOS_APP_MerchantId}:{account.Username}";
            RedisHelper.StringSet(Constant.REDIS_TOKEN_DBINDEX, key, accessToken);

            return new SignonDTO
            {
                AccessToken = accessToken,
                SecretKey = account.SecretKey
            };
        }

        public bool BindNoticeRegId(Guid accountId, BindNoticeRegIdIM im)
        {
            RemoveRegInfoByUserId(accountId);

            var agent = new JPushAgent();

            var tags = new List<string> { Constant.JPUSH_TAG };

            agent.UpdateDeviceInfo(im.NoticeRegId, new DevicePayload
            {
                Tags = new Dictionary<string, object> {
                    { "add", tags.ToArray() }
                }
            });

            string keyOfNotice = $"{RedisKeys.FiiiPOS_APP_Notice_MerchantId}:{accountId}";
            RedisHelper.StringSet(keyOfNotice, im.NoticeRegId);

            return true;
        }

        private void RemoveRegInfoByUserId(Guid userId)
        {
            string redisKey = $"{RedisKeys.FiiiPOS_APP_Notice_MerchantId}:{userId}";
            RedisHelper.KeyDelete(redisKey);
        }

        public void SignOut(Guid merchantId)
        {
            string key = $"{RedisKeys.FiiiPOS_APP_MerchantId}:{merchantId}";
            RedisHelper.KeyDelete(Constant.REDIS_TOKEN_DBINDEX, key);
            string keyOfNotice = $"{RedisKeys.FiiiPOS_APP_Notice_MerchantId}:{merchantId}";
            RedisHelper.KeyDelete(keyOfNotice);
        }


        //public string GetByPosSn(string sn)
        //{
        //    MerchantAccountDAC dac = new MerchantAccountDAC();

        //    MerchantAccount account = dac.GetByPosSn(sn);
        //    return account?.Username;
        //}
        public MerchantAccount GetByPosSn(string posSn, string merchantAccount)
        {
            MerchantAccount account = new MerchantAccountDAC().GetByUsername(merchantAccount);
            if (account == null)
                throw new CommonException(ReasonCode.UNAUTHORIZED, "UNAUTHORIZED");
            if (!account.POSId.HasValue)
                throw new CommonException(ReasonCode.ACCOUNT_UNBUNDLED, Resources.AccountUnbundled);

            if (account.Status == AccountStatus.Locked)
                throw new CommonException(ReasonCode.ACCOUNT_LOCKED, "Account is locked");

            POS pos = new POSDAC().GetBySn(posSn);
            if (pos == null || pos.Id != account.POSId)
                throw new CommonException(ReasonCode.POSSN_ERROR, Resources.SN码不存在);
            return account;
        }

        public MerchantAccount GetById(Guid accountId)
        {
            MerchantAccountDAC dac = new MerchantAccountDAC();

            MerchantAccount account = dac.GetById(accountId);
            return account;
        }

        public MerchantAccountDTO GetMerchantInfoById(Guid accountId)
        {
            MerchantAccount account = new MerchantAccountDAC().GetById(accountId);
            MerchantProfile profile = new MerchantProfileAgent().GetMerchantProfile(accountId);

            //LogHelper.Info("GetMerchantInfoById " + accountId);

            var token = Generate(account.SecretKey, account.RegistrationDate);
            string key = string.Format(RedisKeys.FiiiPOS_APP_BroadcastNo, token);
            var result = RedisHelper.StringGet(FiiiPOSTokenDbIndex, key);
            if (string.IsNullOrWhiteSpace(result))
            {
                RedisHelper.StringSet(FiiiPOSTokenDbIndex, key, accountId.ToString());
            }

            bool isMiningEnabled = false;
            if (account.POSId.HasValue)
            {
                POS pos = new POSDAC().GetById(account.POSId.Value);
                isMiningEnabled = pos?.IsMiningEnabled ?? false;
            }

            return new MerchantAccountDTO
            {
                Id = account.Id,
                Cellphone = CellphoneExtension.GetMaskedCellphone(account.PhoneCode, account.Cellphone),
                Username = account.Username,
                MerchantName = account.MerchantName,
                Status = account.Status,
                Email = account.Email,
                CountryId = account.CountryId,
                IsAllowWithdrawal = account.IsAllowWithdrawal,
                IsAllowAcceptPayment = account.IsAllowAcceptPayment,
                FiatCurrency = account.FiatCurrency,
                Receivables_Tier = account.Receivables_Tier,
                Markup = account.Markup,
                ValidationFlag = account.ValidationFlag,
                Lv1VerifyStatus = profile.L1VerifyStatus,
                Type = account.WithdrawalFeeType,
                IsMiningEnabled = isMiningEnabled,
                BroadcastNo = token
            };
        }

        private static string Generate(string secretKey, DateTime date)
        {
            date = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();

            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var counter = (long)date.Subtract(unixTime).TotalSeconds;

            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            ulong hash;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(keyBytes))
            {
                //byte[] counterBytes = BitConverter.GetBytes(counter);
                var counterBytes = Encoding.UTF8.GetBytes(counter.ToString());
                var hashBytes = hmac.ComputeHash(counterBytes);
                hash = BitConverter.ToUInt64(hashBytes, 0);
            }

            var token = hash % (ulong)Math.Pow(10, 15);

            return token.ToString("000000000000000");
        }

        //public bool HasSettingPIN(Guid accountId)
        //{
        //    MerchantAccountDAC dac = new MerchantAccountDAC();

        //    MerchantAccount account = dac.GetById(accountId);

        //    return !string.IsNullOrEmpty(account.PIN);
        //}

        //public MerchantAccount GetByUsername(string merchantAccount)
        //{
        //    MerchantAccountDAC dac = new MerchantAccountDAC();

        //    MerchantAccount account = dac.GetByUsername(merchantAccount);
        //    return account;
        //}

        public void ChangeLanguage(Guid merchantId, string language = "en")
        {
            RedisHelper.StringSet(Constant.REDIS_LANGUAGE_DBINDEX, $"{RedisKeys.FiiiPOS_APP_Language_MerchantId}:{merchantId}", language);
        }

        //public List<UserSecretES> ListAllSecretKeys()
        //{
        //    return new MerchantAccountDAC().ListAllSecretKeys();
        //}

        //public GetListByCodeListOM GetListByCodeList(string codes)
        //{
        //    var codeInfoList = new TokenAgent().GetMerchantIntoListByCodes(codes, false);
        //    var idList = codeInfoList.Select(a => a.MerchantId).ToList();
        //    var merchantAccountList = new MerchantAccountDAC().GetListByIdList(idList).Where(e => e.POSId != null).ToList();
        //    var profileList = GetMerchanstProfileInfo(merchantAccountList);

        //    var list = new List<GetListByCodeListOMItem>();
        //    foreach (var merchantAccount in merchantAccountList)
        //    {
        //        var profile = profileList.FirstOrDefault(e => e.MerchantId == merchantAccount.Id);
        //        var codeInfo = codeInfoList.FirstOrDefault(b => b.MerchantId == merchantAccount.Id);
        //        list.Add(new GetListByCodeListOMItem
        //        {
        //            Id = merchantAccount.Id.ToString(),
        //            MerchantAccount = GetMasked(merchantAccount.Username),
        //            MerchantName = merchantAccount.MerchantName,
        //            IconUrl = merchantAccount.Photo,
        //            IsAllowAcceptPayment = merchantAccount.IsAllowAcceptPayment,
        //            IsVerified = profile?.Status == VerifyStatus.Certified,
        //            RandomCode = codeInfo?.MerchantCode
        //        });
        //    }

        //    return new GetListByCodeListOM
        //    {
        //        List = list
        //    };
        //}

        //private List<MerchantVerifyStatusOM> GetMerchanstProfileInfo(List<MerchantAccount> list)
        //{
        //    var groupIdDic = list.GroupBy(e => e.CountryId).ToDictionary(e => e.Key, e => e.Select(a => a.Id).ToList());

        //    var resultList = new List<MerchantVerifyStatusOM>();

        //    foreach (var kv in groupIdDic)
        //    {
        //        var result = new MerchantProfileAgent().GetVerifyStatusListByIds(kv.Key, kv.Value);
        //        if (result != null)
        //        {
        //            resultList.AddRange(result);
        //        }
        //    }
        //    return resultList;
        //}

        //private string GetMasked(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //    {
        //        return "";
        //    }

        //    return str[0] + "*****" + str.Substring(Math.Max(0, str.Length - 4));
        //}

        //public void SettingPIN(Guid merchantAccountId, string pin)
        //{
        //    MerchantAccountDAC dac = new MerchantAccountDAC();

        //    var account = dac.GetById(merchantAccountId);
        //    if (account == null)
        //        return;
        //    if (!string.IsNullOrEmpty(account.PIN))
        //        return;

        //    dac.SettingPinById(merchantAccountId, PasswordHasher.HashPassword(pin));
        //}

        //public bool VerifyBusinessLicense(Guid accountId, string token, string businessLicense)
        //{
        //    SecurityVerification sv = new SecurityVerification(SystemPlatform.FiiiPOS);
        //    Dictionary<string, string> dic = sv.VerifyTokenWithExts<Dictionary<string, string>>(token, SecurityMethod.CellphoneCode, false);

        //    var agent = new MerchantProfileAgent();
        //    var profile = agent.GetMerchantProfile(accountId);

        //    if (profile?.VerifyStatus != VerifyStatus.Certified)
        //        return false;

        //    var result = profile.LicenseNo == businessLicense;

        //    if (result)
        //    {
        //        dic["BusinessLicense"] = businessLicense;
        //        sv.UpdateTokenExts(SecurityMethod.CellphoneCode, token, dic);
        //    }

        //    return result;
        //}

        //public MerchantAccount GetMerchantAccountByIdOrCode(Guid? merchantId, string code)
        //{
        //    if (!merchantId.HasValue)
        //    {
        //        merchantId = new TokenAgent().GetMerchantIntoByCode(code)?.MerchantId;
        //    }

        //    return merchantId.HasValue ? new MerchantAccountDAC().GetById(merchantId.Value) : null;
        //}


        public void VerifyModifyPINPIN(Guid accountId, string pin)
        {
            MerchantAccount merchant = new MerchantAccountDAC().GetById(accountId);
            if (merchant == null)
            {
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, Resources.用户不存在);
            }
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, accountId.ToString(), merchant.PIN, pin);

            var model = new UpdatePinVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("ModifyPIN"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void VerifyModifyPINCombine(Guid accountId, string smsCode, string googleCode)
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

            var model = SecurityVerify.GetModel<UpdatePinVerify>(new CustomVerifier("ModifyPIN"), SystemPlatform.FiiiPOS, accountId.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("ModifyPIN"), SystemPlatform.FiiiPOS, accountId.ToString(), model);
        }

        public void ModifyPIN(Guid accountId, string pin)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetById(accountId);

            SecurityVerify.Verify<UpdatePinVerify>(new CustomVerifier("ModifyPIN"), SystemPlatform.FiiiPOS, account.Id.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });
            

            if (PasswordHasher.VerifyHashedPassword(account.PIN, pin))
                throw new CommonException(ReasonCode.PIN_MUST_BE_DIFFERENT, Resources.新PIN不能与原PIN一样);

            account.PIN = PasswordHasher.HashPassword(pin);
            dac.UpdatePIN(account);
        }

        //public void ModifyPINByCellphone(Guid accountId, string cellphoneToken, string pin)
        //{
        //    SecurityVerification sv = new SecurityVerification(SystemPlatform.FiiiPOS);
        //    Dictionary<string, string> dic = sv.VerifyTokenWithExts<Dictionary<string, string>>(cellphoneToken, SecurityMethod.CellphoneCode);

        //    if (!dic.ContainsKey("BusinessLicense"))
        //        return;

        //    var dac = new MerchantAccountDAC();
        //    var account = dac.GetById(accountId);
        //    if (PasswordHasher.VerifyHashedPassword(account.PIN, pin))
        //        throw new CommonException(10000, Resources.新PIN不能与原PIN一样);

        //    account.PIN = PasswordHasher.HashPassword(pin);
        //    dac.UpdatePIN(account);
        //}

        public void Feedback(Guid accountId, string content)
        {
            var feedback = new Feedback
            {
                AccountId = accountId,
                Context = content,
                HasProcessor = false,
                Timestamp = DateTime.UtcNow,
                Type = "FiiiPOS"
            };
            new FeedBackDAC().Insert(feedback);
        }


        public bool ScanQRLogin(Guid merchantId, string qrCode)
        {
            if (!qrCode.StartsWith(RedisKeys.FiiiPOS_APP_QRCodePrefix_Web))
            {
                return false;
            }
            int db = RedisKeys.FiiiPOS_APP_RedisDB_Web;

            string key = $"{RedisKeys.FiiiPOS_APP_Redis_Key_QRCode_Web}{qrCode}";

            if (!RedisHelper.KeyExists(db, key))
            {
                return false;
            }
            //填充qrcode存在的token
            bool result = RedisHelper.StringSet(db, key, merchantId.ToString());
            return result;
        }

        public void ModifyMerchantName(Guid merchantAccountId, string merchantName)
        {
            var dac = new MerchantAccountDAC();

            dac.UpdateMerchantName(merchantAccountId, merchantName);
        }

        public void SendBindingSMS(string cellphone, int countryId, string merchantAccount, string sn)
        {
            var pos = new POSDAC().GetBySn(sn);
            if (pos == null)
                throw new CommonException(ReasonCode.POSSN_ERROR, Resources.SN码不存在);

            var account = new MerchantAccountDAC().GetByUsername(merchantAccount);
            if (account == null)
                throw new GeneralException(Resources.AccountNotExists);

            if (account.POSId.HasValue)
            {
                if (account.POSId == pos.Id)
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.AccountHasBoundThisPOS);
                else
                    throw new CommonException(ReasonCode.GENERAL_ERROR, Resources.AccountHasBoundOtherPOS);
            }

            var country = new CountryComponent().GetById(countryId);
            if (country == null)
                throw new CommonException(10000, Resources.国家不存在);
            if (account.PhoneCode != country.PhoneCode || account.Cellphone != cellphone)
                throw new GeneralException(Resources.当前手机号与账号绑定的手机号不一致);
            
            SecurityVerify.SendCode(new BindAccountCellphoneVerifier(), SystemPlatform.FiiiPOS, merchantAccount, $"{account.PhoneCode}{account.Cellphone}");
        }

        public AccountNeedVerifyInfo VerifyMerchantAccount(int countryId, string cellphone, string code, string merchantAccount)
        {
            SecurityVerify.Verify(new BindAccountCellphoneVerifier(), SystemPlatform.FiiiPOS, merchantAccount, code);

            var accountDac = new MerchantAccountDAC();
            var account = accountDac.GetByUsername(merchantAccount);
            if (account == null)
                throw new GeneralException(Resources.AccountNotExists);

            var country = new CountryComponent().GetById(countryId);
            if (country == null)
                throw new CommonException(10000, Resources.国家不存在);

            string fullCellphone = $"{account.PhoneCode}{account.Cellphone}";
            if (!string.Equals(fullCellphone, country.PhoneCode + cellphone, StringComparison.InvariantCulture))
                throw new GeneralException(Resources.当前手机号与账号绑定的手机号不一致);

            var model = new BindAccountVerify
            {
                MerchantAccount = merchantAccount,
                CellphoneVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("BindAccount"), SystemPlatform.FiiiPOS, merchantAccount, model);

            return new AccountNeedVerifyInfo
            {
                PIN = true,
                GoogleAuth = ValidationFlagComponent.CheckSecurityOpened(account.ValidationFlag, ValidationFlag.GooogleAuthenticator)
            };
        }

        public void VerifyPINByMerchantAccount(string pin, string merchantAccount)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetByUsername(merchantAccount);

            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), account.PIN, pin);
            var model = SecurityVerify.GetModel<BindAccountVerify>(new CustomVerifier("BindAccount"), SystemPlatform.FiiiPOS, merchantAccount);
            model.PinVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("BindAccount"), SystemPlatform.FiiiPOS, merchantAccount, model);
        }

        public void VerifyGoogleAuthByMerchantAccount(string googleCode, string merchantAccount)
        {
            var dac = new MerchantAccountDAC();
            var account = dac.GetByUsername(merchantAccount);

            SecurityVerify.Verify(new GoogleVerifier(), SystemPlatform.FiiiPOS, account.Id.ToString(), account.AuthSecretKey, googleCode);
            var model = SecurityVerify.GetModel<BindAccountVerify>(new CustomVerifier("BindAccount"), SystemPlatform.FiiiPOS, merchantAccount);
            model.GoogleVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("BindAccount"), SystemPlatform.FiiiPOS, merchantAccount, model);
        }

        public SignonDTO BindingAccount(string merhcantAccount, string posSN)
        {
            var accountDac = new MerchantAccountDAC();
            MerchantAccount account = accountDac.GetByUsername(merhcantAccount);
            SecurityVerify.Verify<BindAccountVerify>(new CustomVerifier("BindAccount"), SystemPlatform.FiiiPOS, merhcantAccount, (model) =>
            {
                bool result = true;
                result = result && merhcantAccount.Equals(model.MerchantAccount);
                result = result && model.CellphoneVerified && model.PinVerified;
                if (account == null)
                    return false;
                if (ValidationFlagComponent.CheckSecurityOpened(account.ValidationFlag, ValidationFlag.GooogleAuthenticator))
                    result = result && model.GoogleVerified;
                return result;
            });

            var posDac = new POSDAC();
            if (account.Status == AccountStatus.Locked)
                throw new CommonException(ReasonCode.ACCOUNT_LOCKED, Resources.帐号已锁定);

            var pos = posDac.GetBySn(posSN);
            if (pos == null)
                throw new GeneralException(Resources.SN码不存在);

            if (account.POSId.HasValue)
            {
                if (account.POSId == pos.Id)
                    throw new GeneralException(Resources.AccountHasBoundThisPOS);
                else
                    throw new GeneralException(Resources.AccountHasBoundOtherPOS);
            }

            if (pos.Status)
                throw new GeneralException(Resources.POSHasBoundOtherAccount);
            
            UserAccount userAccount = null;
            if (!string.IsNullOrEmpty(account.InvitationCode))
            {
                userAccount = new UserAccountDAC().GetByInvitationCode(account.InvitationCode);
            }

            POSMerchantBindRecord posBindRecord = new POSMerchantBindRecord
            {
                POSId = pos.Id,
                SN = pos.Sn,
                MerchantId = account.Id,
                MerchantUsername = account.Username,
                BindTime = DateTime.UtcNow,
                BindStatus = (byte)POSBindStatus.Binded
            };

            using (var scope = new TransactionScope())
            {
                account.POSId = pos.Id;
                accountDac.BindPos(account);
                posDac.ActivePOS(pos);
                new POSMerchantBindRecordDAC().Insert(posBindRecord);
                if (!string.IsNullOrEmpty(account.InvitationCode) && userAccount != null)
                {
                    ReBindInviter(posSN, account.Id, userAccount.Id, account.InvitationCode);
                }

                scope.Complete();
            }

            return GetAccessToken(pos, account);
        }

        public void VerifyUnBindAccountPin(Guid merchantId, string pin)
        {
            var merchant = new MerchantAccountDAC().GetById(merchantId);
            SecurityVerify.Verify(new PinVerifier(), SystemPlatform.FiiiPOS, merchant.Id.ToString(), merchant.PIN, pin);

            var model = new UnBindAccountVerify
            {
                PinVerified = true
            };
            SecurityVerify.SetModel(new CustomVerifier("UnBindAccount"), SystemPlatform.FiiiPOS, merchant.Id.ToString(), model);
        }

        public void VerifyUnBindAccountCombine(Guid merchantId, string smsCode, string googleCode)
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

            SecurityVerify.CombinedVerify(SystemPlatform.FiiiPOS, merchantId.ToString(), userSecrets, options);

            var model = SecurityVerify.GetModel<UnBindAccountVerify>(new CustomVerifier("UnBindAccount"), SystemPlatform.FiiiPOS, merchantId.ToString());
            model.CombinedVerified = true;
            SecurityVerify.SetModel(new CustomVerifier("UnBindAccount"), SystemPlatform.FiiiPOS, merchantId.ToString(), model);
        }

        public void UnbindingAccount(Guid merchantAccountId)
        {
            SecurityVerify.Verify<UnBindAccountVerify>(new CustomVerifier("UnBindAccount"), SystemPlatform.FiiiPOS, merchantAccountId.ToString(), (model) =>
            {
                return model.PinVerified && model.CombinedVerified;
            });

            var accountDAC = new MerchantAccountDAC();
            var account = accountDAC.GetById(merchantAccountId);

            var posDAC = new POSDAC();
            var pos = posDAC.GetById(account.POSId.Value);
            var recordId = new POSMerchantBindRecordDAC().GetByMerchantId(merchantAccountId).Id;
            var invitorId = new InviteRecordDAC().GetInvitorIdBySn(pos.Sn);
            account.POSId = null;
            bool bindingGoogleAuth = !string.IsNullOrEmpty(account.AuthSecretKey);
            bool openedGoogleAuth =
                ValidationFlagComponent.CheckSecurityOpened(account.ValidationFlag, ValidationFlag.GooogleAuthenticator);
            if (bindingGoogleAuth && !openedGoogleAuth)
            {
                account.ValidationFlag =
                    ValidationFlagComponent.AddValidationFlag(account.ValidationFlag, ValidationFlag.GooogleAuthenticator);
            }
            
            using (var scope = new TransactionScope())
            {
                accountDAC.UnbindingAccount(account);
                new POSDAC().InactivePOS(pos);
                new POSMerchantBindRecordDAC().UnbindRecord(account.Id, pos.Id);
                if (!string.IsNullOrEmpty(account.InvitationCode))
                    UnBindInviter(pos.Sn);

                scope.Complete();
            }
            //Task.Run(() => RemoveRegInfoByUserId(merchantAccountId));
            if (!string.IsNullOrEmpty(account.InvitationCode))
            {
                RabbitMQSender.SendMessage("UnBindingAccount", new Tuple<Guid, long>(invitorId, recordId));
            }

            RemoveRegInfoByUserId(merchantAccountId);
        }

        public void SettingMarkup(Guid accountId, decimal markupPer)
        {
            var merchantAccountDac = new MerchantAccountDAC();

            merchantAccountDac.SettingMarkup(accountId, markupPer / 100);

        }
        public void SettingFiatCurrency(Guid accountId, string fiatCurrency)
        {
            var list = new CurrenciesDAC().GetAll();

            if (list.All(e => e.Code != fiatCurrency))
                throw new CommonException(ReasonCode.GENERAL_ERROR, "Fiat currency error.");

            var dac = new MerchantAccountDAC();

            dac.SettingFiatCurrency(accountId, fiatCurrency);
        }

        public void CheckAccount(string username, string sn)
        {
            POS pos = new POSDAC().GetBySn(sn);
            if (pos == null || !pos.Status)
                throw new CommonException(ReasonCode.POSSN_ERROR, Resources.SN码不存在);

            if (!pos.IsMiningEnabled)
                throw new CommonException(ReasonCode.NOT_ALLOW_MINING, "Not allow mining");

            MerchantAccount account = new MerchantAccountDAC().GetByPosSn(sn, username);
            if (account == null)
                throw new CommonException(ReasonCode.ACCOUNT_NOT_EXISTS, "Account not exist");

            if (account.Status == AccountStatus.Locked)
                throw new CommonException(ReasonCode.ACCOUNT_LOCKED, "Account locked");
        }

        //public void SettingMinerCryptoAddress(Guid merchantId, string address)
        //{
        //    var account = new MerchantAccountDAC().GetById(merchantId);
        //    if (account?.POSId == null)
        //        return;
        //    if (account.Status != AccountStatus.Active)
        //        return;

        //    var pos = new POSDAC().GetById(account.POSId.Value);
        //    if (!pos.Status)
        //        return;

        //   new FiiiChainAgent().SaveMiners(address, account.Username, pos.Sn);

        //    new MerchantAccountDAC().SettingMinerCryptoAddress(merchantId, address);
        //}

        public void SetWithdrawalFeeType(Guid accountId, WithdrawalFeeType type)
        {
            new MerchantAccountDAC().SetWithdrawalFeeType(accountId, type);
        }

        /// <summary>
        /// POS机指定邀请人
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="merchantId"></param>
        /// <param name="userId"></param>
        /// <param name="invitationCode"></param>
        private int BindInviter(string SN, Guid merchantId, Guid userId, string invitationCode)
        {
            var irDAC = new InviteRecordDAC();
            var existRecord = irDAC.GetBySN(SN);
            if (existRecord == null)
                return irDAC.Insert(new InviteRecord
                {
                    SN = SN,
                    Type = InviteType.Fiiipos,
                    AccountId = merchantId,
                    Timestamp = DateTime.UtcNow,
                    InviterCode = invitationCode,
                    InviterAccountId = userId
                });
            new InviteRecordDAC().UpdateAccountInfo(userId, merchantId, invitationCode, SN);

            return existRecord.Id;
        }

        private void ReBindInviter(string SN, Guid merchantId, Guid userId, string invitationCode)
        {
            new InviteRecordDAC().UpdateAccountInfo(userId, merchantId, invitationCode, SN);
        }

        /// <summary>
        /// POS取消邀请人
        /// </summary>
        /// <param name="SN"></param>
        private void UnBindInviter(string SN)
        {
            new InviteRecordDAC().UpdateAccountInfo(Guid.Empty, Guid.Empty, null, SN);
        }


        public void ChangeLanguagetoDb(Guid accountId, string language)
        {
            new MerchantAccountDAC().UpdateLanguage(accountId, language);
        }
    }
}