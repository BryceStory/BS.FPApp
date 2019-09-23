using FiiiPay.Framework.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiiiPay.Entities;
using FiiiPay.Data;

namespace FiiiPay.Business
{
    public class AdvertisingComponent : BaseComponent
    {
        public async Task<Advertising> GetActiveSingle()
        {
            return await new AdvertisingDAC().GetActiveSingle();
        }

        public async Task<List<StoreBanner>> GetStoreBanners(int countryId)
        {
            return await new StoreBannerDAC().GetAliveStoreBannerAsync(countryId);
        }
    }
}
