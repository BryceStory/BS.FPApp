using System;

namespace FiiiPay.DTO.Invite
{
    public class FiiiposBonusRecordOM
    {
        public string MerchantName { get; set; }

        public string TotalCryptoAmount { get; set; }
        /// <summary>
        /// 作为下次获取商家分红详情的入参
        /// </summary>
        public Guid MerchantId { get; set; }
           
    }
}
