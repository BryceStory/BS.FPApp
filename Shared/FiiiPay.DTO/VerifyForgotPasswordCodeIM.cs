using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Security
{
    public class VerifyForgotPasswordCodeIM
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required,CellphoneRegex]
        public string Cellphone { get; set; }
        [Required,MaxLength(10)]
        public string Code { get; set; }
    }
}
