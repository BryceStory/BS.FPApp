using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class FiiipayMerchantVerifyRecord
    {
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid MerchantInfoId { get; set; }
        public string LicenseNo { get; set; }
        public Guid BusinessLicenseImage { get; set; }
        public VerifyStatus VerifyStatus { get; set; }
        public DateTime? VerifyTime { get; set; }
        public string Auditor { get; set; }
        public string Message { get; set; }
    }
}
