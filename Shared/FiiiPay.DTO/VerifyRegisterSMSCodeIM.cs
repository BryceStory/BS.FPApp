using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.Constants;

namespace FiiiPay.DTO.Account
{
    public class VerifyRegisterSMSCodeIM
    {
        [Required, MathRange(Ranges.MinCountryId, Ranges.MaxCountryId)]
        public int CountryId { get; set; }

        [Required(AllowEmptyStrings = false),CellphoneRegex]
        [StringLength(50)]
        public string Cellphone { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string SMSCode { get; set; }
    }
}
