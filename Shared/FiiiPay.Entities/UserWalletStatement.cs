using System;

namespace FiiiPay.Entities
{
    public class UserWalletStatement
    {
        public long Id { get; set; }
        public long WalletId { get; set; }
        public string Action { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenAmount { get; set; }
        public decimal FrozenBalance { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }

    public class UserWalletStatementAction
    {
        public const string Consume = "Consume";
        public const string Deposit = "Deposit";
        public const string AwardDeposit = "AwardDeposit";
        public const string Withdrawal = "Withdrawal";
        public const string TansferIn = "TansferIn";
        public const string TansferOut = "TansferOut";
        public const string Invite = "Invite";
        public const string BeInvite = "BeInvite";
        public const string Reward = "Reward";
        public const string Refund = "Refund";
        public const string StoreOrderIn = "StoreOrderIn";
        public const string StoreOrderOut = "StoreOrderOut";
    }
}
