using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Framework.Component.Verification
{
    public class PinVerifier : FixValueVerifier
    {
        public override bool Verify(SystemPlatform plateForm,string uniqueKey, string hashedCode, string toCheckCode)
        {
            //string _pin = AES128.Decrypt(toCheckCode, AES128.DefaultKey);
            return PasswordHasher.VerifyHashedPassword(hashedCode, toCheckCode);
        }

        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.PIN_ERROR_5_TIMES, string.Format(GeneralResources.EMPINInputLimit, minCount));
        }

        public override void VerifyFaild(int remainCount)
        {
            throw new CommonException(ReasonCode.PIN_ERROR, string.Format(GeneralResources.EMPINInputError, remainCount));
        }

        public override string GetBussinessName()
        {
            return "PinVerifier";
        }
        public override string GetVerifyTypeName()
        {
            return "PinVerifier";
        }

        public override int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }
    }
}
