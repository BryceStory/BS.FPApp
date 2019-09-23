using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public abstract class CombinedVerifier : IVerifier
    {
        public abstract bool Verify(Func<bool> fun);
        public abstract void FailedMoreTimes(int minCount);
        public virtual void VerifyFaild(int remainCount)
        { }
        public virtual void VerifyFaild(int remainCount, byte erroredFlag)
        {

        }
        public abstract string GetBussinessName();
        public abstract string GetVerifyTypeName();

        public abstract int GetLockTime();
        public virtual string GetErrorCountKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{GetVerifyTypeName()}:ErrorCounts:{uniqueKey}";
        }
    }
}
