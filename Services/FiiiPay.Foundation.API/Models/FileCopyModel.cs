using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    public class FileCopyModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}