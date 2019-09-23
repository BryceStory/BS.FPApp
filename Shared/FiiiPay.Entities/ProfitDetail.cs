using System;

namespace FiiiPay.Entities
{
    public class ProfitDetail
    {
        public int Id { get; set; }

        public int InvitationId { get; set; }

        public decimal CryptoAmount { get; set; }

        public Guid AccountId { get; set; }

        public ProfitType Type { get; set; }

        public DateTime Timestamp { get; set; }

        public int CryptoId { get; set; }

        public InviteStatusType Status { get; set; }

        public string OrderNo { get; set; }

        public string CryptoCode { get; set; }
    }

    [Flags]
    public enum ProfitType
    {
        /// <summary>
        /// 被fiiipay用户邀请
        /// </summary>
        BeInvited = 0x0001,
        /// <summary>
        /// 邀请fiiipay用户
        /// </summary>
        InvitePiiiPay = 0x0010,
        /// <summary>
        /// 邀请购买fiiipos机(POS机挖矿奖励邀请人)
        /// </summary>
        InvitePiiiPos = 0x0100,
        /// <summary>
        /// 邀请满五十人额外奖励
        /// </summary>
        Reward = 0x1000
    }
    public enum InviteStatusType
    {
        IssuedFrozen = 1,
        IssuedActive
    }
}
