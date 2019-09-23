using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingMinerCryptoAddressModel
    {
        /// <summary>
        /// 挖矿地址
        /// </summary>
        [Required]
        public string Address { get; set; }
    }
}