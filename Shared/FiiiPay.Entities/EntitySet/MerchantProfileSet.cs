using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities.EntitySet
{
    public class MerchantProfileSet
    {
        public Guid? Id { get; set; }
        public string Cellphone { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string Username { get; set; }
        public string MerchantName { get; set; }
        public string LastName { set; get; }
        public string FirstName { set; get; }
        public string IdentityDocNo { set; get; }
        public IdentityDocType? IdentityDocType { set; get; }
        public DateTime? IdentityExpiryDate { set; get; }
        public string SN { get; set; }
        //public bool? IsVerified { get; set; }
        public long? POSId { get; set; }
        public int? BeaconId { get; set; }
        public string Email { get; set; }
        public bool? IsVerifiedEmail { get; set; }
        public int? CountryId { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Photo { get; set; }
        public string PIN { get; set; }
        public string SecretKey { get; set; }
        public VerifyStatus L1VerifyStatus { get; set; }
        public VerifyStatus L2VerifyStatus { get; set; }
        public bool? IsAllowWithdrawal { get; set; }
        public bool? IsAllowAcceptPayment { get; set; }
        public string CountryName { get; set; }
        public string CompanyName { get; set; }
        
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }
        ///////////////////////////////////////////////////////
        public Guid? MerchantId { set; get; }
        public string Address1 { set; get; }
        public string Address2 { set; get; }
        public string City { set; get; }
        public string State { set; get; }
        public string Postcode { set; get; }
        public int Country { set; get; }
        public Guid? BusinessLicenseImage { set; get; }
        public string LicenseNo { set; get; }
        /// <summary>
        /// Google验证
        /// </summary>
        public string AuthSecretKey { get; set; }
        public byte ValidationFlag { get; set; }

        public Guid? FrontIdentityImage { get; set; }
        public Guid? BackIdentityImage { get; set; }
        public Guid? HandHoldWithCard { get; set; }

    }
}
