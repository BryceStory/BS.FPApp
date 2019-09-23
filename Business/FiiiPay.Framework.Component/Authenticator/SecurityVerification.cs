using System;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Framework.Component.Authenticator
{
    public class SecurityVerification
    {
        internal string Platform { get; set; }
        public SecurityVerification(SystemPlatform platform)
        {
            Platform = platform.ToString();
        }

        public string GenegeToken(string key, SecurityMethod securityMethod)
        {
            var token = RandomAlphaNumericGenerator.Generate(16);
            string tokenKey = $"{Platform}:{securityMethod.ToString()}:{SecurityMethod.TempToken.ToString()}:{key}";
            RedisHelper.StringSet(tokenKey, token, TimeSpan.FromMinutes(Constant.TEMPTOKEN_EXPIRED_TIME));
            return token;
        }

        public void VerifyToken(string key, string token, SecurityMethod securityMethod)
        {
            string tokenKey = $"{Platform}:{securityMethod.ToString()}:{SecurityMethod.TempToken.ToString()}:{key}";
            var cacheToken = RedisHelper.StringGet(tokenKey);
            var errorCount = CheckErrorCount(SecurityMethod.TempToken, key, securityMethod);
            if (string.IsNullOrEmpty(cacheToken))
            {
                IncreaseErrorCount(SecurityMethod.TempToken, key, securityMethod);
            }
            if (token != cacheToken)
            {
                IncreaseErrorCount(SecurityMethod.TempToken, key, securityMethod);
            }
            RedisHelper.KeyDelete(tokenKey);
            DeleteErrorCount(SecurityMethod.TempToken, key, securityMethod);
        }

        public string GenegeToken(Guid accountId, SecurityMethod securityMethod)
        {
            return GenegeToken(accountId.ToString(), securityMethod);
        }
        public void VerifyToken(Guid accountId, string token, SecurityMethod securityMethod)
        {
            VerifyToken(accountId.ToString(), token, securityMethod);
        }

        public string GenegeToken(SecurityMethod securityMethod)
        {
            var token = RandomAlphaNumericGenerator.Generate(16);
            string tokenKey = $"{Platform}:{securityMethod.ToString()}:{SecurityMethod.TempToken.ToString()}:{token}";

            RedisHelper.StringSet(tokenKey, token, TimeSpan.FromMinutes(Constant.TEMPTOKEN_EXPIRED_TIME));
            return token;
        }

        public void VerifyToken(string token, SecurityMethod securityMethod, bool needDeleteToken = true)
        {
            string tokenKey = $"{Platform}:{securityMethod.ToString()}:{SecurityMethod.TempToken.ToString()}:{token}";
            var cacheToken = RedisHelper.StringGet(tokenKey);
            var errorCount = CheckErrorCount(SecurityMethod.TempToken, token);
            if (string.IsNullOrEmpty(cacheToken))
            {
                IncreaseErrorCount(SecurityMethod.TempToken, token);
                ++errorCount;
                throw new CommonException(ReasonCode.FAIL_AUTHENTICATOR, string.Format(GeneralResources.SecurityValidateError, errorCount));
            }
            if (token != cacheToken)
            {
                IncreaseErrorCount(SecurityMethod.TempToken, token);
                ++errorCount;
                throw new CommonException(ReasonCode.FAIL_AUTHENTICATOR, string.Format(GeneralResources.SecurityValidateError, errorCount));
            }
            RedisHelper.KeyDelete(tokenKey);
            DeleteErrorCount(SecurityMethod.TempToken, token);
        }

        /// <summary>
        /// 检查谷歌验证码是否被验证过
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="code"></param>
        public void CheckUsedSign( string accountId, string code)
        {
            var errorKey = $"{Platform}:{SecurityMethod.GoogleAuthencator.ToString()}:BeUsed:{accountId}{code}";
            if (RedisHelper.KeyExists(errorKey))
                throw new CommonException(ReasonCode.GOOGLECODE_BEUSED, GeneralResources.EMGoogleCodeBeUsed);
        }
        /// <summary>
        /// 谷歌验证码添加验证过的标志
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="code"></param>
        public void InsertUsedSign(string accountId, string code)
        {
            var errorKey = $"{Platform}:{SecurityMethod.GoogleAuthencator.ToString()}:BeUsed:{accountId}{code}";
            RedisHelper.StringSet(errorKey, code, TimeSpan.FromMinutes(2));
        }
        
        /// <summary>
        /// 安全验证错误次数检查（除手机码验证）
        /// </summary>
        /// <param name="securityMethod"></param>
        /// <param name="accountId"></param>
        /// <param name="childrMethod"></param>
        /// <returns></returns>
        public int CheckErrorCount(SecurityMethod securityMethod, string accountId, SecurityMethod? childrMethod = null)
        {
            var errorKey = $"{Platform}:{securityMethod.ToString()}:ErrorCounts:{accountId}";
            if(securityMethod== SecurityMethod.TempToken)
                errorKey = $"{Platform}:{childrMethod}:{securityMethod.ToString()}:ErrorCounts:{accountId}";
            var errorCountsStr = RedisHelper.StringGet(errorKey);
            int.TryParse(errorCountsStr, out int errorCount);
            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(errorKey);
                ThrowMoreTimesException(securityMethod, minCount);
            }
            return errorCount;
        }

        /// <summary>
        /// 手机码验证错误次数检查
        /// </summary>
        /// <param name="business"></param>
        /// <param name="uniqueKey"></param>
        /// <returns></returns>
        public int CheckErrorCount(SMSBusiness business, string uniqueKey)
        {
            SecurityMethod securityMethod = SecurityMethod.CellphoneCode;
            var errorKey = $"{Platform}:{securityMethod.ToString()}:{business.ToString()}:ErrorCounts:{uniqueKey}";
            var errorCountsStr = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, errorKey);
            int.TryParse(errorCountsStr, out int errorCount);
            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(Constant.REDIS_SMS_DBINDEX, errorKey);
                ThrowMoreTimesException(business, minCount);
            }
            return errorCount;
        }

        /// <summary>
        /// 错误次数加1（除手机码验证）
        /// </summary>
        /// <param name="securityMethod"></param>
        /// <param name="accountId"></param>
        /// <param name="childrMethod">子操作类型，如安全验证包含多个方式的验证，此参数用于返回相应的错误代码</param>
        public void IncreaseErrorCount(SecurityMethod securityMethod, string accountId, SecurityMethod? childrMethod = null)
        {
            var errorKey = $"{Platform}:{securityMethod.ToString()}:ErrorCounts:{accountId}";
            if (securityMethod == SecurityMethod.TempToken)
                errorKey = $"{Platform}:{childrMethod}:{securityMethod.ToString()}:ErrorCounts:{accountId}";
            var errorCountsStr = RedisHelper.StringGet(errorKey);
            int.TryParse(errorCountsStr, out int errorCount);
            ++errorCount;
            int spInt = Constant.VIRIFY_FAILD_LOCK_TIME;
            RedisHelper.StringSet(errorKey, errorCount.ToString(), TimeSpan.FromMinutes(spInt));
            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(errorKey);
                ThrowMoreTimesException(securityMethod, minCount);
            }
            else
            {
                ThrowVerifyFaildException(securityMethod, Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount, childrMethod);
            }
        }

        /// <summary>
        /// 手机码验证错误次数加1
        /// </summary>
        /// <param name="business"></param>
        /// <param name="uniqueKey"></param>
        public void IncreaseErrorCount(SMSBusiness business, string uniqueKey)
        {
            SecurityMethod securityMethod = SecurityMethod.CellphoneCode;
            var errorKey = $"{Platform}:{securityMethod.ToString()}:{business.ToString()}:ErrorCounts:{uniqueKey}";

            var errorCountsStr = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, errorKey);
            int.TryParse(errorCountsStr, out int errorCount);
            ++errorCount;
            int spInt = Constant.VIRIFY_FAILD_LOCK_TIME;
            if (business == SMSBusiness.Register || business == SMSBusiness.UpdateCellphoneNew)
                spInt = Constant.REGISTER_FAILD_LOCK_TIME;
            RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, errorKey, errorCount.ToString(), TimeSpan.FromMinutes(spInt));
            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(Constant.REDIS_SMS_DBINDEX, errorKey);
                ThrowMoreTimesException(business, minCount);
            }
            else
            {
                ThrowVerifyFaildException(securityMethod, Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount);
            }
        }

        /// <summary>
        /// 清除安全验证错误次数（除手机码验证）
        /// </summary>
        /// <param name="securityMethod"></param>
        /// <param name="accountId"></param>
        /// <param name="childrMethod"></param>
        public void DeleteErrorCount(SecurityMethod securityMethod, string accountId, SecurityMethod? childrMethod = null)
        {
            var errorKey = $"{Platform}:{securityMethod.ToString()}:ErrorCounts:{accountId}";
            if (securityMethod == SecurityMethod.TempToken)
                errorKey = $"{Platform}:{childrMethod}:{securityMethod.ToString()}:ErrorCounts:{accountId}";
            RedisHelper.KeyDelete(errorKey);
        }

        /// <summary>
        /// 清除手机码验证错误次数
        /// </summary>
        /// <param name="business"></param>
        /// <param name="uniqueKey"></param>
        public void DeleteErrorCount(SMSBusiness business, string uniqueKey)
        {
            var errorKey = $"{Platform}:{SecurityMethod.CellphoneCode.ToString()}:{business.ToString()}:ErrorCounts:{uniqueKey}";
            RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, errorKey);
        }

        /// <summary>
        /// 验证失败异常
        /// </summary>
        /// <param name="securityMethod"></param>
        /// <param name="timesLeft"></param>
        /// <param name="childrMethod"></param>
        private void ThrowVerifyFaildException(SecurityMethod securityMethod, int timesLeft, SecurityMethod? childrMethod = null)
        {
            switch (securityMethod)
            {
                case SecurityMethod.CellphoneCode:
                    throw new CommonException(ReasonCode.WRONG_CODE_ENTERRED, string.Format(GeneralResources.EMSMSCodeError, timesLeft));
                case SecurityMethod.LoginGoogleAuthencator:
                case SecurityMethod.LoginBySMSGoogleAuthencator:
                case SecurityMethod.GoogleAuthencator:
                    throw new CommonException(ReasonCode.GOOGLEAUTH_VERIFY_FAIL, string.Format(GeneralResources.EMGoogleCodeError, timesLeft));
                case SecurityMethod.Password:
                    throw new CommonException(ReasonCode.WRONG_PASSWORD_ENTERRED, string.Format(GeneralResources.EMAccountPasswordError, timesLeft));
                case SecurityMethod.OldPassword:
                    throw new CommonException(ReasonCode.WRONG_OLD_PASSWORD_ENTERRED, string.Format(GeneralResources.EMPasswordError, timesLeft));
                case SecurityMethod.Pin:
                    throw new CommonException(ReasonCode.PIN_ERROR, string.Format(GeneralResources.EMPINInputError, timesLeft));
                case SecurityMethod.RegisterPhoneCode:
                    throw new CommonException(ReasonCode.WRONG_CODE_ENTERRED, string.Format(GeneralResources.EMSMSCodeError, timesLeft));
                case SecurityMethod.SecurityValidate:
                    if (childrMethod.HasValue)
                    {
                        if (childrMethod.Value == SecurityMethod.GoogleAuthencator)
                            throw new CommonException(ReasonCode.WRONG_SECURITYGOOGLECODE_ENTERRED, string.Format(GeneralResources.EMGoogleCodeError, timesLeft));
                        else if (childrMethod.Value == SecurityMethod.CellphoneCode)
                            throw new CommonException(ReasonCode.WRONG_SECURITYPHONECODE_ENTERRED, string.Format(GeneralResources.EMSMSCodeError, timesLeft));
                    }
                    throw new CommonException(ReasonCode.FAIL_AUTHENTICATOR, string.Format(GeneralResources.SecurityValidateError, timesLeft));
                case SecurityMethod.TempToken:
                    throw new CommonException(ReasonCode.FAIL_AUTHENTICATOR, string.Format(GeneralResources.SecurityValidateError, timesLeft));
            }
            throw new CommonException(ReasonCode.FAIL_AUTHENTICATOR, string.Format(GeneralResources.SecurityValidateError, timesLeft));
        }

        /// <summary>
        /// 安全验证多次错误异常（除手机码验证）
        /// </summary>
        /// <param name="securityMethod"></param>
        /// <param name="minCount"></param>
        private void ThrowMoreTimesException(SecurityMethod securityMethod, int minCount)
        {
            switch (securityMethod)
            {
                case SecurityMethod.LoginGoogleAuthencator:
                case SecurityMethod.LoginBySMSGoogleAuthencator:
                case SecurityMethod.GoogleAuthencator:
                    throw new CommonException(ReasonCode.GOOGLEAUTH_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyLimit5Times,minCount));
                case SecurityMethod.Password:
                    throw new CommonException(ReasonCode.LOGIN_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMPasswordTry5Times,minCount));
                case SecurityMethod.OldPassword:
                    throw new CommonException(ReasonCode.OLD_PASSWORD_TOO_MANY_TIMES, string.Format(GeneralResources.EMPasswordError5Times, minCount));
                case SecurityMethod.Pin:
                    throw new CommonException(ReasonCode.PIN_ERROR_5_TIMES, string.Format(GeneralResources.EMPINInputLimit,minCount));
                case SecurityMethod.RegisterPhoneCode:
                    throw new CommonException(ReasonCode.PHONECODE_VERIFYFAILED_TOOMANY_TEIMS, string.Format(GeneralResources.EMRegisterVerifyLimit5Times, minCount));
                case SecurityMethod.SecurityValidate:
                    throw new CommonException(ReasonCode.SECURITY_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
                case SecurityMethod.TempToken:
                    throw new CommonException(ReasonCode.SECURITY_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
            }
            throw new CommonException(ReasonCode.SECURITY_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
        }

        /// <summary>
        /// 手机验证码多次错误异常
        /// </summary>
        /// <param name="business"></param>
        /// <param name="minCount"></param>
        private void ThrowMoreTimesException(SMSBusiness business, int minCount)
        {
            switch (business)
            {
                case SMSBusiness.Login:
                    throw new CommonException(ReasonCode.LOGIN_ERROR_TOO_MANY_TIMES_BYSMS, string.Format(GeneralResources.EMLoginSMSCodeTry5Times, minCount));
                case SMSBusiness.Register:
                case SMSBusiness.UpdateCellphoneNew:
                    throw new CommonException(ReasonCode.PHONECODE_VERIFYFAILED_TOOMANY_TEIMS, string.Format(GeneralResources.EMRegisterVerifyLimit5Times, minCount));
                default:
                    throw new CommonException(ReasonCode.PHONECODE_VERIFYFAILED_TOOMANY_TEIMS, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
            }
        }

        private int GetErrorCount(string errorKey)
        {
            var errorCountsStr = RedisHelper.StringGet(errorKey).ToString();
            int errorCount = 0;
            if (!string.IsNullOrEmpty(errorCountsStr))
                errorCount = Convert.ToInt32(errorCountsStr);
            return errorCount;
        }

        private int GetErrorLockTime(string errorKey)
        {
            var ts = RedisHelper.KeyTimeToLive(errorKey);
            var minCount = ts.HasValue ? Math.Ceiling(ts.Value.TotalMinutes) : 1;
            return Convert.ToInt32(minCount);
        }

        private int GetErrorLockTime(int db, string errorKey)
        {
            var ts = RedisHelper.KeyTimeToLive(db,errorKey);
            var minCount = ts.HasValue ? Math.Ceiling(ts.Value.TotalMinutes) : 1;
            return Convert.ToInt32(minCount);
        }
    }
}
