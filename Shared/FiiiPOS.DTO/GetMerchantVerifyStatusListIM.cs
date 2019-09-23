using System;
using System.Collections.Generic;

namespace FiiiPOS.DTO
{
    public class GetMerchantVerifyStatusListIM
    {
        public List<GetMerchantVerifyStatusListIMItem> List { get; set; }
    }

    public class GetMerchantVerifyStatusListIMItem
    {
        public Guid MerchantId { get; set; }
        public int CountryId { get; set; }
    }
}
