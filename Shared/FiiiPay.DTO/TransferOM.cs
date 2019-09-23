namespace FiiiPay.DTO.Transfer
{
    /// <summary>
    /// 转账返回
    /// </summary>
    public class TransferOM
    {
        /// <summary>
        /// 转账时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 交易Id
        /// </summary>
        public long TracingId { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string TracingNo { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        public string AccountName { get; set; }
    }
}
