using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Investor
{
    /// <summary>
    /// Statement Entity
    /// </summary>
    public class StatementDTO
    {
        /// <summary>
        /// Statement Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Statement 类型
        /// </summary>
        public InvestorTransactionType TransactionType { get; set; }
        /// <summary>
        /// Statement 金额
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 交易时间戳
        /// </summary>
        public long Timestamp { get; set; }
    }
}