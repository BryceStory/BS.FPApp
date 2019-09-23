using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Enums;
using System;
using System.Collections.Generic;

namespace FiiiPay.Framework.Component.Verification
{
    /// <summary>
    /// 安全验证类
    /// </summary>
    public class SecurityVerify
    {
        private const int RedisDBIndex = Constant.REDIS_SMS_DBINDEX;

        public static void SendCode(RandomValueVerifier verifier, SystemPlatform plateForm, string uniqueKey, string targetCellphone,string message=null)
        {
            var errorCountKey = verifier.GetErrorCountKey(plateForm,uniqueKey);
            CheckErrorCount(verifier, errorCountKey);

            var code = verifier.SendCode(plateForm, uniqueKey, targetCellphone, message);

            if (string.IsNullOrEmpty(code))
                return;

            var model = new RandomCodeModel
            {
                Code = code,
                ExpireTime = DateTime.UtcNow.AddMinutes(verifier.GetExpireTime()).ToUnixTime()
            };
            string codeKey = verifier.GetCodeKey(plateForm, uniqueKey);

            RedisHelper.Set(RedisDBIndex, codeKey, model, TimeSpan.FromMinutes(Constant.SMS_CACHE_TIME));
        }

        public static void Verify(RandomValueVerifier verifier, SystemPlatform plateForm, string uniqueKey, string code, bool codeCanVerifyAgain = false)
        {
            var errorCountKey = verifier.GetErrorCountKey(plateForm, uniqueKey);
            int errorCount = CheckErrorCount(verifier, errorCountKey);
            var codeKey = verifier.GetCodeKey(plateForm, uniqueKey);
            var codeModel = RedisHelper.Get<RandomCodeModel>(Constant.REDIS_SMS_DBINDEX, codeKey);
            bool result = verifier.Verify(plateForm, uniqueKey, code);
            if (result)
            {
                if (errorCount > 0)
                    DeleteErrorCount(errorCountKey);
                if (!codeCanVerifyAgain)
                    InvalidateCode(verifier, plateForm, uniqueKey);
            }
            else
            {
                IncreaseErrorCount(verifier, errorCountKey);
            }
        }

        public static void Verify(ComputedValueVerifier verifier, SystemPlatform plateForm, string uniqueKey, string secretKey, string code,bool codeCanVerifyAgain=false)
        {
            var errorCountKey = verifier.GetErrorCountKey(plateForm, uniqueKey);
            int errorCount = CheckErrorCount(verifier, errorCountKey);

            if (!CheckCodeValid(verifier, plateForm, uniqueKey, code))
            {
                verifier.VerifyInvalidCode();
            }

            var result = verifier.Verify(secretKey, code);
            if (result)
            {
                if (errorCount > 0)
                    DeleteErrorCount(errorCountKey);
                if (!codeCanVerifyAgain)
                    InvalidateCode(verifier, plateForm, uniqueKey, code);
            }
            else
            {
                IncreaseErrorCount(verifier, errorCountKey);
            }
        }

        public static void Verify(FixValueVerifier verifier, SystemPlatform plateForm, string uniqueKey, string hashedCode, string toCheckCode, bool needDecrypt = true)
        {
            var errorCountKey = verifier.GetErrorCountKey(plateForm, uniqueKey);
            int errorCount = CheckErrorCount(verifier, errorCountKey);

            bool result = true;
            if (needDecrypt)
            {
                try
                {
                    toCheckCode = AES128.Decrypt(toCheckCode, AES128.DefaultKey);
                }
                catch
                {
                    result = false;
                }
            }

            result = result && verifier.Verify(plateForm, uniqueKey, hashedCode, toCheckCode);
            if (result)
            {
                if (errorCount > 0)
                    DeleteErrorCount(errorCountKey);
            }
            else
            {
                IncreaseErrorCount(verifier, errorCountKey);
            }
        }

