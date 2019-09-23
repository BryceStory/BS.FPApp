using System;
using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class RedisOrderDTO
    {
        public Guid MerchantGuid { get; set; }

        public string OrderNo { get; set; }

        public decimal FiatAmount { get; set; }

        public string FiatCurrency { get; set; }

        public int CryptoId { get; set; }
        
        public Guid UserId { get; set; }

        public int CountryId { get; set; }

        public decimal Markup { get; set; }

        public string ClientIP { get; set; }

        public PaymentType Type { get; set; }
    }

}
