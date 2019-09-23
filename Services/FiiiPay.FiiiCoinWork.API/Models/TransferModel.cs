using System.ComponentModel.DataAnnotations;

namespace FiiiPay.FiiiCoinWork.API.Models
{
    public class TransferModel
    {
        /// <summary>
        /// PIN验证通过后获得的Token
        /// </summary>
        [Required]
        public string PINToken { get; set; }
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required]
        public int CountryId { get; set; }
        /// <summary>
        /// 手机号 例: 13312345678
        /// </summary>
        [Required]
        public string Cellphone { get; set; }

        /// <summary>
        /// 转账数量
        /// </summary>
        [Required, Range(0, 999999999)]
        public decimal Amount { get; set; }
    }
}