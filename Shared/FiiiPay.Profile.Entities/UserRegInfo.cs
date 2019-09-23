using System;

namespace FiiiPay.Profile.Entities
{
    public class UserRegInfo
    {
        public Guid? UserAccountId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }    
        public string IdentityDocNo { get; set; }
        public byte IdentityDocType { get; set; }
        public string Postcode { get; set; }
        public int Country { get; set; }
        public string Password { get; set; }
        public string Cellphone { get; set; }
    }
}
