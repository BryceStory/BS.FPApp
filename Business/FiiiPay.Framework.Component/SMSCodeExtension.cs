using System;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;

namespace FiiiPay.Business
{
    public static class SMSCodeExtension
    {
        public static void SendWithExtension(this SMSCodeComponent scc, SMSBusiness business, string uniqueKey, string cellphone, object extension)
        {
            scc.Send(business, uniqueKey, cellphone);
            var cacheExtensionKey = $"{scc.Platform}:{business}:Code:{uniqueKey}:Extension";
            RedisHelper.Set(Constant.REDIS_SMS_DBINDEX, cacheExtensionKey, extension, TimeSpan.FromMinutes(Constant.TEMPTOKEN_EXPIRED_TIME));
        }

        public static T VerifyWithExtension<T>(this SMSCodeComponent scc, SMSBusiness business, string uniqueKey, string code, bool deleteCode)
        {
            scc.Verify(business, uniqueKey, code, deleteCode);

            var cacheExtensionKey = $"{scc.Platform}:{business}:Code:{uniqueKey}:Extension";
            var cacheExtension = RedisHelper.Get<T>(Constant.REDIS_SMS_DBINDEX, cacheExtensionKey);

            if (deleteCode)
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, cacheExtensionKey);

            return cacheExtension;
        }

        public static void DeleteCodeWithExtension(this SMSCodeComponent scc, SMSBusiness business, string uniqueKey)
        {
            scc.DeleteSMSCode(business, uniqueKey);
            var cacheExtensionKey = $"{scc.Platform}:{business}:Code:{uniqueKey}:Extension";
            RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, cacheExtensionKey);
        }
    }
}