using System.ComponentModel.DataAnnotations;
using FiiiPay.Framework.ValidationAttributes;

namespace FiiiPay.DTO.HomePage
{
    public class ToggleShowInHomePageIM
    {
        /// <summary>
        /// 加密货币的Id
        /// </summary>
        [Required,Plus]
        public int CoinId { get; set; }
    }
}
