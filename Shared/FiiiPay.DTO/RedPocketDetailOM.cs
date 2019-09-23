using System;

namespace FiiiPay.DTO
{
    /// <summary>
    /// Class FiiiPay.DTO.RedPocketDetailOM
    /// </summary>
    public class RedPocketDetailOM
    {
        /// <summary>
        /// 红包Id
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// 发送者昵称
        /// </summary>
        /// <value>
        /// The sneder nickname.
        /// </value>
        public string SnederNickname { get; set; }

        /// <summary>
        /// 祝福语
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// 自己领取币数量(为空表示该红包自己并没有领取)
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public string SelfAmount { get; set; }

        /// <summary>
        /// 红包总数量
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        /// <value>
        /// The crypto code.
        /// </value>
        public string CryptoCode { get; set; }

        /// <summary>
        /// 已经领取数量
        /// </summary>
        /// <value>
        /// The receive count.
        /// </value>
        public int ReceiveCount { get; set; }

        /// <summary>
        /// 红包总数量
        /// </summary>
        /// <value>
        /// The total amount.
        /// </value>
        public string TotalAmount { get; set; }

        /// <summary>
        /// 已经领取数量
        /// </summary>
        /// <value>
        /// The receive amount.
        /// </value>
        public string ReceiveAmount { get; set; }

        /// <summary>
        /// 口令
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
        /// 是否领取成功
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has receive; otherwise, <c>false</c>.
        /// </value>
        public ReceiveStatusEnum ReceiveStatus { get; set; }

        /// <summary>
        /// 是否自己发送
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has self sned; otherwise, <c>false</c>.
        /// </value>
        public bool HasSelfSned { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has expried; otherwise, <c>false</c>.
        /// </value>
        public bool HasExpried { get; set; }

        /// <summary>
        /// 法币金额
        /// </summary>
        /// <value>
        /// The fiat amount.
        /// </value>
        public string FiatAmount { get; set; }
    }

    public enum ReceiveStatusEnum : byte
    {
        /// <summary>
        /// 没有获取
        /// </summary>
        None = 0,
        /// <summary>
        /// 领取成功
        /// </summary>
        Receive,
        /// <summary>
        /// 已领取
        /// </summary>
        HasReceive
    }
}
