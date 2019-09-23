using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class FeedbackModel
    {
        /// <summary>
        /// 反馈内容
        /// </summary>
        [Required, StringLength(2000)]
        public string Content { get; set; }
    }
}