using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Exceptions;
using System;
using FiiiPay.Framework.Component.Properties;
using System.Collections.Generic;

namespace FiiiPay.Framework.Component.Verification
{
    /// <summary>
    /// 手机验证码通用验证
    /// </summary>
    public class CellphoneVerifier : RandomValueVerifier
    {
        public override string SendCode(SystemPlatform plateForm, string uniqueKey, string targetCellphone,string message=null)
        {
            var isTestSMS = System.Configuration.ConfigurationManager.AppSettings["Test_Sms"] == "1";
            var keyByCount = $"{plateForm}:CellphoneVerifier:SendCount:{uniqueKey}";
            var keyTimeInterval = $"{plateForm}:CellphoneVerifier:SendInterval:{GetBussinessName()}:{uniqueKey}";

            var sentCode = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, keyTimeInterval);
            if (!string.IsNullOrEmpty(sentCode))
                return null;

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

            //var keyByCode = base.GetCodeKey(plateForm, "CellphoneVerifier", business, uniqueKey);
            var code = string.Empty;
            //var codeInDb = RedisHelper.StringGet(Constant.REDIS_SMS_DBINDEX, keyByCode);
            //if (!string.IsNullOrEmpty(codeInDb))
            //{
            //    code = codeInDb;
            //}
            //else
            //{
            //    code = isTestSMS ? "123456" : new Random().Next(0, 1000000).ToString("000000");
            //}

            //暂定每次发送的验证码不同
            code = isTestSMS ? "123456" : new Random().Next(0, 1000000).ToString("000000");

            string template = string.Empty;
            switch (plateForm)
            {
                case SystemPlatform.FiiiPOS:
                    template = GeneralResources.SMS_Template_POS;
                    break;
                case SystemPlatform.FiiiPay:
                case SystemPlatform.FiiiShop:
                    template = GeneralResources.SMS_Template_FiiiPay;
                    break;
            }
            string msmStr = string.Format(template, code, Constant.SMS_EXPIRED_TIME);

            if (!isTestSMS)
                new Data.Agents.SMSAgent().Send(targetCellphone, msmStr, 5);

            Info($"SMSSend:PlateForm:{plateForm},Business:{GetBussinessName()},Cellphone:{targetCellphone},Content:{msmStr}");

            RedisHelper.StringSet(Constant.REDIS_SMS_DBINDEX, keyTimeInterval, code, new TimeSpan(0, 1, 0));
            return code;
        }

        public override int GetExpireTime()
        {
            return Constant.SMS_EXPIRED_TIME;
        }

        public override int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }

        public override bool Verify(SystemPlatform plateForm,string uniqueKey, string code)
        {
            var codeKey = GetCodeKey(plateForm, uniqueKey);
            var codeModel = RedisHelper.Get<RandomCodeModel>(Constant.REDIS_SMS_DBINDEX, codeKey);
            if (codeModel == null)
                return false;
            if (codeModel.ExpireTime <= DateTime.UtcNow.ToUnixTime())
                CodeExpried();
            return codeModel.Code != null && code == codeModel.Code;
        }

        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.PHONECODE_VERIFYFAILED_TOOMANY_TEIMS, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
        }

        public override void CodeExpried()
        {
            throw new CommonException(ReasonCode.CODE_EXPIRE, GeneralResources.EMSMSCodeExpire);
        }

        public override void VerifyFaild(int remainCount)
        {
            throw new CommonException(ReasonCode.WRONG_CODE_ENTERRED, string.Format(GeneralResources.EMSMSCodeError, remainCount));
        }

        public override void VerifyInvalidCode(int remainCount)
        {
            throw new CommonException(ReasonCode.WRONG_CODE_ENTERRED, string.Format(GeneralResources.EMSMSCodeError, remainCount));
        }

        public override string GetBussinessName()
        {
            return "CellphoneVerifier";
        }
        public override string GetVerifyTypeName()
        {
            return "CellphoneVerifier";
        }
    }
    public class RandomCodeModel
    {
        public string Code { get; set; }
        public long ExpireTime { get; set; }
    }

    /// <summary>
    /// 注册手机验证码
    /// </summary>
    public class RegisterCellphoneVerifier: CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "Register";
        }
        public override int GetLockTime()
        {
            return Constant.REGISTER_FAILD_LOCK_TIME;
        }
        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.PHONECODE_VERIFYFAILED_TOOMANY_TEIMS, string.Format(GeneralResources.EMRegisterVerifyLimit5Times, minCount));
        }
    }

    public class LoginCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "Login";
        }
        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.LOGIN_ERROR_TOO_MANY_TIMES_BYSMS, string.Format(GeneralResources.EMLoginSMSCodeTry5Times, minCount));
        }
    }

    public class ForgetPasswordCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "ForgetPassword";
        }
    }

    public class UpdatePinCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "UpdatePin";
        }
    }

    public class ResetPinCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "ResetPin";
        }
    }

    public class UpdatePasswordCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "UpdatePassword";
        }
    }
    public class UpdateCellphoneOldVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "UpdateCellphoneOld";
        }
    }
    public class UpdateCellphoneNewVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "UpdateCellphoneNew";
        }
    }
    public class MandatoryCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "MandatoryVerifier";
        }
    }
    public class BindAccountCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "BindAccount";
        }
    }
    public class FiiiPosRegisterVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "FiiiPosRegister";
        }
        public override int GetLockTime()
        {
            return Constant.REGISTER_FAILD_LOCK_TIME;
        }
        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.PHONECODE_VERIFYFAILED_TOOMANY_TEIMS, string.Format(GeneralResources.EMRegisterVerifyLimit5Times, minCount));
        }
        private string GetExtensionKey(SystemPlatform platform, string uniqueKey)
        {
            return $"{platform}:{GetBussinessName()}:Code:{uniqueKey}:Extension";
        }
        public void CacheRegisterModel(SystemPlatform platform, string uniqueKey, object extension)
        {
            var extensionKey = GetExtensionKey(platform, uniqueKey);
            RedisHelper.Set(Constant.REDIS_SMS_DBINDEX, extensionKey, extension, TimeSpan.FromMinutes(Constant.TEMPTOKEN_EXPIRED_TIME));
        }
        public Dictionary<string, string> GetRegisterModel(SystemPlatform platform, string uniqueKey,bool deleteCode)
        {
            var extensionKey = GetExtensionKey(platform, uniqueKey);
            var cacheExtension = RedisHelper.Get<Dictionary<string, string>>(Constant.REDIS_SMS_DBINDEX, extensionKey);

            if (deleteCode)
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, extensionKey);

            return cacheExtension;
        }
        public void DeleteCacheModel(SystemPlatform platform, string uniqueKey)
        {
            var extensionKey = GetExtensionKey(platform, uniqueKey);
            RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, extensionKey);
        }
    }

    /// <summary>
    /// Fiiipos修改手机号
    /// </summary>
    public class ModifyCellphoneVerifier : CellphoneVerifier
    {
        public override string GetBussinessName()
        {
            return "ModifyCellphone";
        }
    }
}
