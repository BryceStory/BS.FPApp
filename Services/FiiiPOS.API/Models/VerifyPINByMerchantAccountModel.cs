using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class VerifyPINByMerchantAccountModel
    {
        /// <summary>
        /// 商户账号
        /// </summary>
        [Required]
        public string MerchantAccount { get; set; }
        /// <summary>
        /// 加密后商户PIN
        /// </summary>
        [Required]
        public string EncryptPin { get; set; }
    }

    public class VerifyGoogleAuthByMerchantAccountModel
    {
        /// <summary>
        /// 商户账号
        /// </summary>
        [Required]
        public string MerchantAccount { get; set; }
        /// <summary>
        /// GoogleCode
        /// </summary>
        [Required]
        public string GoogleCode { get; set; }
    }
}