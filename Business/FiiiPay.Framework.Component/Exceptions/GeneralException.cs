namespace FiiiPay.Framework.Exceptions
{
    public class GeneralException : CommonException
    {
        public GeneralException(string message) : base(Framework.ReasonCode.GENERAL_ERROR, message)
        {

        }
    }
}