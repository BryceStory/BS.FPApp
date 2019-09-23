using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Framework.Component.Verification
{
    public class GoogleVerifier : ComputedValueVerifier
    {
        public override int GetExpireTime()
        {
            return 3;
        }

        public override bool Verify(string secretKey, string code)
        {
            if(string.IsNullOrEmpty(secretKey))
                throw new CommonException(ReasonCode.GOOGLEAUTH_VERIFY_FAIL, GeneralResources.GACodeError);
            return new GoogleAuthenticator().ValidatePIN(secretKey, code);
        }

        public override void FailedMoreTimes(int minCount)
        {
            throw new CommonException(ReasonCode.GOOGLEAUTH_ERROR_TOO_MANY_TIMES, string.Format(GeneralResources.EMVerifyLimit5Times, minCount));
        }

        public override void VerifyFaild(int remainCount)
        {
            throw new CommonException(ReasonCode.GOOGLEAUTH_VERIFY_FAIL, string.Format(GeneralResources.EMGoogleCodeError, remainCount));
        }

        public override void VerifyInvalidCode()
        {
            throw new CommonException(ReasonCode.GOOGLECODE_BEUSED, GeneralResources.EMGoogleCodeBeUsed);
        }

        /// <summary>
        /// Google验证通过后，当前验证码在所有业务中无效
        /// </summary>
        /// <param name="plateForm"></param>
        /// <param name="subName"></param>
        /// <param name="business"></param>
        /// <param name="uniqueKey"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public override string GetBeUsedKey(SystemPlatform plateForm, string uniqueKey, string code)
        {
            return $"{plateForm}:GoogleVerifier:Beused:{uniqueKey}:{code}";
        }

        public override string GetBussinessName()
        {
            return "GoogleVerifier";
        }

        public override string GetVerifyTypeName()
        {
            return "GoogleVerifier";
        }

        public override int GetLockTime()
        {
            return Constant.VIRIFY_FAILD_LOCK_TIME;
        }
    }
}
