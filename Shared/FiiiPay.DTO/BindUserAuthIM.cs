using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Authenticator
{
    /// <summary>
    /// 用户绑定谷歌验证器
    /// </summary>
    public class BindUserAuthIM
    {
        /// <summary>
        /// 密钥
        /// </summary>
        [Required(AllowEmptyStrings = false),MaxLength(50)]
        public string SecretKey { get; set; }
    }
}
