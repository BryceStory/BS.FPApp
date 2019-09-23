using System;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; }
        public Guid MerchantAccountId { get; set; }
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
        public decimal ActualFiatAmount { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        public decimal FiatAmount { get; set; }
        /// <summary>
        /// 法币类型
        /// </summary>
        public string FiatCurrency { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Remark { get; set; }
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
        /// 商户IP
        /// </summary>
        public string MerchantIP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MerchantToken { get; set; }
        public string UserIP { get; set; }
        public string UserToken { get; set; }
        /// <summary>
        /// 支付类型(蓝牙，NFC，扫码)
        /// </summary>
        public PaymentType PaymentType { get; set; }
        /// <summary>
        /// 支付过期时间
        /// </summary>
        public DateTime ExpiredTime { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// 统一法币类型，用于统计计算
        /// </summary>
        public string UnifiedFiatCurrency { get; set; }
        public decimal UnifiedExchangeRate { get; set; }
        public decimal UnifiedActualFiatAmount { get; set; }
        public decimal UnifiedFiatAmount { get; set; }
        
    }


    public enum OrderStatus : byte
    {
        Pending=1,
        Completed,
        Refunded
    }
}
