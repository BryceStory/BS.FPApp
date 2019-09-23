using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Profile.API.Form
{
    public class FiiiPayId
    {
        [Required]
        public Guid Id { get; set; }
    }
}