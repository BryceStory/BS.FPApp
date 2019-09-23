using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Transfer
{
    /// <summary>
    /// 转账准备请求
    /// </summary>
    public class PreTransferIM
    {
        /// <summary>
        /// 币种ID
        /// </summary>
        [Required, Plus]
        public int CoinId { get; set; }
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
    }
}
