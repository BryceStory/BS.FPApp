namespace FiiiPay.DTO
{
    public class StatementDetailOM
    {
        /// <summary>
        /// 类型 14,收入15,支出
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public byte Type { get; set; }

        /// <summary>
        /// 红包金额
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public string Amount { get; set; }
        
        /// <summary>
        /// 是否退款
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has refund; otherwise, <c>false</c>.
        /// </value>
        public bool HasRefund { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        /// <value>
        /// The refund amount.
        /// </value>
        public string RefundAmount { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        /// <value>
        /// The refund timestamp.
        /// </value>
        public string RefundTimestamp { get; set; }

        /// <summary>
        /// 红包ID
        /// </summary>
        /// <value>
        /// The pocket identifier.
        /// </value>
        public long PocketId { get; set; }

        /// <summary>
        /// 状态  1,活动中2,已经完成3已经退款
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public byte Status { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        /// <value>
        /// The crypto code.
        /// </value>
        public string CryptoCode { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        /// <value>
        /// The order no.
        /// </value>
        public string OrderNo { get; set; }
    }
}