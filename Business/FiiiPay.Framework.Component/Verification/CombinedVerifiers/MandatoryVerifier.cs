using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Enums;
using FiiiPay.Framework.Exceptions;
using System;

namespace FiiiPay.Framework.Component.Verification
{
    public class MandatoryVerifier: CombinedVerifier
    {
        public override bool Verify(Func<bool> fun)
        {
            return fun();
        }
        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.SECURITY_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
        }

        public override void VerifyFaild(int remainCount,byte erroredFlag)
        {
            switch (erroredFlag)
            {
                case (byte)ValidationFlag.Cellphone:
                    throw new CommonException(ReasonCode.WRONG_SECURITYPHONECODE_ENTERRED, string.Format(GeneralResources.EMSMSCodeError, remainCount));
                case (byte)ValidationFlag.GooogleAuthenticator:
                    throw new CommonException(ReasonCode.WRONG_SECURITYGOOGLECODE_ENTERRED, string.Format(GeneralResources.EMGoogleCodeError, remainCount));
                case (byte)ValidationFlag.IDNumber:
                    throw new CommonException(ReasonCode.WRONG_SECURITYGOOGLECODE_ENTERRED, string.Format(GeneralResources.EMIdentityNoError, remainCount));
                default:
                    throw new CommonException(ReasonCode.FAIL_AUTHENTICATOR, string.Format(GeneralResources.SecurityValidateError, remainCount));
            }
        }

        public override string GetBussinessName()
        {
            return "MandatoryVerifier";
        }
        public override string GetVerifyTypeName()
        {
            return "MandatoryVerifier";
        }

        public override int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }
    }
}
