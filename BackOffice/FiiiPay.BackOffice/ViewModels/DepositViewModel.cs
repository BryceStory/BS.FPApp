using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.BackOffice.ViewModels
{
    public class DepositViewModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Username { get; set; }
        public bool SelfPlatform { get; set; }
        public int CryptoId { get; set; }
        public int CountryId { get; set; }
        public long RequestId { get; set; }
        public string CryptoName { get; set; }
        public decimal Amount { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }
        public string TXID { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}