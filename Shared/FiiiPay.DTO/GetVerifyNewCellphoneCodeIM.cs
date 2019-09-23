using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Security
{
    public class GetVerifyNewCellphoneCodeIM
    {
        /// <summary>
        /// 不要带上地区码
        /// </summary>
        [Required(AllowEmptyStrings = false), MaxLength(50), CellphoneRegex]
        public string NewCellphone { get; set; }
    }
}
