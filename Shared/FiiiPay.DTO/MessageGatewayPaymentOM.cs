using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class MessageGatewayPaymentOM
    {
        /// <summary>
        /// 交易状态0：已完成，1：已退款
        /// </summary>
        public int TradeStatus { get; set; }
        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal PaymentFiatAmount { get; set; }
        /// <summary>
        /// 法币币种
        /// </summary>
        public string FiatCode { get; set; }

        /// <summary>
        /// 溢价率
        /// </summary>
        public decimal Markup { get; set; }
        /// <summary>
        /// 支付币种
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 交易汇率
        /// </summary>
        public decimal TradeExchangeRate { get; set; }
        /// <summary>
        /// 当前汇率
        /// </summary>
        public decimal CurrentExchangeRate { get; set; }
        /// <summary>
        /// 汇率涨幅
        /// </summary>
        public decimal IncreaseRate { get; set; }
        /// <summary>
        /// 实付数量
        /// </summary>
        public decimal PaymentCryptoAmount { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string TradeTime { get; set; }
        /// <summary>
        /// 退款时间
        /// </summary>
        public string RefundTime { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        public string MerchantOrderNo { get; set; }

    }
}
