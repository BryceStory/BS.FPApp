namespace FiiiPay.Framework.Exceptions
{
    public class UnauthorizedException : CommonException
    {
        public UnauthorizedException() : base(Framework.ReasonCode.UNAUTHORIZED, "UNAUTHORIZED")
        {
            
        }
    }
}
