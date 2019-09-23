using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public class Deposit
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public long WalletId { get; set; }
        public string CryptoCode { get; set; }
        /// <summary>
        /// 充币来源
        /// </summary>
        public DepositFromType FromType { get; set; }
        public string FromAddress { get; set; }
        public string FromTag { get; set; }
        public string ToAddress { get; set; }
        public string ToTag { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string OrderNo { get; set; }
        public string TransactionId { get; set; }
        public long? RequestId { get; set; }
    }
}