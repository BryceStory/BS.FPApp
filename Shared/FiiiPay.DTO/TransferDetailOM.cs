using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Transfer
{
    /// <summary>
    /// 转账详情
    /// </summary>
    public class TransferDetailOM
    {
        /// <summary>
        /// 交易状态
        /// </summary>
        public TransactionStatus Status { get; set; }
        /// <summary>
        /// 交易状态文本
        /// </summary>
        public string StatusStr { get; set; }
        /// <summary>
        /// 交易类型，转账
        /// </summary>
        public string TradeType { get; set; }
        /// <summary>
        /// 转账类型，转入或转出
        /// </summary>
        public string TransferType { get; set; }
        /// <summary>
        /// 币种编码
        /// </summary>
        public string CoinCode { get; set; }
        /// <summary>
        /// 转账金额
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 目标账号名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 目标全名
        /// </summary>
        public string Fullname { get; set; }
        /// <summary>
        /// 转账时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }
    }
}
