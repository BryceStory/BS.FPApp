using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// 发送注册手机验证码
    /// </summary>
    public class GetRegisterSMSCodeModel
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
    /// 验证注册手机验证码
    /// </summary>
    public class ChekRegisterSMSCodeModel
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

        /// <summary>
        /// 手机验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string SMSCode { get; set; }
    }

    /// <summary>
    /// 注册
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// 国家ID
        /// </summary>
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }

        /// <summary>
        /// 密码明文
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [PasscodeRegex]
        public string Password { get; set; }

        /// <summary>
        /// 手机验证码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string SMSCode { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        [StringLength(10)]
        public string InviterCode { get; set; }
    }
}