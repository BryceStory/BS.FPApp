using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class UserWithdrawal
    {
        public long Id { get; set; }
        public Guid UserAccountId { get; set; }
        public long UserWalletId { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
        public string OrderNo { get; set; }
        public string TransactionId { get; set; }
        public bool SelfPlatform { get; set; }
        public long? RequestId { get; set; }

        public int CryptoId { get; set; }

        public string CryptoCode { get; set; }
    }
}
