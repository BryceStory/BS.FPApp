using System;

namespace FiiiPay.Entities
{
    public class UserExTransferOrder
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string OrderNo { get; set; }
        public ExTransferType OrderType { get; set; }
        public Guid AccountId { get; set; }
        public long WalletId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
        public decimal Amount { get; set; }
        public byte Status { get; set; }
        public string Remark { get; set; }
        public string ExId { get; set; }
    }


    public enum ExTransferType
    {
        FromEx = 1,
        ToEx
    }
}