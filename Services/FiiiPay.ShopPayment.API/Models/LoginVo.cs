using System.ComponentModel.DataAnnotations;

namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class LoginVo
    /// </summary>
    public class LoginVo
    {
        /// <summary>
        /// CountryPhoneCode
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string CountryPhoneCode { get; set; }

        /// <summary>
        /// Gets or sets the cell phone.
        /// </summary>
        /// <value>
        /// The cell phone.
        /// </value>
        [Required(AllowEmptyStrings = false)]
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required(AllowEmptyStrings = false)]
        public string SmsCode { get; set; }
    }
}