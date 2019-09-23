using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ModifyEmailModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }

        /// <summary>
        /// 验证PIN时产生的Token，首次不填
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// 原邮箱验证码，首次不填
        /// </summary>
        public string OriginalCode { get; set; }

    }


}