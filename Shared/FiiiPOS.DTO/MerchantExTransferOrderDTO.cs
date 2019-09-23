using FiiiPay.Entities;

namespace FiiiPOS.DTO
{
    public class MerchantExTransferOrderDTO
    {
        public long Id { get; set; }
        public ExTransferType ExTransferType { get; set; }
        public string Amount { get; set; }
        public string CryptoCode { get; set; }
        public byte Status { get; set; }
        public long Timestamp { get; set; }
        public string OrderNo { get; set; }
    }
}