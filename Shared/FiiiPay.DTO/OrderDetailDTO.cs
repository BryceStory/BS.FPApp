using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Investor
{
    /// <summary>
    /// 订单详情Entity
    /// </summary>
    public class OrderDetailDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public InvestorTransactionType TransactionType { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 币种Code
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 对方账户
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// 对方名称
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 交易时间戳
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
    }
}