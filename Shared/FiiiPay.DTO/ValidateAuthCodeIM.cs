using FiiiPay.Framework.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Account
{
    /// <summary>
    /// validate google authenticator
    /// </summary>
    public class ValidateAuthCodeIM
    {
        /// <summary>
        /// CountryId
        /// </summary>
        [Required, Plus]
        public int CountryId { get; set; }
        /// <summary>
        /// Cellphone
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Password { get; set; }
        /// <summary>
        /// google authenticator code
        /// </summary>
        [StringLength(10)]
        public string GoogleCode { get; set; }

    }

    public class NewDeviceLoginIM
    {
        /// <summary>
        /// CountryId
        /// </summary>
        [Required, Plus]
        public int CountryId { get; set; }
        /// <summary>
        /// Cellphone
        /// </summary>
        [Required(AllowEmptyStrings = false), CellphoneRegex]
        public string Cellphone { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Password { get; set; }
        /// <summary>
        /// google authenticator code
        /// </summary>
        [StringLength(10)]
        public string GoogleCode { get; set; }
        /// <summary>
        /// pin码
        /// </summary>
        [StringLength(256)]
        public string Pin { get; set; }

        /// <summary>
        /// IdentityDocNo
        /// </summary>
        [StringLength(100)]
        public string IdentityDocNo { get; set; }
    }
}
