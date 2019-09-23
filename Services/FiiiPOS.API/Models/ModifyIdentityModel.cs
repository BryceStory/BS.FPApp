using FiiiPay.Framework.Enums;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.API.Models
{
    public class ModifyIdentityModel
    {
        /// <summary>
        /// 证件类型
        /// </summary>
        [Required]
        public IdentityDocType IdentityDocType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string IdentityDocNo { get; set; }
    }
}