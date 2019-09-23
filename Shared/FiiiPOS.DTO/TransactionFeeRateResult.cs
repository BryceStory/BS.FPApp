using FiiiPay.Entities.Enums;

namespace FiiiPOS.DTO
{
    public class TransactionFeeDTO
    {
        /// <summary>
        /// 手续费率
        /// </summary>
        public string TransactionFeeRate { get; set; }

        /// <summary>
        /// 基础手续费
        /// </summary>
        public string TransactionFee { get; set; }

        /// <summary>
        /// 地址类型
        /// </summary>
        public CryptoAddressType CryptoAddressType { get; set; }
    }
}
