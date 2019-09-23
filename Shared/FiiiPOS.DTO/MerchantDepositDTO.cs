using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class MerchantDepositDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 充币/提币
        /// </summary>
        public TransactionType TransactionType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public TransactionStatus TransactionStatus { get; set; }

        /// <summary>
        /// 币种Code
        /// </summary>
        public string CryptoCode { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        //public string Address { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        public string FaitAmount { get; set; }
        /// <summary>
        /// 法币币种
        /// </summary>
        public string FaitCurrency { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 是否自己平台内充币
        /// </summary>
        public bool SelfPlatform { get; set; }

        /// <summary>
        /// 确认次数
        /// </summary>
        public string CheckTime { get; set; }
    }
}