        public static void Verify<T>(CustomVerifier verifier, SystemPlatform plateForm, string uniqueKey, Func<T,bool> VerifyFunction) where T : IVerified
        {
            var errorCountKey = verifier.GetErrorCountKey(plateForm, uniqueKey);
            int errorCount = CheckErrorCount(verifier, errorCountKey);
            string modelKey = verifier.GetVerifyModelKey(plateForm, uniqueKey);

            var model = RedisHelper.Get<T>(RedisDBIndex, modelKey);

            if (model != null && model.ExpireTime <= DateTime.UtcNow.ToUnixTime())
                verifier.CodeExpried();

            bool result = true;
            try
            {
                result = VerifyFunction.Invoke(model);
            }
            catch (Framework.Exceptions.CommonException ce)
            {
                result = false;
                IncreaseErrorCount(verifier, errorCountKey, ce);
            }
            catch (Exception ex)
            {
                result = false;
                IncreaseErrorCount(verifier, errorCountKey);
            }

            if (result)
            {
                if (errorCount > 0)
                    DeleteErrorCount(errorCountKey);
                InvalidateCode(verifier, plateForm, uniqueKey);
            }
            else
            {
                IncreaseErrorCount(verifier, errorCountKey);
            }
        }

        public static T GetModel<T>(CustomVerifier verifier, SystemPlatform plateForm, string uniqueKey) where T : IVerified
        {
            string modelKey = verifier.GetVerifyModelKey(plateForm, uniqueKey);
            return RedisHelper.Get<T>(RedisDBIndex, modelKey);
        }

        public static void SetModel<T>(CustomVerifier verifier, SystemPlatform plateForm, string uniqueKey,T model) where T : IVerified
        {
            string modelKey = verifier.GetVerifyModelKey(plateForm, uniqueKey);
            model.ExpireTime = DateTime.UtcNow.AddMinutes(Constant.VERIFICATION_CACHE_TIME).ToUnixTime();
            RedisHelper.Set(RedisDBIndex, modelKey, model, TimeSpan.FromMinutes(Constant.TEMPTOKEN_EXPIRED_TIME));
        }

        /// <summary>
        /// 组合验证，验证用户的所有安全设置
        /// </summary>
        /// <param name="plateForm">系统</param>
        /// <param name="uniqueKey">唯一标志</param>
        /// <param name="serects">用户个人信息</param>
        /// <param name="options">需验证的类型</param>
        public static void CombinedVerify(SystemPlatform plateForm, string uniqueKey, UserSecrets serects, List<CombinedVerifyOption> options, string divisionCode=null)
        {
            if (options == null || options.Count == 0)
                return;
            var combinedVerifier = new MandatoryVerifier();
            var idNumberVerifier = new IDNumberVerifier();
            var cellphoneVerifier = new MandatoryCellphoneVerifier();
            var googleVerifier = new GoogleVerifier();
            var errorCountKey = combinedVerifier.GetErrorCountKey(plateForm, uniqueKey);

            int errorCount = CheckErrorCount(combinedVerifier, errorCountKey);

            bool result = true;
            bool googleCodeBeUsed = false;
            byte erroredFlag = (byte)ValidationFlag.Cellphone;

            var identityOption = options.Find(t => t.AuthType == (byte)ValidationFlag.IDNumber);
            var cellphoneOption = options.Find(t => t.AuthType == (byte)ValidationFlag.Cellphone);
            var googleAuthOption = options.Find(t => t.AuthType == (byte)ValidationFlag.GooogleAuthenticator);

            if (identityOption != null)
            {
                result = result && idNumberVerifier.Verify(plateForm, uniqueKey, serects.IdentityNo, identityOption.Code);
                if (!result)
                    erroredFlag = (byte)ValidationFlag.IDNumber;
            }
            if (result)
            {
                if (CheckSecurityOpened(serects.ValidationFlag, ValidationFlag.Cellphone))
                {
                    if (cellphoneOption == null)
                        result = false;

                    result = result && cellphoneVerifier.Verify(plateForm, divisionCode+uniqueKey, cellphoneOption.Code);
                    if (!result)
                        erroredFlag = (byte)ValidationFlag.Cellphone;
                }
            }
            if (result)
            {
                if (CheckSecurityOpened(serects.ValidationFlag, ValidationFlag.GooogleAuthenticator))
                {
                    if (googleAuthOption == null)
                        result = false;
                    if (!CheckCodeValid(googleVerifier, plateForm, uniqueKey, googleAuthOption.Code))
                    {
                        googleCodeBeUsed = true;
                        result = false;
                    }

                    result = result && googleVerifier.Verify(serects.GoogleAuthSecretKey, googleAuthOption.Code);
                    if (!result)
                        erroredFlag = (byte)ValidationFlag.GooogleAuthenticator;
                }
            }

            if (result)
            {
                InvalidateCode(cellphoneVerifier, plateForm, divisionCode + uniqueKey);
                if (googleAuthOption != null)
                    InvalidateCode(googleVerifier, plateForm, uniqueKey, googleAuthOption.Code);
                if (errorCount > 0)
                    DeleteErrorCount(errorCountKey);
            }
            else
            {
                if (googleCodeBeUsed)
                    googleVerifier.VerifyInvalidCode();
                IncreaseCombinedErrorCount(combinedVerifier, errorCountKey, erroredFlag);
            }
        }

