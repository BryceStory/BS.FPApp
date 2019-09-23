using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Models
{
    [SugarTable("Profiles")]
    public class GatewayProfile
    {
        public Guid AccountId { get; set; }

        public string CompanyName { get; set; }

        public string Cellphone { get; set; }

        public string LicenseNo { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string IdentityDocNo { get; set; }

        public byte IdentityDocType { get; set; }

        public DateTime IdentityExpiryDate { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Postcode { get; set; }

        public int Country { get; set; }

        public Guid FrontIdentityImage { get; set; }

        public Guid BackIdentityImage { get; set; }
        public Guid HandHoldWithCard { get; set; }

        public Guid BusinessLicenseImage { get; set; }
    }
}