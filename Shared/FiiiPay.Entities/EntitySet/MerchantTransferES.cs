using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities.EntitySet
{
    public class MerchantTransferES
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 充币/提币
        /// </summary>
        public TransactionType TransferType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Timestamp { get; set; }

        public int CryptoId { get; set; }
    }
}