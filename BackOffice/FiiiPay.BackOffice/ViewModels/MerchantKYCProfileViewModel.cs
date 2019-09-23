using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class MerchantKYCProfileViewModel : Entities.MerchantProfile
    {
        public string CountryName { get; set; }
        public string IdentityDocTypeName { get; set; }
        public string L1VerifyStatusName { get; set; }
        public string L2VerifyStatusName { get; set; }
        public string L1SubmitTime { get; set; }
    }
}