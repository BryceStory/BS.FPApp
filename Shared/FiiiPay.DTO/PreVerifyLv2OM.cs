using System;

namespace FiiiPay.DTO.Profile
{
    public class PreVerifyLv2OM
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string CountryName { get; set; }
        public Guid? ResidentImage { get; set; }
    }
}
