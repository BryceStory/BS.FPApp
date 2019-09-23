using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Security
{
    public class VerifyFindPinBackIdNoIM
    {
        /// <summary>
        /// 上一步拿到的短信验证码
        /// </summary>
        [Required(AllowEmptyStrings = false), MaxLength(10)]
        public string Code { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [Required(AllowEmptyStrings = false), MaxLength(50)]
        public string IdNo { get; set; }
    }
}
