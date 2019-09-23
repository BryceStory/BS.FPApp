using System;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    /// <summary>
    /// the merchant account DTO
    /// </summary>
    public class MerchantAccountDTO
    {
        /// <summary>
        /// The merchant Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The merchant full cellphone
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// The merchant account username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The merchant name
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// The merchant account status
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// The merchant email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The merchant country id
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// The merchant is allow withdrawal
        /// </summary>
        public bool IsAllowWithdrawal { get; set; }
        /// <summary>
        /// The merchant is allow accept payment
        /// </summary>
        public bool IsAllowAcceptPayment { get; set; }
        /// <summary>
        /// The merchant fiat currency
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// The merchant receivables tier
        /// </summary>
        public decimal Receivables_Tier { get; set; }

        /// <summary>
        /// The merchant markup
        /// </summary>
        public decimal Markup { get; set; }
        /// <summary>
        /// The merchant validation flag
        /// </summary>
        public byte ValidationFlag { get; set; }

        /// <summary>
        /// The merchant profile Lv1 verify status
        /// </summary>
        public VerifyStatus Lv1VerifyStatus { get; set; }
        ///// <summary>
        ///// The miner crypto currency address
        ///// </summary>
        //public string MinerCryptoAddress { get; set; }

        /// <summary>
        /// The account is mining enabled
        /// </summary>
        public bool IsMiningEnabled { get; set; }
        /// <summary>
        /// 手续费是否使用飞币支付
        /// </summary>
        public WithdrawalFeeType Type { get; set; }

        /// <summary>
        /// POS广播号
        /// </summary>
        /// <value>
        /// The broadcast no.
        /// </value>
        public string BroadcastNo { get; set; }
    }
}