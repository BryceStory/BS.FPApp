namespace FiiiPay.Entities.Enums
{
    public enum VerifyStatus
    {
        /// <summary>
        /// 未提交认证
        /// </summary>
        Uncertified = 0,
        /// <summary>
        ///  已认证
        /// </summary>
        Certified = 1,
        /// <summary>
        /// 驳回
        /// </summary>
        Disapproval = 2,
        /// <summary>
        /// 待核验
        /// </summary>
        UnderApproval = 3,
    }
}