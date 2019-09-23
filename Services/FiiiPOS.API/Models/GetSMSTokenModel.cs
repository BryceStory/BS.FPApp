using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class GetSMSTokenModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
    }
}