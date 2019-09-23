namespace FiiiPay.DTO.Invite
{
    public class PreFiiiPayProfitOM
    {
        /// <summary>
        /// 邀请人数
        /// </summary>
        public int InvitationCount { get; set; }
        /// <summary>
        /// fiiicoin总收益
        /// </summary>
        public decimal TotalProfitAmount { get; set; }
    }
}
