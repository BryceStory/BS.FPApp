using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateCellphoneIM
    {
        /// <summary>
        /// 新手机，不要带上地区码
        /// </summary>
        [Required,MaxLength(50),CellphoneRegex]
        public string NewCellphone { get; set; }

    }
}
