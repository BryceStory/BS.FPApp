using System;

namespace FiiiPay.DTO.GatewayOrder
{
    /// <summary>
    /// 三方的交易订单信息
    /// </summary>
    public class GatewayOrderOM
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 商户id
        /// </summary>
        public Guid MerchantAccountId { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public Guid ProjectId { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>Class1.cs
        public Guid? UserAccountId { get; set; }

        public string Title { get; set; }

        public byte Status { get; set; }

        public string FiatCurrency { get; set; }

        public decimal? FiatAmount { get; set; }

        public decimal? ActualFiatAmount { get; set; }

        public string Crypto { get; set; }

        public decimal CryptoAmount { get; set; }

        public DateTime? PaymentTime { get; set; }

        public DateTime ExpiredTime { get; set; }

        public decimal TransactionFee { get; set; }

        public decimal MarkupRate { get; set; }

        public decimal MarkupAmount { get; set; }

        public decimal ActualCryptoAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        public bool NotifyStatus { get; set; }

        public string NotifyMethod { get; set; }

        public string NotifyUrl { get; set; }

        public DateTime Timestamp { get; set; }

        public string Remark { get; set; }
    }
}
