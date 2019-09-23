using System;

namespace FiiiPay.Profile.Entities
{
    public class Lv1Info
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte IdentityDocType { get; set; }
        public string IdentityDocNo { get; set; }
        public Guid? FrontIdentityImage { get; set; }
        public Guid? BackIdentityImage { get; set; }
        public Guid? HandHoldWithCard { get; set; }
        public VerifyStatus? L1VerifyStatus { get; set; }
    }
}
