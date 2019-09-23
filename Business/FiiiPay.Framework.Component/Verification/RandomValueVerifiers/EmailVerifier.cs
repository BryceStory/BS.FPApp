using FiiiPay.Data.Agents;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public class EmailVerifier : RandomValueVerifier
    {
        public override string SendCode(SystemPlatform plateForm, string uniqueKey, string targetEmail,string subject)
        {
            var isTestSMS = System.Configuration.ConfigurationManager.AppSettings["Test_Email"] == "1";
            var code = isTestSMS ? "123456" : new Random().Next(0, 1000000).ToString("000000");
            string merchant="";
            if (plateForm == SystemPlatform.FiiiPay)
                merchant = "FiiiPay";
            else if (plateForm == SystemPlatform.FiiiPOS)
                merchant = "FiiiPOS";
            new EmailAgent().Send(targetEmail, subject, string.Format(GeneralResources.EmailVerfiyMessage, code, Constant.EMAIL_EXPIRED_TIME), 5, merchant);
            return code;
        }
        public override int GetExpireTime()
        {
            return Constant.EMAIL_EXPIRED_TIME;
        }

        public override int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }

        public override bool Verify(SystemPlatform plateForm, string uniqueKey, string code)
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
            throw new CommonException(ReasonCode.FiiiPosReasonCode.EMAILCODE_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyFaildMoreTimes,minCount));
        }

        public override void CodeExpried()
        {
            throw new CommonException(ReasonCode.CODE_EXPIRE, GeneralResources.EMSMSCodeExpire);
        }

        public override void VerifyFaild(int remainCount)
        {
            throw new CommonException(ReasonCode.WRONG_CODE_ENTERRED, string.Format(GeneralResources.EMVerifyFaild, remainCount));
        }

        public override void VerifyInvalidCode(int remainCount)
        {
            throw new CommonException(ReasonCode.WRONG_CODE_ENTERRED, string.Format(GeneralResources.EMVerifyFaild, remainCount));
        }

        public override string GetBussinessName()
        {
            return "EmailVerifier";
        }
        public override string GetVerifyTypeName()
        {
            return "EmailVerifier";
        }
    }

    public class UpdateEmailOriginalVerifier : EmailVerifier
    {
        public override string GetBussinessName()
        {
            return "UpdateEmailOriginal";
        }
    }

    public class UpdateEmailNewVerifier : EmailVerifier
    {
        public override string GetBussinessName()
        {
            return "UpdateEmailNew";
        }
    }

    public class SetEmailVerifier : EmailVerifier
    {
        public override string GetBussinessName()
        {
            return "SetEmail";
        }
    }
}
