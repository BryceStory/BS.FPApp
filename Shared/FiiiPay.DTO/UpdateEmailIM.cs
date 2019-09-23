using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Profile
{
    public class UpdateEmailIM
    {
        /// <summary>
        /// 新的邮箱
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailRegex]
        [StringLength(50)]
        public string Email { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OriginalEmailIM
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required,EmailRegex]
        public string Email { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VerifyOriginalEmailIM
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required,EmailRegex]
        public string Email { get; set; }

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }
    }

}
