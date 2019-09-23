namespace FiiiPay.DTO.Invite
{
    public class PreFiiiPosProfitOM
    {
        /// <summary>
        /// 收益利率
        /// </summary>
        public decimal ProfitRate { get; set; }
        /// <summary>
        /// 已经获得fiiicoin数量
        /// </summary>
        public string CryptoAmount { get; set; }
        /// <summary>
        /// 邀请购买fiiipos数量
        /// </summary>
        public int InviteCount { get; set; }
    }
}