        ///// <summary>
        ///// 组合验证，自定义验证组合
        ///// </summary>
        ///// <param name="plateForm">系统</param>
        ///// <param name="business">业务</param>
        ///// <param name="uniqueKey">唯一标志</param>
        ///// <param name="fun">自定义验证方法</param>
        //public static void Verify(SystemPlatform plateForm, FiiipayBusiness business, string uniqueKey, Func<bool> fun)
        //{
        //    var combinedVerifier = new CombinedVerifier();
        //    var errorCountKey = combinedVerifier.GetErrorCountKey(plateForm, "CombinedVerifier", business, uniqueKey);
        //    int errorCount = CheckErrorCount(combinedVerifier, business, errorCountKey);

        //    var result = fun();

        //    if (result)
        //    {
        //        if (errorCount > 0)
        //            DeleteErrorCount(errorCountKey);
        //    }
        //    else
        //    {
        //        IncreaseCombinedErrorCount(business, errorCountKey, null);
        //    }
        //}

        /// <summary>
        /// 删除错误验证次数，用于解除多次错误的锁定时间
        /// </summary>
        /// <typeparam name="T">验证类</typeparam>
        /// <param name="plateForm">系统</param>
        /// <param name="business">业务</param>
        /// <param name="uniqueKey">唯一标志</param>
        public static void DeleteErrorCount(IVerifier verifier, SystemPlatform plateForm,string uniqueKey)
        {
            var errorCountKey = verifier.GetErrorCountKey(plateForm, uniqueKey);
            DeleteErrorCount(errorCountKey);
        }

        ///// <summary>
        ///// 使验证码无效
        ///// </summary>
        ///// <typeparam name="T">验证类</typeparam>
        ///// <param name="plateForm">系统</param>
        ///// <param name="business">业务</param>
        ///// <param name="uniqueKey">唯一标志</param>
        ///// <param name="code">验证码，计算类型的验证类必传</param>
        //public static void InvalidateCode<T>(SystemPlatform plateForm, FiiipayBusiness business, string uniqueKey, string code) where T : AbstractVerifier, new()
        //{
        //    var verifier = new T();
        //    InvalidateCode(verifier, plateForm, business, uniqueKey, code);
        //}

