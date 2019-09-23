using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities.EntitySet
{
    public class UserProfileSet
    {
        public Guid? UserAccountId { get; set; }
        public string Cellphone { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        //public string Fullname { get; set; }
        public string IdentityDocNo { get; set; }
        public IdentityDocType? IdentityDocType { get; set; }
        public string IdentityDocFile { get; set; }
        public DateTime? IdentityExpiryDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// L1认证提交时间
        /// </summary>
        public DateTime? L1SubmissionDate { get; set; }
        public DateTime? L2SubmissionDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public int? Country { get; set; }
        public byte? Gender { get; set; }
        public Guid? FrontIdentityImage { get; set; }
        public Guid? ResidentImage { get; set; }
        public Guid? BackIdentityImage { get; set; }
        public Guid? HandHoldWithCard { get; set; }
        //public int? VerifyStatus { get; set; }
        public string Remark { get; set; }
        public VerifyStatus? L1VerifyStatus { get; set; }
        public VerifyStatus? L2VerifyStatus { get; set; }
        ////
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool? IsVerifiedEmail { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }
        public Guid? Photo { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
        public string SecretKey { get; set; }
        public byte? Status { get; set; }
        public bool? IsAllowWithdrawal { get; set; }
        public bool? IsAllowExpense { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }
        public string L1Remark { get; set; }
        public string L2Remark { get; set; }

        public string InvitationCode { get; set; }
        public string InviterCode { get; set; }
        /// <summary>
        /// Google验证
        /// </summary>
        public string AuthSecretKey { get; set; }
        public byte ValidationFlag { get; set; }
    }
}
