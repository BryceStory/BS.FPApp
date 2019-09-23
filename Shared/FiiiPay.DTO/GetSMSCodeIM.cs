using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;

namespace FiiiPay.DTO.Account
{
    public class GetSMSCodeIM
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required(AllowEmptyStrings = false),CellphoneRegex]
        [StringLength(50)]
        public string Cellphone { get; set; }
    }
}
