using FiiiPay.Framework.Exceptions;

namespace FiiiPay.Framework.Component.Exceptions
{
    public class FiiiFinanceException : CommonException
    {
        public FiiiFinanceException(int reasoncode, string message) : base(reasoncode, message)
        {
        }
    }
}