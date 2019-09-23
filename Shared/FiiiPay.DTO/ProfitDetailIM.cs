using FiiiPay.Entities;
using System;

namespace FiiiPay.DTO.Invite
{
    public class ProfitDetailIM
    {
        /// <summary>
        /// 邀请记录表的唯一主键
        /// </summary>
        public int InvitationId { get; set; }

        public decimal CryptoAmount { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid AccountId { get; set; }

        public ProfitType Type { get; set; }

        public byte Status { get; set; }

        public string OrderNo { get; set; }

        public DateTime Timestamp { get; set; }

        public int CryptoId { get; set; }
    }
}
