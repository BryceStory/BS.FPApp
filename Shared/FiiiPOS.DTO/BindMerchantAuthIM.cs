using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.DTO
{
    /// <summary>
    /// 商家绑定谷歌验证器
    /// </summary>
    public class BindMerchantAuthIM
    {
        /// <summary>
        /// 密钥
        /// </summary>
        [Required]
        public string SecretKey { get; set; }
    }
}
