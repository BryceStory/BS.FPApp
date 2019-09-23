using FiiiPay.Framework.Exceptions;

namespace FiiiPOS.Business
{
    public class InvalidProfileServiceException : CommonException
    {
        public InvalidProfileServiceException() : base(0, "Invalid profile server.")
        {

        }
    }
}
