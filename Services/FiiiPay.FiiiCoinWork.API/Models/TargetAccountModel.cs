using System.ComponentModel.DataAnnotations;

namespace FiiiPay.FiiiCoinWork.API.Models
{
    /// <summary>
    /// TargetAccount Entity
    /// </summary>
    public class TargetAccountModel
    {
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
    }
}