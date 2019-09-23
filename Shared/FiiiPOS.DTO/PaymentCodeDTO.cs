using System;

namespace FiiiPay.DTO.Order
{
    /// <summary>
    /// 用户支付码实体
    /// </summary>
    public class PaymentCodeDTO
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 支付码 （以00开头的17位纯数字字符串, eg：00628130633101607）
        /// </summary>
        public string PaymentCode { get; set; }

        /// <summary>
        /// 过期时间戳
        /// </summary>
        public string ExpireTimestamp { get; set; }
    }
}