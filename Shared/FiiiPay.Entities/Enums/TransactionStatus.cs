namespace FiiiPay.Entities.Enums
{
    public enum TransactionStatus
    {
        /// <summary>
        /// 未提交远程
        /// </summary>
        UnSubmit=0,
        /// <summary>
        /// 确认
        /// </summary>
        Confirmed = 1,
        /// <summary>
        /// 正在处理
        /// </summary>
        Pending,
        /// <summary>
        /// 取消
        /// </summary>
        Cancelled
    }
}