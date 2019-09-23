using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class VerifyOriginalEmailIM
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailRegex]
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string Code { get; set; }
    }
}