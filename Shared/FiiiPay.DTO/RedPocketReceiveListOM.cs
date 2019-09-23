using System.Collections.Generic;

namespace FiiiPay.DTO
{
    public class RedPocketReceiveListOM
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
        public List<ReceiveRedPockDetail> DetailList { get; set; }
    }

    public class ReceiveRedPockDetail
    {
        /// <summary>
        /// 红包ID
        /// </summary>
        /// <value>
        /// The red pocket identifier.
        /// </value>
        public long RedPocketId { get; set; }

        /// <summary>
        /// 领取者昵称
        /// </summary>
        /// <value>
        /// The sender nickname.
        /// </value>
        public string Nickname { get; set; }

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
        /// 时间
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; set; }
    }
}
