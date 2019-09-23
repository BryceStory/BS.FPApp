namespace FiiiPay.DTO.Statement
{
    public class ListIM
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        public int? CoinId { get; set; }

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

        /// <summary>
        /// 0：充币，1：提币，2：消费，3：退款，4：转入，5：转出，6：划入，7：划出， 8：奖励 不传表示查询全部
        /// </summary>
        public int? Type { get; set; }
    }
}
