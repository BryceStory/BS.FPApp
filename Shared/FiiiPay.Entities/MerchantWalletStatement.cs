using System;

namespace FiiiPay.Entities
{
    public class MerchantWalletStatement
    {
        public long Id { get; set; }
        public long WalletId { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }

    public class MerchantWalletStatementAction
    {
        public const string Receipt = "Receipt";
        public const string Deposit = "Deposit";
        public const string Withdrawal = "Withdrawal";
    }
}
