using System.Collections.Generic;

namespace FiiiPay.DTO
{
    public class RedPocketListDetailOM
    {
        /// <summary>
        /// 发送者昵称
        /// </summary>
        /// <value>
        /// The nickname.
        /// </value>
        public string Nickname { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public string Amount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        /// <value>
        /// The crypto code.
        /// </value>
        public string CryptoCode { get; set; }

        /// <summary>
        /// 是否手气最佳
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is best luck; otherwise, <c>false</c>.
        /// </value>
        public bool IsBestLuck { get; set; }
    }
}
