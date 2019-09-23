using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    public class FeedbackIM
    {
        [Required]
        public string Content { get; set; }
    }
}