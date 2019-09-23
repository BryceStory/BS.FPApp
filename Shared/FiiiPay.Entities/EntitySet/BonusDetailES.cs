using System;

namespace FiiiPay.Entities.EntitySet
{
    public class BonusDetailES
    {
        public decimal CryptoAmount { get; set; }

        public Guid AccountId { get; set; }

        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName { get; set; }

        public Guid MerchantId { get; set; }
    }
}
