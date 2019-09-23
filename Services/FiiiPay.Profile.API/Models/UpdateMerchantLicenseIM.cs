using System;

namespace FiiiPay.Profile.API.Models
{
    public class UpdateMerchantLicenseIM
    {
        public Guid MerchantId { get; set; }
        public string CompanyName { get; set; }
        public string LicenseNo { get; set; }
        public Guid BusinessLicense { get; set; }
    }
}
