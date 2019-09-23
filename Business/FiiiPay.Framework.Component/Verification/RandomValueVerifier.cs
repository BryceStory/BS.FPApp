using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public abstract class RandomValueVerifier : BaseComponent,IVerifier
    {
        public abstract string SendCode(SystemPlatform plateForm, string uniqueKey, string target,string message);
        public abstract bool Verify(SystemPlatform plateForm,string uniqueKey, string code);
        public abstract int GetExpireTime();
        public abstract void FailedMoreTimes(int minCount);
        public abstract void VerifyFaild(int remainCount);
        public abstract void CodeExpried();
        public abstract void VerifyInvalidCode(int remainCount);
        public abstract string GetBussinessName();
        public abstract string GetVerifyTypeName();

        public abstract int GetLockTime();

        public virtual string GetCodeKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{GetBussinessName()}:{GetVerifyTypeName()}:Code:{uniqueKey}";
        }
        public virtual string GetErrorCountKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{GetVerifyTypeName()}:ErrorCounts:{uniqueKey}";
        }
    }
}
