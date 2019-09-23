using System;

namespace FiiiPay.DTO.Profile
{
    public class Lv1InfoIM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte IdType { get; set; }
        public string IdNumber { get; set; }
        public Guid? FrontIdentityImage { get; set; }
        public Guid? BackIdentityImage { get; set; }
        public Guid? HandHoldWithCard { get; set; }
    }
}
