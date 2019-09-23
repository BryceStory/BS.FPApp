using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Account
{
    /// <summary>
    /// ValidateLoginBySMSCode
    /// </summary>
    public class ValidateLoginBySMSCodeIM
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
        /// google authencator code
        /// </summary>
        [StringLength(10)]
        public string GoogleCode { get; set; }
    }
    public class NewDeviceLoginBySMSCodeIM
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
        /// google authencator code
        /// </summary>
        [StringLength(10)]
        public string GoogleCode { get; set; }
        /// <summary>
        /// pin码
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// IdentityDocNo
        /// </summary>
        public string IdentityDocNo { get; set; }
    }

}
