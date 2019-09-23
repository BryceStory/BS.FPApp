using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class MerchantTransferDTO
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
    }
}