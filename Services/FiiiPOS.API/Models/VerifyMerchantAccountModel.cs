using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class VerifyMerchantAccountModel
    {
        /// <summary>
        /// 商户账号
        /// </summary>
        [Required, StringLength(100)]
        public string MerchantAccount { get; set; }

        /// <summary>
        /// 国家ID
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// 商户手机号,不包含国家代码
        /// </summary>
        [Required, StringLength(50)]
        public string Cellphone { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required,StringLength(10)]
        public string Code { get; set; }
    }
}