using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Framework.Component.Verification
{
    public class IDNumberVerifier : FixValueVerifier
    {
        public override bool Verify(SystemPlatform plateForm, string uniqueKey, string oldCode, string toCheckCode)
        {
            if (string.IsNullOrEmpty(toCheckCode))
                return false;
            return toCheckCode.Trim().Equals(oldCode.Trim(), System.StringComparison.CurrentCultureIgnoreCase);
        }

        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.IdentityDocNo_Error_5Times, string.Format(GeneralResources.EMIdentityNoError, minCount));
        }

        public override void VerifyFaild(int remainCount)
        {
            throw new CommonException(ReasonCode.IdentityDocNo_NOT_RIGHT, string.Format(GeneralResources.EMIdentityNoError, remainCount));
        }

        public override string GetBussinessName()
        {
            return "IDNumberVerifier";
        }
        public override string GetVerifyTypeName()
        {
            return "IDNumberVerifier";
        }

        public override int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }
    }
}
