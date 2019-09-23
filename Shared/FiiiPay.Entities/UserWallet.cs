using System;

namespace FiiiPay.Entities
{
    public class UserWallet
    {
        public long Id { get; set; }
        public Guid UserAccountId { get; set; }
        public int CryptoId { get; set; }

        public string CryptoCode { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal Balance { get; set; }
        public decimal FrozenBalance { get; set; }
        public string Address { get; set; }
        public string Tag { get; set; }

        /// <summary>
        /// 是否展示在首页
        /// </summary>
        public bool ShowInHomePage { get; set; }

        /// <summary>
        /// 首页展示顺序（值小的排在前面）
        /// </summary>
        public int HomePageRank { get; set; }

        /// <summary>
        /// 支付顺序（冲币、提币、以及添加提币地址都使用这个排序）（值小的排在前面）
        /// </summary>
        public int PayRank { get; set; }
    }
}
