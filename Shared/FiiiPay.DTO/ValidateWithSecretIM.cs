using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Authenticator
{
    /// <summary>
    /// ValidateWithSecret
    /// </summary>
    public class ValidateWithSecretIM
    {
        /// <summary>
        /// 密钥
        /// </summary>
        [Required]
        public string SecretKey { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required]
        public string Code { get; set; }
    }
    public class ValidateGoogleAuthIM
    {
        /// <summary>
        /// 谷歌验证码
        /// </summary>
        [Required]
        public string GoogleCode { get; set; }
    }
}
