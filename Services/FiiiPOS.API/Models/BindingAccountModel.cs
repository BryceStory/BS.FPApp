using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class BindingAccountModel
    {
        /// <summary>
        /// pos机sn码
        /// </summary>
        [Required]
        public string PosSN { get; set; }

        /// <summary>
        /// 商户账号
        /// </summary>
        [Required]
        public string MerchantAccount { get; set; }
    }
}