using FiiiPay.Framework;
using FiiiPos.MessageWorkerService.Properties;

namespace FiiiPos.MessageWorkerService
{
    public class ResourceHelper
    {
        private static ResourceManager _fiiiPos;

        public static ResourceManager FiiiPos => _fiiiPos ?? (_fiiiPos = new ResourceManager(typeof(Resources)));
    }
}
