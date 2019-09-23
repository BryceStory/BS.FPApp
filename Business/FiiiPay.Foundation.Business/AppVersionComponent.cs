using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Business
{
    public class AppVersionComponent
    {
        public AppVersion GetLatestByPlatform(byte platform, byte app)
        {
            return new AppVersionDAC().GetLatestByPlatform(platform, app);
        }
    }
}
