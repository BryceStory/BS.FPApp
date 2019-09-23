namespace FiiiPay.Entities.EntitySet
{
    public class OrderDayStat
    {
        /// <summary>
        /// 订单日期
        /// </summary>
        public string OrderDay { set; get; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { set; get; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { set; get; }
    }

    public class OrderMonthStat
    {
        /// <summary>
        /// 订单年份
        /// </summary>
        public int OrderYear { set; get; }

        /// <summary>
        /// 订单月份
        /// </summary>
        public int OrderMonth { set; get; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { set; get; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { set; get; }

    }

    /// <summary>
    /// 统计
    /// </summary>
    public class OrderStat
    {
        /// <summary>
        /// 总交易量
        /// </summary>
        public int TotalCount { set; get; }

        /// <summary>
        /// 今日交易量
        /// </summary>
        public int TodayCount { set; get; }

        /// <summary>
        /// 总交易金额
        /// </summary>
        public decimal TotalMoney { set; get; }

        /// <summary>
        /// 今日交易金额
        /// </summary>
        public decimal TodayMoney { set; get; }

    }
}
