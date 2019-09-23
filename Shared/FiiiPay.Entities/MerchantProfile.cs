using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class MerchantProfile
    {
        public Guid MerchantId { set; get; }
        public string LastName { set; get; }
        public string FirstName { set; get; }
        public VerifyStatus L1VerifyStatus { set; get; }
        public VerifyStatus L2VerifyStatus { set; get; }
        public string IdentityDocNo { set; get; }
        public IdentityDocType? IdentityDocType { set; get; }
        public DateTime? IdentityExpiryDate { set; get; }
        /// <summary>
        /// L1认证提交时间
        /// </summary>
        public DateTime? L1SubmissionDate { get; set; }
        public DateTime? L2SubmissionDate { get; set; }
        public string Address1 { set; get; }
        public string Address2 { set; get; }
        public string City { set; get; }
        public string State { set; get; }
        public string Postcode { set; get; }
        public int Country { set; get; }
        public Guid? BusinessLicenseImage { set; get; }
        public string LicenseNo { set; get; }
        public string CompanyName { get; set; }
        public string L1Remark { get; set; }
        public string L2Remark { get; set; }
        public string Cellphone { get; set; }
        public Guid? FrontIdentityImage { get; set; }
        public Guid? BackIdentityImage { get; set; }
        public Guid? HandHoldWithCard { get; set; }
        
    }
}
