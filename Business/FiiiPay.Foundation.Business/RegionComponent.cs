using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Foundation.Business
{
    public class RegionComponent
    {
        private RegionDAC rDAC = new RegionDAC();
        public async Task<IEnumerable<Regions>> GetStateListAsync(int countryId)
        {
            return await rDAC.GetStateListAsync(countryId);
        }

        public async Task<IEnumerable<Regions>> GetCityListAsync(long parentId)
        {
            return await rDAC.GetListAsync(parentId, RegionLevel.City);
        }
    }
}
