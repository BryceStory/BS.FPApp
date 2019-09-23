using System.Collections.Generic;

namespace FiiiPOS.DTO
{
    public class TradingReportDTO
    {
        /// <summary>
        /// 统计开始时间戳
        /// </summary>
        public long FormDate { get; set; }
        /// <summary>
        /// 统计结束时间戳
        /// </summary>
        public long ToDate { get; set; }

        /// <summary>
        /// 交易量
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public string SumAmount { get; set; }
        /// <summary>
        /// 均价
        /// </summary>
        public string AvgAmount { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 明细
        /// </summary>
        public List<Stat> Stats { get; set; }
    }

    public class Stat
    {
        /// <summary>
        /// 日期
        /// </summary>
        public long Date { get; set; }
        /// <summary>
        /// 交易量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 交易额
        /// </summary>
        public decimal Amount { get; set; }
    }
}