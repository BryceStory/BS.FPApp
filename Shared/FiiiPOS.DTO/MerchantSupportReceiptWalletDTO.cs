using FiiiPay.Foundation.Entities.Enum;
using System;

namespace FiiiPOS.DTO
{
    public class MerchantSupportReceiptWalletDTO
    {
        public long WalletId { get; set; }
        public int CryptoId { get; set; }
        public CryptoStatus CryptoStatus { get; set; }
        public string CryptoCode { get; set; }
        public string CryptoName { get; set; }
        public Guid? IconURL { get; set; }
        public string MarketPrice { get; set; }
        public int DecimalPlace { get; set; }
        public decimal Markup { get; set; }
        public decimal Balance { get; set; }
        /// <summary>
        /// 是否默认选择 0否 1是
        /// </summary>
        public int IsDefault { get; set; }
        /// <summary>
        /// 货币的状态 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}