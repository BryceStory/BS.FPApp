using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class InvestorOrder
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; }
        public InvestorTransactionType TransactionType { get; set; }
        public byte Status { get; set; }
        public int InverstorAccountId { get; set; }
        public int CryptoId { get; set; }
        public Guid UserAccountId { get; set; }
        public decimal CryptoAmount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}