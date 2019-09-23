using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.Profile.API.Models
{
    public class DateIM
    {
        [RequiredGuid]
        public Guid Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
