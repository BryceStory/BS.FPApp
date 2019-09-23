using System;
using FiiiPay.Entities;
using FiiiPay.Foundation.Entities.Enum;

namespace FiiiPOS.DTO
{
    public class OrderDetailDTO
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 状态 已完成 Completed=2,已退款 Refunded=3
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 创单时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 退款时间戳
        /// </summary>
        public long? RefundTimestamp { get; set; }
        public CryptoStatus CryptoStatus { get; set; }
        /// <summary>
        /// 加密币Code
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 加密币金额
        /// </summary>
        public string CryptoAmount { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        public string FiatAmount { get; set; }
        /// <summary>
        /// 溢价
        /// </summary>
        public decimal Markup { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public string ActualFiatAmount { get; set; }
        /// <summary>
        /// 加密币对法币汇率
        /// </summary>
        public string ExchangeRate { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string TransactionFee { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public string ActualCryptoAmount { get; set; }
        /// <summary>
        /// 客户帐号
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// Pos SN
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 当前加密币对法币汇率
        /// </summary>
        public string CurrentExchangeRate { get; set; }

        /// <summary>
        /// 涨幅
        /// </summary>
        public string IncreaseRate { get; set; }
        /// <summary>
        /// 手续费 币种名称
        /// </summary>
        public string FeeCryptoCode { get; set; }
        /// <summary>
        /// 货币的状态 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}