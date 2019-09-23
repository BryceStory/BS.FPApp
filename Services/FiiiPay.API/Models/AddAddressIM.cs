using System.ComponentModel.DataAnnotations;

namespace FiiiPay.API.Models
{
    /// <summary>
    /// Class FiiiPay.API.Models.AddAddressIM
    /// </summary>
    public class AddAddressIM
    {
        [Required]
        public string Address { get; set; }

        public string Tag { get; set; }

        public int CoinId { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [Required]
        public string Alias { get; set; }
    }
}