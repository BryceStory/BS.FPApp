using FiiiPay.Foundation.Entities.Enum;
using System;

namespace FiiiPay.Foundation.Entities
{
    public class Cryptocurrency
    {
        public int Id { get; set; }

        /// <summary>
        /// 比如：Bitcoin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 比如：BTC
        /// </summary>
        public string Code { get; set; }

        public CryptoStatus Status { get; set; }
        public byte DecimalPlace { get; set; }
        public Guid? IconURL { get; set; }
        public int Sequence { get; set; }

        /// <summary>
        /// 提现手续费率
        /// </summary>
        public decimal? Withdrawal_Tier { get; set; }

        /// <summary>
        /// 提现固定手续费
        /// </summary>
        public decimal? Withdrawal_Fee { get; set; }
        /// <summary>
        /// 是否需要tag
        /// </summary>
        public bool NeedTag { get; set; }
        /// <summary>
        /// 是否是固定价格
        /// </summary>
        public bool IsFixedPrice { get; set; }
        /// <summary>
        /// 是否可用0：不可用 1：可用
        /// </summary>
        public byte Enable { get; set; }
        /// <summary>
        /// 是否是白标客户币种0：否 1：是
        /// </summary>
        public byte IsWhiteLabel { get; set; }

    }
}
