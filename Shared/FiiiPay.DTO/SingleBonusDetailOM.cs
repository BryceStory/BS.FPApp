namespace FiiiPay.DTO.Invite
{
    /// <summary>
    /// 单个的交易详情
    /// </summary>
    public class SingleBonusDetailOM
    {
        /// <summary>
        /// 类型
        /// </summary>
        public BonusType Type { get; set; }
        /// <summary>
        /// 类型字符串
        /// </summary>
        public string TypeStr { get;set; }
        /// <summary>
        /// 交易说明
        /// </summary>
        public string TradeDescription { get; set; }
        /// <summary>
        /// 来自什么
        /// </summary>
        public string BonusFrom { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public Entities.InviteStatusType Status { get; set; }
        /// <summary>
        /// 交易状态字符串
        /// </summary>
        public string StatusStr { get;set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 奖励金额
        /// </summary>
        public string Amount { get; set; }
        
        /// <summary>
        /// 奖励类型
        /// </summary>
        public enum BonusType
        {
            /// <summary>
            /// 奖励收入
            /// </summary>
            BonusIncome,
        }

    }

    

    
}
