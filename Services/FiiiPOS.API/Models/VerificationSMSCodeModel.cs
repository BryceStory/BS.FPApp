using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class VerificationSMSCodeModel
    {
        /// <summary>
        /// 国家ID
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// 手机号，不含国家代码
        /// </summary>
        [Required, StringLength(50)]
        public string Cellphone { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
    }
}