using System.ComponentModel.DataAnnotations;
using FiiiPay.Entities;

namespace FiiiPOS.API.Models
{
    public class ReadModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public ReadRecordType Type { get; set; }
    }
}