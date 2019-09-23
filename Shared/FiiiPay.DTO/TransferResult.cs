using System;

namespace FiiiPay.DTO.Investor
{
    /// <summary>
    /// Transfer成功结果
    /// </summary>
    public class TransferResult
    {
        /// <summary>
        /// 交易Id
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 收款方
        /// </summary>
        public string TargetAccount { get; set; }
    }
}