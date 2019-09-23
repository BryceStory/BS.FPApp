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
    public class CustomVerifier : IVerifier
    {
        private string BussniessName;
        public CustomVerifier(string bussniessName)
        {
            BussniessName = bussniessName;
        }
        public void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.VERIFY_FAILD_MORETIMES, string.Format(GeneralResources.EMVerifyFaildMoreTimes, minCount));
        }

        public int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }

        public void VerifyFaild(int remainCount)
        {
            throw new CommonException(ReasonCode.VERIFY_FAILD, string.Format(GeneralResources.EMVerifyFaild, remainCount));
        }
        public void CodeExpried()
        {
            throw new CommonException(ReasonCode.CODE_EXPIRE, GeneralResources.EMSMSCodeExpire);
        }

        public string GetErrorCountKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{BussniessName}:ErrorCounts:{uniqueKey}";
        }

        public virtual string GetVerifyModelKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{BussniessName}:VerifyModel:{uniqueKey}";
        }
    }
}
