using FiiiPay.Data.Agents;
using FiiiPay.Framework;
using System;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Business
{
    public class SMSCodeComponent : BaseComponent
    {
        internal SystemPlatform Platform { get; set; }
        public SMSCodeComponent(SystemPlatform platform)
        {
            Platform = platform;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="business">短信业务类型</param>
        /// <param name="uniqueKey">唯一标志，用于缓存短信码</param>
        /// <param name="cellphone">手机号</param>
        [Obsolete("请使用SecurityVerify.SendCode()")]
        public void Send(SMSBusiness business, string uniqueKey, string cellphone)
        {
            new SecurityVerification(Platform).CheckErrorCount(business, uniqueKey);

            var isTestSMS = System.Configuration.ConfigurationManager.AppSettings["Test_Sms"] == "1";
            var keyByCount = $"{Platform}:SMSSend:Count:{cellphone}";
            var keyTimeInterval = $"{Platform}:SMSSend:Interval:{cellphone}:{business}";

            var interval = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, keyTimeInterval);
            if (interval == "1")
                return;

            var limit = string.IsNullOrWhiteSpace(Constant.SEND_SMS_LIMIT) ? 20 : Convert.ToInt32(Constant.SEND_SMS_LIMIT);

            var countStr = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, keyByCount);
            int.TryParse(countStr, out int count);
            if (count >= limit)
            {
                throw new CommonException(ReasonCode.PHONECODE_SEND_TOOMANY_TIMES, GeneralResources.SMSLimit);
            }
            var dateNow = DateTime.Now;
            var tomorrowStartTime = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day).AddDays(1);
            RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, keyByCount, (count + 1).ToString(), tomorrowStartTime - dateNow);//一天内限制发送条数，每天重置

            var keyByCode = $"{Platform}:{SecurityMethod.CellphoneCode}:{business}:Code:{uniqueKey}";

            var code = string.Empty;
            //var codeInDb = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, keyByCode);
            //if (!string.IsNullOrEmpty(codeInDb))
            //{
            //    code = codeInDb;
            //}
            //else
            //{
            //    code = isTestSMS ? "123456" : new Random().Next(0, 1000000).ToString("000000");
            //    RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, keyByCode, code, TimeSpan.FromMinutes(Constant.SMS_EXPIRED_TIME));
            //}

            //暂定每次发送的验证码不同
            code = isTestSMS ? "123456" : new Random().Next(0, 1000000).ToString("000000");
            RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, keyByCode, code, TimeSpan.FromMinutes(Constant.SMS_EXPIRED_TIME));

            string template = string.Empty;
            switch (Platform)
            {
                case SystemPlatform.FiiiPOS:
                    template = GeneralResources.SMS_Template_POS;
                    break;
                case SystemPlatform.FiiiPay:
                    template = GeneralResources.SMS_Template_FiiiPay;
                    break;
            }

            string msmStr = string.Format(template, code, Constant.SMS_EXPIRED_TIME);

            if (!isTestSMS)
                new SMSAgent().Send(cellphone, msmStr, 5);

            Info($"SMSSend:PlateForm:{Platform},Business:{business},Cellphone:{cellphone},Content:{msmStr}");

            RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, keyTimeInterval, "1", new TimeSpan(0, 1, 0));
        }

        /// <summary>
        /// 验证短信
        /// </summary>
        /// <param name="business">短信业务类型</param>
        /// <param name="uniqueKey">唯一标志，用于获取缓存短信码</param>
        /// <param name="code">短信码</param>
        /// <param name="deleteCode">是否删除短信码，短信码最多有两次使用次数</param>
        [Obsolete("请使用SecurityVerify.Verify()")]
        public void Verify(SMSBusiness business, string uniqueKey, string code, bool deleteCode)
        {
            bool isOnlyCellphoneVerify = business != SMSBusiness.SecurityValidate;
            var securityVerify = new SecurityVerification(Platform);

            if (isOnlyCellphoneVerify)
                securityVerify.CheckErrorCount(business, uniqueKey);

            var keyByCode = $"{Platform.ToString()}:{SecurityMethod.CellphoneCode.ToString()}:{business.ToString()}:Code:{uniqueKey}";
            var codeInDb = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, keyByCode);
            if (codeInDb != null && codeInDb == code)//验证通过
            {
                if (deleteCode)
                {
                    DeleteSMSCode(business, uniqueKey);
                }
                else
                {
                    //如果验证通过，并且不删除这个验证码，表示以后还要用这个验证码验证
                    RedisHelper.KeyExpire(keyByCode, TimeSpan.FromMinutes(Constant.SMS_EXPIRED_TIME));
                }
                if (isOnlyCellphoneVerify)
                    securityVerify.DeleteErrorCount(business, uniqueKey);
                return;
            }
            if (isOnlyCellphoneVerify)
                securityVerify.IncreaseErrorCount(business, uniqueKey);
            else
                securityVerify.IncreaseErrorCount(SecurityMethod.SecurityValidate, uniqueKey, SecurityMethod.CellphoneCode);
        }

        public void DeleteSMSCode(SMSBusiness business, string uniqueKey)
        {
            var keyByCode = $"{Platform.ToString()}:{SecurityMethod.CellphoneCode.ToString()}:{business.ToString()}:Code:{uniqueKey}";
            RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, keyByCode);
        }
    }
}
