using System.Collections.Generic;
using FiiiPay.Profile.Entities;

namespace FiiiPay.Profile.API.Models
{
    public class GetMerchnatVerifyListOM
    {
        public List<MerchantProfile> ResultSet { get; set; }
        public int TotalCount { get; set; }
    }
}
