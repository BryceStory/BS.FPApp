using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class ModifyMerchantNameModel
    {

        /// <summary>
        /// 商家名称
        /// </summary>
        [Required, StringLength(36)]
        public string MerchantName { get; set; }
    }
}