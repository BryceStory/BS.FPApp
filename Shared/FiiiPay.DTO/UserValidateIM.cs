using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Authenticator
{
    /// <summary>
    /// 用户验证谷歌验证器
    /// </summary>
    public class UserValidateIM
    {
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(AllowEmptyStrings = false),MaxLength(10)]
        public string Code { get; set; }
    }
}
