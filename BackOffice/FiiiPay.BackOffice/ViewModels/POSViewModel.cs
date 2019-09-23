using FiiiPay.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FiiiPay.Entities.Enums;

namespace FiiiPay.BackOffice.ViewModels
{
    public class POSViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Sn { get; set; }
        public string Cellphone { get; set; }
        public string MerchantName { get; set; }
        public string Email { get; set; }
        public int DefaultCryptoId { get; set; }
        public string DefaultCrypto { get; set; }
        public AccountStatus Status { get; set; }
        public bool IsAllowWithdrawal { get; set; }
        public bool IsAllowAcceptPayment { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public VerifyStatus L1VerifyStatus { get; set; }
        public VerifyStatus L2VerifyStatus { get; set; }
        public string LicenseNo { get; set; }
        public int CountryId { get; set; }
        public string CompanyName { get; set; }
        public Guid? BusinessLicenseImage { get; set; }

        public decimal Receivables_Tier { get; set; }
        public decimal Markup { get; set; }


        public string AuthSecretKey { get; set; }
        public byte ValidationFlag { get; set; }
    }    
}