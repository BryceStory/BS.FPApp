using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SignonModel
    {
        /// <summary>
        /// POS机SN码
        /// </summary>
        [Required, StringLength(20)]
        public string POSSN { get; set; }

        /// <summary>
        /// 商户注册时的帐号
        /// </summary>
        [Required]
        public string MerchantId { get; set; }
        /// <summary>
        /// PIN码
        /// </summary>
        [Required]
        public string PIN { get; set; }
    }
}