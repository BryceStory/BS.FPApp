using FiiiPay.Foundation.Entities.Enum;
using System;

namespace FiiiPOS.DTO
{
    public class MerchantWalletDTO
    {
        /// <summary>
        /// 钱包ID
        /// </summary>
        public long WalletId { get; set; }
        /// <summary>
        /// 币种ID
        /// </summary>
        public int CryptoId { get; set; }
        public CryptoStatus CryptoStatus { get; set; }
        /// <summary>
        /// 币种简称
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string CryptoName { get; set; }
        /// <summary>
        /// 币种icon地址
        /// </summary>
        public Guid? IconURL { get; set; }
        /// <summary>
        /// 可用余额
        /// </summary>
        public string Balance { get; set; }
        /// <summary>
        /// 冻结余额
        /// </summary>
        public string FrozenBalance { get; set; }

        /// <summary>
        /// DecimalPlace
        /// </summary>
        public int DecimalPlace { get; set; }

        /// <summary>
        /// 法币兑换率
        /// </summary>
        public string FiatExchangeRate { get; set; }
        /// <summary>
        /// 折合法币金额
        /// </summary>
        public string FiatBalance { get; set; }
        /// <summary>
        /// 法币英文简写
        /// </summary>
        public string FiatCode { get; set; }
        /// <summary>
        /// 货币的状态 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}