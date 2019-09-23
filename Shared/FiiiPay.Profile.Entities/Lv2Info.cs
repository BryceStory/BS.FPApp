using System;

namespace FiiiPay.Profile.Entities
{
    public class Lv2Info
    {
        public Guid Id { get; set; }
        public Guid? ResidentImage { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public int Country { get; set; }
        public VerifyStatus? L2VerifyStatus { get; set; }
    }
}
