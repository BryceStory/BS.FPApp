using System.Collections.Generic;

namespace FiiiPay.DTO
{
    /// <summary>
    /// Class FiiiPay.DTO.PushRedPocketDetailOM
    /// </summary>
    public class PushRedPocketListOM
    {
        /// <summary>
        /// 共发红包数
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public int Total { get; set; }

        /// <summary>
        /// 收到红包金额
        /// </summary>
        /// <value>
        /// The total fiat amount.
        /// </value>
        public string TotalFiatAmount { get; set; }

        /// <summary>
        /// 列表
        /// </summary>
        /// <value>
        /// The detail list.
        /// </value>
        public List<PushRedPockDetail> DetailList { get; set; }
    }

    public class PushRedPockDetail
    {
        /// <summary>
        /// 红包ID
        /// </summary>
        /// <value>
        /// The red pocket identifier.
        /// </value>
        public long RedPocketId { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        /// <value>
        /// The crypto code.
        /// </value>
        public string CryptoCode { get; set; }

        /// <summary>
        /// 数量 
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public string Amount { get; set; }

        /// <summary>
        /// 退回数量
        /// </summary>
        /// <value>
        /// The refund amount.
        /// </value>
        public string RefundAmount { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; set; }

        /// <summary>
        /// 状态(1,进行中2,已领完3,过期已退款)
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public byte Status { get; set; }
    }
}
