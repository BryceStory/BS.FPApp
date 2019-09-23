using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckAccountModel
    {
        /// <summary>
        /// 商家账号
        /// </summary>
        [Required]
        public string MerchantId { get; set; }
        /// <summary>
        /// Pos机sn码
        /// </summary>
        [Required, StringLength(20)]
        public string SN { get; set; }
    }
}