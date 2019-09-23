using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class CryptoAddress
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public AccountType AccountType { get; set; }
        public int CryptoId { get; set; }
        public string Alias { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
    }
}
