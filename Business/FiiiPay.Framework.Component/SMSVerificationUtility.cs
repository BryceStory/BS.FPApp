using System;
using FiiiPay.Data.Agents;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;
using Newtonsoft.Json;

namespace FiiiPay.Business
{
    public class SMSVerificationUtility
    {
        /// <summary>
        /// 每天单个手机发送验证短信限额
        /// </summary>
        private static readonly int SEND_LIMIT = string.IsNullOrWhiteSpace(Constant.SEND_SMS_LIMIT) ? 20 : Convert.ToInt32(Constant.SEND_SMS_LIMIT);

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="cellphone">手机号</param>
        /// <param name="platform">平台 (FiiiPay, FiiiPOS)</param>
        /// <param name="business">业务 (功能业务 如: Signup)</param>
        /// <param name="uniqueKey">唯一标志，用于缓存短信码</param>
        /// <param name="formattContent">格式化短信内容 包含两个占位符({0} 验证码, {1} 过期时间)</param>
        /// <param name="extensions">存储扩展信息</param>
        public static void Send(string cellphone, string platform, string business, string uniqueKey, string formattContent, object extensions = null)
        {
            if (string.IsNullOrEmpty(cellphone))
                throw new ArgumentNullException(nameof(cellphone));
            if (string.IsNullOrEmpty(platform))
                throw new ArgumentNullException(nameof(platform));
            if (string.IsNullOrEmpty(business))
                throw new ArgumentNullException(nameof(business));
            if (string.IsNullOrEmpty(uniqueKey))
                throw new ArgumentNullException(nameof(uniqueKey));

            //短信发送达到上限
            if (HasAttainLimit(cellphone, platform))
                throw new GeneralException(GeneralResources.SMSLimit);

            int expiredTime = Constant.SMS_EXPIRED_TIME;
            string code = GenerateCode(platform, business, uniqueKey, extensions);
            string content = string.Format(formattContent, code, expiredTime);
            new SMSAgent().Send(cellphone, content, 5);

            AddSendCount(cellphone, platform);
        }

        private static string GenerateCode(string platform, string business, string uniqueKey, object extensions)
        {
            string codeKey = $"{platform}:APP:SMSVerification:{business}:{uniqueKey}";
            string codeCacheString = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, codeKey);
            string code;
            if (!string.IsNullOrEmpty(codeCacheString))
            {
                CodeExtension<object> ext = JsonConvert.DeserializeObject<CodeExtension<object>>(codeCacheString);
                code = ext?.Code ?? RandomAlphaNumericGenerator.GenerateAllNumber(6);
            }
            else
            {
                code = RandomAlphaNumericGenerator.GenerateAllNumber(6);
            }

            CodeExtension<object> codeCache = new CodeExtension<object>
            {
                Code = code,
                Extension = extensions
            };

            RedisHelper.Set(codeKey, codeCache, TimeSpan.FromMinutes(Constant.SMS_EXPIRED_TIME));
            return code;
        }

        private static bool HasAttainLimit(string cellphone, string platform)
        {
            var sendCountKey = $"{platform}:APP:SMSVerification:SendCount:{cellphone}";
            RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, sendCountKey);
            int.TryParse(sendCountKey, out int sendCount);
            return sendCount >= SEND_LIMIT;
        }

        private static void AddSendCount(string cellphone, string platform)
        {
            var sendCountKey = $"{platform}:APP:SMSVerification:SendCount:{cellphone}";
            var sendCountStr = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, sendCountKey);
            int.TryParse(sendCountKey, out int sendCount);

            DateTime now = DateTime.Now;
            DateTime end = now.Date.AddDays(1).AddSeconds(-1);

            RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, sendCountKey, (sendCount + 1).ToString(), end - now);
        }

        public static bool Verify(string cellphone, string platform, string business, string code)
        {
            if (string.IsNullOrEmpty(cellphone))
                throw new ArgumentNullException(nameof(cellphone));
            if (string.IsNullOrEmpty(platform))
                throw new ArgumentNullException(nameof(platform));
            if (string.IsNullOrEmpty(business))
                throw new ArgumentNullException(nameof(business));
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            string codeKey = $"{platform}:APP:SMSVerification:{business}:{cellphone}";
            string codeCache = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, codeKey);

            bool result = codeCache != null && codeCache == code;

            //验证码校验成功即失效
            if (result)
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, codeKey);
            return result;
        }
        
        private class CodeExtension<T>
        {
            public string Code { get; set; }
            public T Extension { get; set; }
        }
    }
}