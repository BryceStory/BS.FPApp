using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    /// <summary>
    /// All type
    /// </summary>
    public class ListAllTypeIM
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        public int? CoinId { get; set; }

        /// <summary>
        /// 需查询类型的最大值(用于兼容旧版本)
        /// </summary>
        public int MaxType { get; set; }

        public int? PageSize { get; set; } = 10;

        /// <summary>
        /// 从0开始
        /// </summary>
        public int? PageIndex { get; set; } = 0;

        /// <summary>
        /// 按月选择，比如：2018-02
        /// </summary>

        public string Mounth { get; set; }

        /// <summary>
        /// 按日选择起始日期，比如：2018-02-01
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 按日选择结束日期，比如：2018-02-01
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 搜索的关键字：订单号或者商家名
        /// </summary>
        public string Keyword { get; set; }
    }
}
