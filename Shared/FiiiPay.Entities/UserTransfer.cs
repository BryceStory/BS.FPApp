using System;

namespace FiiiPay.Entities
{
    public class UserTransfer
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string OrderNo { get; set; }
        public Guid FromUserAccountId { get; set; }
        public long FromUserWalletId { get; set; }
        public int CoinId { get; set; }
        public string CoinCode { get; set; }
        public Guid ToUserAccountId { get; set; }
        public long ToUserWalletId { get; set; }
        public decimal Amount { get; set; }
        public byte Status { get; set; }
        public string Remark { get; set; }
    }
}
