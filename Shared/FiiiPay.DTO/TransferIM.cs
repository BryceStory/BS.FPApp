using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Transfer
{
    /// <summary>
    /// 转账请求
    /// </summary>
    public class TransferIM
    {
        /// <summary>
        /// 国家ID
        /// </summary>
        [Required, Plus]
        public int ToCountryId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required, CellphoneRegex]
        public string ToCellphone { get; set; }
        /// <summary>
        /// 转账加密币币种Id
        /// </summary>
        [Required, Plus]
        public int CoinId { get; set; }
        /// <summary>
        /// 转账金额
        /// </summary>
        [Required, Plus]
        public decimal Amount { get; set; }
        /// <summary>
        /// Pin
        /// </summary>
        [Required, MaxLength(256)]
        public string Pin { get; set; }
    }
}
