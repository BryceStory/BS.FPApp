using System;
using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.Account
{
    public class DateIM
    {
        [RequiredGuid]
        public Guid Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