        /// <summary>
        /// 检查验证码是否有效(已经成功验证则无效)
        /// </summary>
        /// <param name="plateForm">系统</param>
        /// <param name="uniqueKey">唯一标志</param>
        /// <param name="code">验证码，计算类型的验证类必传</param>
        /// <returns></returns>
        public static bool CheckCodeValid(ComputedValueVerifier verifier,SystemPlatform plateForm, string uniqueKey, string code)
        {
            string invalidCodeKey = verifier.GetBeUsedKey(plateForm, uniqueKey, code);
            return !RedisHelper.KeyExists(RedisDBIndex, invalidCodeKey);
        }

        public static void InvalidateCode(CustomVerifier verifier, SystemPlatform plateForm, string uniqueKey)
        {
            string modelKey = verifier.GetVerifyModelKey(plateForm, uniqueKey);
            RedisHelper.KeyDelete(RedisDBIndex, modelKey);
        }

        public static void InvalidateCode(ComputedValueVerifier verifier, SystemPlatform plateForm, string uniqueKey, string code)
        {
            RedisHelper.StringSet(RedisDBIndex, verifier.GetBeUsedKey(plateForm, uniqueKey, code), "1", TimeSpan.FromMinutes(verifier.GetExpireTime()));
        }

        public static void InvalidateCode(RandomValueVerifier verifier, SystemPlatform plateForm, string uniqueKey)
        {
            RedisHelper.KeyDelete(RedisDBIndex, verifier.GetCodeKey(plateForm, uniqueKey));
        }
        
        public static int CheckErrorCount(IVerifier verifier, string errorCountKey)
        {
            int.TryParse(RedisHelper.StringGet(RedisDBIndex, errorCountKey), out int errorCount);

            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(errorCountKey);
                verifier.FailedMoreTimes(minCount);
            }
            return errorCount;
        }

        public static void DeleteErrorCount(string errorCountKey)
        {
            RedisHelper.KeyDelete(RedisDBIndex, errorCountKey);
        }

        public static void IncreaseErrorCount(IVerifier verifier,string errorCountKey, Exception ex=null)
        {
            var errorCountsStr = RedisHelper.StringGet(RedisDBIndex, errorCountKey);
            int.TryParse(errorCountsStr, out int errorCount);
            ++errorCount;
            int spInt = verifier.GetLockTime();
            RedisHelper.StringSet(RedisDBIndex, errorCountKey, errorCount.ToString(), TimeSpan.FromMinutes(spInt));
            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(errorCountKey);
                verifier.FailedMoreTimes(minCount);
            }
            else
            {
                if (ex != null)
                    throw ex;
                verifier.VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount);
            }
        }
        public static void IncreaseCombinedErrorCount(CombinedVerifier verifier, string errorCountKey, byte errorFlag)
        {
            var errorCountsStr = RedisHelper.StringGet(RedisDBIndex, errorCountKey);
            int.TryParse(errorCountsStr, out int errorCount);
            ++errorCount;
            int spInt = verifier.GetLockTime();
            RedisHelper.StringSet(RedisDBIndex, errorCountKey, errorCount.ToString(), TimeSpan.FromMinutes(spInt));
            if (errorCount >= Constant.VIRIFY_FAILD_TIMES_LIMIT)
            {
                var minCount = GetErrorLockTime(errorCountKey);
                verifier.FailedMoreTimes(minCount);
            }
            else
            {
                verifier.VerifyFaild(Constant.VIRIFY_FAILD_TIMES_LIMIT - errorCount, errorFlag);
            }
        }

        public static int GetErrorLockTime(string errorKey)
        {
            var ts = RedisHelper.KeyTimeToLive(RedisDBIndex, errorKey);
            var minCount = ts.HasValue ? Math.Ceiling(ts.Value.TotalMinutes) : 1;
            return Convert.ToInt32(minCount);
        }

        public static bool CheckSecurityOpened(byte? validateValue, ValidationFlag validationFlag)
        {
            if (!validateValue.HasValue || validateValue.Value == 0)
                return false;
            ValidationFlag validate = (ValidationFlag)Enum.Parse(typeof(ValidationFlag), validateValue.Value.ToString());
            return (validate & validationFlag) != 0;
        }
    }
}
