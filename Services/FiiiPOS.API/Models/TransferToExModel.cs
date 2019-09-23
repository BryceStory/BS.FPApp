using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 划转到 Ex Entity
    /// </summary>
    public class TransferToExModel
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        [Required, MathRange(Ranges.MinCoinId, Ranges.MaxCoinId)]
        public int CryptoId { get; set; }
        /// <summary>
        /// 划转数量
        /// </summary>
        [Required, MathRange(typeof(decimal), Ranges.MinCoinAmount, Ranges.MaxCoinAmount)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 校验PIN获得的token
        /// </summary>
        [Required]
        public string PINToken { get; set; }
    }

    /// <summary>
    /// 从 Ex 划转 Entity
    /// </summary>
    public class TransferFromExModel
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        [Required, MathRange(Ranges.MinCoinId, Ranges.MaxCoinId)]
        public int CryptoId { get; set; }
        /// <summary>
        /// 划转数量
        /// </summary>
        [Required, MathRange(typeof(decimal), Ranges.MinCoinAmount, Ranges.MaxCoinAmount)]
        public decimal Amount { get; set; }

        /// <summary>
        /// 校验PIN获得的token
        /// </summary>
        [Required]
        public string PINToken { get; set; }
    }
}