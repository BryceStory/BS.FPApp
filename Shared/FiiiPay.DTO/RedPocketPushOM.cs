namespace FiiiPay.DTO
{
    /// <summary>
    /// Class FiiiPay.DTO.RedPocketPushOM
    /// </summary>
    public class RedPocketPushOM
    {
        /// <summary>
        /// 红包口令
        /// </summary>
        /// <value>
        /// The pass code.
        /// </value>
        public string PassCode { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// 祝福语
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}
