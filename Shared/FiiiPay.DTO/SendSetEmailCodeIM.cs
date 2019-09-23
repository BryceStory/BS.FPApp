using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Profile
{
    public class SendSetEmailCodeIM
    {
        [Required(AllowEmptyStrings = false), EmailRegex]
        [StringLength(50)]
        public string Email { get; set; }
    }
}
