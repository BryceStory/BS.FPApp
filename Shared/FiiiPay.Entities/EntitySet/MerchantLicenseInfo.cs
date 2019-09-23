using System;

namespace FiiiPay.Entities.EntitySet
{
    public class MerchantLicenseInfo
    {
        public Guid Id { get; set; }
        public Guid BusinessLicenseImage { get; set; }
        public string MerchantName { get; set; }
        public string LicenseNo { get; set; }

    }
}
