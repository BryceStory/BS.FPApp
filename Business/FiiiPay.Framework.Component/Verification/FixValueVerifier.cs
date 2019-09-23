using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public abstract class FixValueVerifier : BaseComponent, IVerifier
    {
        public abstract bool Verify(SystemPlatform plateForm, string uniqueKey,string hashedCode, string toCheckCode);

        public abstract void FailedMoreTimes(int minCount);

        public abstract void VerifyFaild(int remainCount);
        public abstract int GetLockTime();
        public abstract string GetBussinessName();
        public abstract string GetVerifyTypeName();

        public virtual string GetErrorCountKey(SystemPlatform plateForm, string uniqueKey)
        {
            return $"{plateForm}:{GetVerifyTypeName()}:ErrorCounts:{uniqueKey}";
        }
    }
}
