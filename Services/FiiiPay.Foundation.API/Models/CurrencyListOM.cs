using System.Collections.Generic;

namespace FiiiPay.Foundation.API.Models
{
    /// <summary>
    /// 法币列表返回类
    /// </summary>
    public class CurrencyListOM
    {
        /// <summary>
        /// 用户当前设置的法币
        /// </summary>
        public CurrencyItem DefaultCurrency { get; set; }

        /// <summary>
        /// 法币列表
        /// </summary>
        public List<CurrencyItem> CurrencyList { get; set; }
    }


    public class CurrencyItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
    }
}
