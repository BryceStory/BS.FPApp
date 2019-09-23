using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    /// <summary>
    /// 设置商家法币实体
    /// </summary>
    public class SettingFiatCurrencyModel
    {
        /// <summary>
        /// 法币
        /// </summary>
        [Required, StringLength(10)]
        public string FiatCurrency { get; set; }
    }
}