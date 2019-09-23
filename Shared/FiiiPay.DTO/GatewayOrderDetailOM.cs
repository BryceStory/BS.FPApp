using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class GatewayOrderDetailOM
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 加密货币币种名称：比如：BTC
        /// </summary>
        public string Code { get; set; }

        public string Timestamp { get; set; }

        /// <summary>
        /// 法币金额
        /// </summary>
        public string FiatAmount { get; set; }

        /// <summary>
        /// 法币：比如：MRY
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 加密货币金额
        /// </summary>
        public string CryptoAmount { get; set; }

        public string MerchantName { get; set; }

        /// <summary>
        /// 比如：已完成
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 溢价率，比如：10.00%
        /// </summary>
        public string MarkUp { get; set; }

        /// <summary>
        /// 当时的汇率，比如：1BTC = 1670.80MRY
        /// </summary>
        public string ExchangeRate { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public string RefundTimestamp { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 交易单号，底部的条形码也使用这个
        /// </summary>
        public string OrderNo { get; set; }

        public string TradeNo { get; set; }

        /// <summary>
        /// 当前加密币对法币汇率
        /// </summary>
        public string CurrentExchangeRate { get; set; }

        /// <summary>
        /// 涨幅
        /// </summary>
        public string IncreaseRate { get; set; }
    }
}
