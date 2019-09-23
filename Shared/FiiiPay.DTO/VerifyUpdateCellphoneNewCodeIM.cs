using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Security
{
    public class VerifyUpdateCellphoneNewCodeIM
    {
        [Required(AllowEmptyStrings = false), MaxLength(50), CellphoneRegex]
        public string Cellphone { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }
    }
}
