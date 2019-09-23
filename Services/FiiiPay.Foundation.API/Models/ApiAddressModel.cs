using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Foundation.API.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiAddressModel
    {
        /// <summary>
        /// 0：ios，1：android
        /// </summary>
        [Required]
        public byte Platform { get; set; }
    }
}