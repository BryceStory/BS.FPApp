using FiiiPay.Framework;

namespace FiiiPay.Business
{
    internal class IdentityHelper
    {
        private static readonly Identity _identity = new Identity(90);

        public static string OrderNo()
        {
            return _identity.StringId();
        }
    }
}
