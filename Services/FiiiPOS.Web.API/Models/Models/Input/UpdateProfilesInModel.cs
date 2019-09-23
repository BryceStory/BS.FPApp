using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 修改商家信息
    /// </summary>
    public class UpdateProfilesInModel
    {
        /// <summary>
        /// 商家名称
        /// </summary>
        [Required, StringLength(36)]
        public string MerchantName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailRegex]
        public string Email { get; set; }


        /// <summary>
        /// 所在城市
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 1)]
        public int CityId { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 1)]
        public string Postcode { get; set; }

        /// <summary>
        /// 地址1
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 1)]
        public string Address1 { get; set; }        
    }
}