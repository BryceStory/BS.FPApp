using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    public class GetFileByMd5Model
    {
        [Required]
        public string Md5 { get; set; }
    }
}