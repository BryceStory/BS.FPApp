using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Withdraw
{
    public class TransactionFeeRateIM
    {
        /// <summary>
        /// 提币的币种Id
        /// </summary>
        [Required,Plus]
        public int CoinId { get; set; }

        /// <summary>
        /// 提币到的地址
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string TargetAddress { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public string TargetTag { get; set; }
    }
}
