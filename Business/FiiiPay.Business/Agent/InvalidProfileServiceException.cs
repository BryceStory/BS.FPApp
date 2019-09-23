using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Data.Agents
{
    public class InvalidProfileServiceException : CommonException
    {
        public InvalidProfileServiceException() : base(0,"Invalid profile server.")
        {

        }
    }
}
