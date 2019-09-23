// ReSharper disable InconsistentNaming

using FiiiPay.Entities.Enums;

namespace FiiiPay.DTO.Withdraw
{
    /// <summary>
    /// 
    /// </summary>
    public class TransactionFeeRateOM
    {
        /// <summary>
        /// 手续费率
        /// </summary>
        public string TransactionFeeRate { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public string TransactionFee { get; set; }

        /// <summary>
        /// 地址类型
        /// </summary>
        public CryptoAddressType CryptoAddressType { get; set; }
    }

    
}
