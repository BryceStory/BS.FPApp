using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// 重置密码
    /// </summary>
    public class ResetPasswordModel
    {
        /// <summary>
        /// 国家ID
        /// </summary>
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }
        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }

        /// <summary>
        /// 密码明文
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [PasscodeRegex]
        public string Password { get; set; }
    }
    /// <summary>
    /// 发送重置密码手机验证码
    /// </summary>
    public class SendResetPasswordSMSCodeModel
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }
    }
    /// <summary>
    /// 验证重置密码手机验证码
    /// </summary>
    public class CheckResetPasswordSMSCodeModel
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required, CellphoneRegex]
        public string Cellphone { get; set; }
        /// <summary>
        /// 手机验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string Code { get; set; }
    }
}