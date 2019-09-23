using System;

namespace FiiiPay.Entities.EntitySet
{
    public class UserWalletStatementES
    {
        public string OrderId { get; set; }
        public int CryptoId { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 红包退款状态
        /// </summary>
        public int RefundStatus { get; set; }
        public DateTime Timestamp { get; set; }


        /// <summary>
        /// 加密货币金额（如果是提币，表示实际到账金额）
        /// </summary>
        public decimal CryptoAmount { get; set; }
        
        /// <summary>
        /// 0：充币，1：提币，2：消费，3：退款，4：转帐，客户端根据这个调用对应的接口查询详情
        /// </summary>
        public int Type { get; set; }
    }
}
