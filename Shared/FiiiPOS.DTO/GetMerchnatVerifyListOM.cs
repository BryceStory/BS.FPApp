using FiiiPay.Entities;
using System.Collections.Generic;

namespace FiiiPOS.DTO
{
    public class GetMerchnatVerifyListOM
    {
        public List<MerchantProfile> ResultSet { get; set; }
        public int TotalCount { get; set; }
    }
}
