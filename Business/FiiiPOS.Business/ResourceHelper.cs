using FiiiPay.Framework;
using FiiiPOS.Business.Properties;

namespace FiiiPay.Business
{
    public class ResourceHelper
    {
        private static ResourceManager _fiiiPos;

        public static ResourceManager FiiiPos => _fiiiPos ?? (_fiiiPos = new ResourceManager(typeof(Resources)));
    }
}
