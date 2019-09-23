using System;
using FiiiPay.Framework;

namespace FiiiPay.DTO.Invite
{
    public class InviteRecordIM
    {
        public string InvitationCode { get; set; }
        /// <summary>
        /// 被邀请人的主键
        /// </summary>
        public Guid BeInvitedAccountId { get; set; }

        public int Type { get; set; }
    }
}
