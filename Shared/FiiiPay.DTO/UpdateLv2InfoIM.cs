using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Profile
{
    public class UpdateLv2InfoIM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 1)]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 1)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(32, MinimumLength = 1)]
        public string State { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 1)]
        public string Postcode { get; set; }
        [Required, RequiredGuid]
        public Guid ResidentImage { get; set; }
    }
}
