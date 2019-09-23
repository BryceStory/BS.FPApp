using System;

namespace FiiiPay.Entities
{
    public class MerchantWallet
    {
        public long Id { get; set; }
        public Guid MerchantAccountId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
        public MerchantWalletStatus Status { get; set; }
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }

        /// <summary>
        /// 支持收款
        /// </summary>
        public bool SupportReceipt { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; }
    }

    public enum MerchantWalletStatus
    {
        /// <summary>
        /// 启用
        /// </summary>
        Displayed=1,
        /// <summary>
        /// 禁用
        /// </summary>
        Concealed=2
    }
}
