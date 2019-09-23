using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Account
{
    public class LoginIM
    {
        /// <summary>
        /// CountryId
        /// </summary>
        [Required, Plus]
        public int CountryId { get; set; }
        /// <summary>
        /// Cellphone
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
