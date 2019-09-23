using FiiiPay.Entities;

namespace FiiiPay.DTO.FiiiExTransfer
{
    public class UserExTransferOrderOM
    {
        public long Id { get; set; }
        public ExTransferType ExTransferType { get; set; }
        public string ExTransferTypeStr { get; set; }
        public string Amount { get; set; }
        public string CryptoCode { get; set; }
        public byte Status { get; set; }
        public string StatusStr { get; set; }
        public long Timestamp { get; set; }
        public string OrderNo { get; set; }
    }
}