using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class SettingCrytocurrencyModel
    {
        /// <summary>
        /// 收款的币种Id
        /// </summary>
        [Required]
        public List<int> CryptoIds { get; set; }
    }
}