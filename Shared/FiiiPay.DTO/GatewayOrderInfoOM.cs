using System;

namespace FiiiPay.DTO.GatewayOrder
{
    public class GatewayOrderInfoOM
    {
        public Guid OrderId { get; set; }
        public string MerchantName { get; set; }
        public string CryptoCode { get; set; }
        public decimal ActurlCryptoAmount { get; set; }

        public string Timestamp { get; set; }
    }
}
