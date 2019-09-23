using System;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public class WalletEntity
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
    }
}