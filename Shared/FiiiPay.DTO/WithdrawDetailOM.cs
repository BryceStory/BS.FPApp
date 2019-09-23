using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Withdraw
{
    public class WithdrawDetailOM
    {
        public long Id { get; set; }

        /// <summary>
        /// 加密货币币种名称：比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否需要Tag
        /// </summary>
        public bool NeedTag { get; set; }

        public string Timestamp { get; set; }

        /// <summary>
        /// 法币金额
        /// </summary>
        public string FiatAmount { get; set; }

        /// <summary>
        /// 法币：比如：MRY
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 加密货币金额
        /// </summary>
        public string CryptoAmount { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// 比如：已完成
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 交易Id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public string TransactionFee { get; set; }

        public string Address { get; set; }

        public string Tag { get; set; }

        /// <summary>
        /// 确认次数
        /// </summary>
        public string CheckTime { get; set; }

        /// <summary>
        /// 是否自己平台内充币
        /// </summary>
        public bool SelfPlatform { get; set; }

        /// <summary>
        /// 提币失败的Remark
        /// </summary>
        public string Remark { get; set; }
    }
}
