using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Models
{
    [SugarTable("Accounts")]
    public class GatewayAccount
    {
        public Guid Id { get; set; }

        public string PhoneCode { get; set; }

        public string Cellphone { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string MerchantName { get; set; }

        public decimal Balance { get; set; }

        public string Password { get; set; }

        public string PIN { get; set; }

        public GayewayAccountStatus Status { get; set; }

        public string CallbackUrl { get; set; }

        public bool IsVerifiedEmail { get; set; }

        public int CountryId { get; set; }

        public string Currency { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Guid Photo { get; set; }

        public string SecretKey { get; set; }

        public bool IsAllowPayment { get; set; }

        public string AuthSecretKey { get; set; }
    }

    /// <summary>
    /// 账户状态
    /// </summary>
    public enum GayewayAccountStatus
    {
        /// <summary>
        /// 已禁用
        /// </summary>
        Disabled,
        /// <summary>
        /// 已注册，未绑定邮箱与设置密码
        /// </summary>
        Registered,
        /// <summary>
        /// 正常状态
        /// </summary>
        Active
    }

}