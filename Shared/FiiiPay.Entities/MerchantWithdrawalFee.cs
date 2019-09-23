using System;

namespace FiiiPay.Entities
{
    public class MerchantWithdrawalFee
    {
        public long Id { get; set; }
        public long WithdrawalId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
    }
}
