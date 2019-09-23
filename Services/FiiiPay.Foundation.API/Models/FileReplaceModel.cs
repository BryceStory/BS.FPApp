using System;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    public class FileReplaceModel
    {
        [Required]
        public Guid SourceId { get; set; }
        [Required]
        public Guid TargetId { get; set; }
    }
}