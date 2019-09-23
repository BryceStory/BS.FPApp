using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Models.Input
{
    public class UpdateMerchantNameModel
    {
        /// <summary>
        /// 商家名称
        /// </summary>
        [Required, StringLength(36)]
        public string MerchantName { get; set; }
    }
}