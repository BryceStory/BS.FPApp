using FiiiPay.Framework.Component.Properties;

namespace FiiiPay.Framework.Exceptions
{
    public class AccessTokenExpireException : CommonException
    {
        public AccessTokenExpireException() : base(Framework.ReasonCode.ACCESSTOKEN_EXPIRE, GeneralResources.EMAccessTokenExpire)
        {

        }
    }
}
