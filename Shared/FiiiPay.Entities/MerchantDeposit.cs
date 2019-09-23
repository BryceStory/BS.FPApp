using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class MerchantDeposit
    {
        public long Id { get; set; }
        public Guid MerchantAccountId { get; set; }
        public long MerchantWalletId { get; set; }
        public DepositFromType FromType { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
        public string OrderNo { get; set; }
        public string TransactionId { get; set; }
        public bool SelfPlatform { get; set; }
        public string CryptoCode { get; set; }
        public long? RequestId { get; set; }
    }
}
