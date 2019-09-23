using FiiiPay.Business.Properties;
using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Business
{
    public class SystemErrorException : CommonException
    {
        public SystemErrorException() : base(10000, MessageResources.SystemError)
        {

        }
    }
}
