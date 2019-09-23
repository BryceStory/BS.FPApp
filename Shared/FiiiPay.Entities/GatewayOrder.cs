using System;

namespace FiiiPay.Entities
{
    public class GatewayOrder
    {
        public Guid Id { get; set; }
        public string TradeNo { get; set; }
        public string OrderNo { get; set; }
        public Guid MerchantAccountId { get; set; }
        public string MerchantName { get; set; }
        public Guid? UserAccountId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }

        /// <summary>
        /// 订单加密币金额
        /// </summary>
        public decimal CryptoAmount { get; set; }
        /// <summary>
        /// 商家实际收款（CryptoAmount - TransactionFee）
        /// </summary>
        public decimal ActualCryptoAmount { get; set; }
        /// <summary>
        /// 订单金额（FiatAmount + Markup）
        /// </summary>
        public decimal? ActualFiatAmount { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        public decimal? FiatAmount { get; set; }
        /// <summary>
        /// 法币类型
        /// </summary>
        public string FiatCurrency { get; set; }
        public GatewayOrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 加密币对法币汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// 溢价
        /// </summary>
        public decimal Markup { get; set; }
        /// <summary>
        /// 手续费（收取商家）
        /// </summary>
        public decimal TransactionFee { get; set; }
        /// <summary>
        /// 支付过期时间
        /// </summary>
        public DateTime ExpiredTime { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaymentTime { get; set; }
        public string Remark { get; set; }
    }

    public enum GatewayOrderStatus
    {
        Pending = 1,
        Completed,
        Refunded,
        Cancel
    }
}
