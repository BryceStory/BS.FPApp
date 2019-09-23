using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// Class FiiiPOS.API.Models.OrderCreateModel
    /// </summary>
    public class OrderCreateModel
    {
        /// <summary>
        /// 币种ID
        /// </summary>
        [Required, MathRange(Ranges.MinCoinId, Ranges.MaxCoinId)]
        public int CryptoId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Required, MathRange(typeof(decimal), Ranges.MinFiatAmount, Ranges.MaxFiatAmount)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 法币编码
        /// </summary>
        [MaxLength(4)]
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        [Required]
        public PaymentType PaymentType { get; set; }

        ///// <summary>
        ///// 客户Id
        ///// </summary>
        //public Guid? UserAccountId { get; set; }

        /// <summary>
        /// 用户token
        /// </summary>
        public string UserToken { get; set; }
    }
}