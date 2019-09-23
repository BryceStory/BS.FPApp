using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;
// ReSharper disable InconsistentNaming

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 注册model
    /// </summary>
    public class SignupModel : VerifyAccountModel
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required, Plus]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        [StringLength(50)]
        public string Cellphone { get; set; }
        /// <summary>
        /// POS机SN码
        /// </summary>
        [Required]
        public string POSSN { get; set; }

        /// <summary>
        /// PIN码
        /// </summary>
        [Required]
        public string PIN { get; set; }
    }
}