using System.ComponentModel.DataAnnotations;

namespace FiiiPay.ShopPayment.API.Models
{
    /// <summary>
    /// Class SendLoginSMSCodeVo
    /// </summary>
    public class SendLoginSMSCodeVo
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
    }
}