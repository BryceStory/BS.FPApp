using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Security
{
    public class ResetPasswordByForgotPasswordCodeIM
    {
        public int? CountryId { get; set; }
        [Required,CellphoneRegex]
        public string Cellphone { get; set; }
        [Required]
        public string Code { get; set; }
        [Required, MaxLength(256)]
        public string Password { get; set; }
    }
}
