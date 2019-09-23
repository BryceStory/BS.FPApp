using System;

namespace FiiiPay.Entities
{
    public class InviteRecord
    {
        public int Id { get; set; }
        public string SN { get; set; }
        /// <summary>
        /// 被邀请人id
        /// </summary>
        public Guid AccountId { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviterCode { get; set; }

        public InviteType Type { get; set; }

        /// <summary>
        /// 邀请人id
        /// </summary>
        public Guid InviterAccountId { get; set; }

        public DateTime Timestamp { get; set; }
        
    }
    
    public enum InviteType
    {
        Fiiipay,
        Fiiipos,
        /// <summary>
        /// Fiiipay门店
        /// </summary>
        FiiipayMerchant,
        /// <summary>
        /// Fiiipos门店
        /// </summary>
        FiiiposMerchant
    }
}
