using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 提币Model
    /// </summary>
    public class WithdrawalModel
    {
        /// <summary>
        /// 提币数量
        /// </summary>
        [Required, MathRange(typeof(decimal), Ranges.MinCoinAmount, Ranges.MaxCoinAmount)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 提币地址
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// 提币标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 加密币ID
        /// </summary>
        [Required, MathRange(Ranges.MinCoinId, Ranges.MaxCoinId)]
        public int CryptoId { get; set; }
    }
}