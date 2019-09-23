using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Account
{
    /// <summary>
    /// LoginBySMSCode
    /// </summary>
    public class LoginBySMSCodeIM
    {
        /// <summary>
        /// 国家Id
        /// </summary>
        [Required]
        public int CountryId { get; set; }

        /// <summary>
        /// 手机号，不包含地区码
        /// </summary>
        [Required]
        public string Cellphone { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string SMSCode { get; set; }
    }
}
