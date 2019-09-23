using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public abstract class ComputedValueVerifier : IVerifier
    {
        public abstract bool Verify(string secretKey, string code);
        public abstract int GetExpireTime();
        public abstract void FailedMoreTimes(int minCount);
        public abstract void VerifyFaild(int remainCount);
        public abstract void VerifyInvalidCode();
        public abstract string GetBussinessName();
        public abstract string GetVerifyTypeName();

        public abstract int GetLockTime();
        public virtual string GetErrorCountKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{GetVerifyTypeName()}:ErrorCounts:{uniqueKey}";
        }
        public virtual string GetBeUsedKey(SystemPlatform plateForm, string uniqueKey, string code)
        {
            return $"{plateForm}:{GetVerifyTypeName()}:Beused:{uniqueKey}:{code}";
        }
    }
}
