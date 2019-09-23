using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    public class FileDeleteModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}