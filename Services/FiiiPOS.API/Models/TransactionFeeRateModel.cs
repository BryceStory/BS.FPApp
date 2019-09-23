using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPOS.API.Models
{
    public class TransactionFeeRateModel
    {
        /// <summary>
        /// 提币的币种Id
        /// </summary>
        [Required, MathRange(Ranges.MinCoinId, Ranges.MaxCoinId)]
        public int CoinId { get; set; }

        /// <summary>
        /// 提币到的地址
        /// </summary>
        [Required]
        public string TargetAddress { get; set; }

        /// <summary>
        /// tag
        /// </summary>
        public string TargetTag { get; set; }
    }
}
