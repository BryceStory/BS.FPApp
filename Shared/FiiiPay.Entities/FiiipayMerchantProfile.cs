using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class FiiipayMerchantProfile
    {
        public Guid MerchantInfoId { get; set; }
        public string LicenseNo { get; set; }
        public Guid BusinessLicenseImage { get; set; }
    }
}
