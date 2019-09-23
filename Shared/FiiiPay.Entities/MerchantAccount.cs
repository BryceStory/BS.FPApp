using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class MerchantAccount
    {
        public Guid Id { get; set; }
        public string PhoneCode { get; set; }
        public string Cellphone { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string Username { get; set; }
        public string MerchantName { get; set; }
        public AccountStatus Status { get; set; }       

        public long? POSId { get; set; }
        public string Email { get; set; }
        public bool IsVerifiedEmail { get; set; }
        public int CountryId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Photo { get; set; }
        public string PIN { get; set; }
        public string SecretKey { get; set; }
        public bool IsAllowWithdrawal { get; set; }
        public bool IsAllowAcceptPayment { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }

        public decimal Receivables_Tier { get; set; }

        public decimal Markup { get; set; }
        public string AuthSecretKey { get; set; }
        public byte ValidationFlag { get; set; }
        public VerifyStatus L1VerifyStatus { get; set; }
        public VerifyStatus L2VerifyStatus { get; set; }
        //public string MinerCryptoAddress { get; set; }

        public WithdrawalFeeType WithdrawalFeeType { get; set; }
        public string Language { get; set; }
        public string InvitationCode { get; set; }
    }

    public enum WithdrawalFeeType
    {
        /// <summary>
        /// 手续费默认的币种
        /// </summary>
        AutoCryptoCoin = 0,
        /// <summary>
        /// 手续费币种为飞币支付
        /// </summary>
        FiiiCoin = 1
    }
}
