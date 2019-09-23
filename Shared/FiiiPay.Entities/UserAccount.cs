using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public string PhoneCode { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public bool IsVerifiedEmail { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int CountryId { get; set; }
        public Guid? Photo { get; set; }
        public string Password { get; set; }
        public string Pin { get; set; }
        public string SecretKey { get; set; }
        public string AuthSecretKey { get; set; }
        public byte? Status { get; set; }
        public bool? IsAllowWithdrawal { get; set; }
        public bool? IsAllowExpense { get; set; }
        public bool? IsAllowTransfer { get; set; }
        public bool IsBindingDevice { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }

        public string InvitationCode { get; set; }
        public string InviterCode { get; set; }
        public byte ValidationFlag { get; set; }

        public string Language { get; set; }
        public VerifyStatus L1VerifyStatus { get; set; }
        public VerifyStatus L2VerifyStatus { get; set; }

        public string Nickname { get; set; }
    }
}
