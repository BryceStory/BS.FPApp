using FiiiPay.Entities;
using System.Collections.Generic;

namespace FiiiPay.DTO.Account
{
    public class UserProfileListIM
    {
        public string Cellphone { get; set; }
        public int Country { get; set; }
        public string OrderByFiled { get; set; }
        public bool IsDesc { get; set; }
        public int? VerifyStatus { get; set; }
        public int PageSize { get; set; }
        public int Index { get; set; }
    }
    public class UserProfileListOM
    {
        public List<UserProfile> ResultSet { get; set; }
        public int TotalCount { get; set; }
    }
}
