using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Verification
{
    public interface IVerifier
    {
        void FailedMoreTimes(int minCount);
        void VerifyFaild(int remainCount);
        int GetLockTime();
        string GetErrorCountKey(SystemPlatform plateForm, string uniqueKey);
    }
}
