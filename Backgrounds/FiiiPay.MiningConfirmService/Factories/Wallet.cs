using System;

namespace FiiiPay.MiningConfirmService.Factories
{
    public class Wallet
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public int CryptoId { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
    }
}