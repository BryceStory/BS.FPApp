using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class SendVerifyEmailModel
    {
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required(AllowEmptyStrings = false), EmailRegex]
        [StringLength(50)]
        public string EmailAddress { get; set; }
        
    }
}