using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class InvestorWalletStatement
    {
        public Guid Id { get; set; }
        public int InvestorId { get; set; }
        public Guid TagAccountId { get; set; }
        public InvestorTransactionType Action { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}
