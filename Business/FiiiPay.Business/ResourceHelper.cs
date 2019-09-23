using FiiiPay.Business.Properties;
using FiiiPay.Framework;

namespace FiiiPay.Business
{
    public class ResourceHelper
    {
        private static ResourceManager _fiiiPay;

        public static ResourceManager FiiiPay => _fiiiPay ?? (_fiiiPay = new ResourceManager(typeof(Resources)));
    }
}
