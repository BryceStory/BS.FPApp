using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities.EntitySet.Investor
{
    public class StatementES
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
        public decimal Amount { get; set; }
        /// <summary>
        /// 交易时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}