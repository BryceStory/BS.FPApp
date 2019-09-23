using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class GatewayAccountViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Password { get; set; }
        public string LicenseNo { get; set; }
        public Guid BusinessLicenseImage { set; get; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string IdentityDocNo { get; set; }
        public byte IdentityDocType { get; set; }
        public Guid FrontIdentityImage { get; set; }
        public Guid BackIdentityImage { get; set; }
        public Guid HandHoldWithCard { get; set; }
    }
}