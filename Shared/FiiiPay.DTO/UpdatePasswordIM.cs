using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Security
{
    public class UpdatePasswordIM
    {
        [Required(AllowEmptyStrings = false), MaxLength(256)]
        public string OldPassword { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(256)]
        public string Password { get; set; }
    }
}
