
using FiiiPay.Framework;
using FiiiPay.MessageWorkerService.Properties;

namespace FiiiPay.MessageWorkerService
{
    public class ResourceHelper
    {
        private static ResourceManager _fiiiPay;

        public static ResourceManager FiiiPay => _fiiiPay ?? (_fiiiPay = new ResourceManager(typeof(Resources)));
    }
}
