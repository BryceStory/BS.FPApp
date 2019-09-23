using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.API.Models
{
    public class SettingMarkupModel
    {
        /// <summary>
        /// 溢价率百分数
        /// </summary>
        [RegularExpression("^(0|100|[0-9]{1,2})(.[0-9]{0,2})?$", ErrorMessage = "输入值必须0到100以内，且最多2位小数。")]
        public decimal PerMarkup { get; set; }
    }
}