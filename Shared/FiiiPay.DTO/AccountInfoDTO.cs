namespace FiiiPay.DTO.Investor
{
    /// <summary>
    /// 帐号信息实体
    /// </summary>
    public class AccountInfoDTO
    {
        /// <summary>
        /// 帐号
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string InvestorName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Cellphone { get; set; }
        /// <summary>
        /// 是否重设过密码
        /// </summary>
        public bool IsResetPassword { get; set; }
        /// <summary>
        /// 是否重设过PIN码
        /// </summary>
        public bool IsResetPIN { get; set; }
    }
}