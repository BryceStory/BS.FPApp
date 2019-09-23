using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class StoreOrderPayIM
    {
        public decimal FiatAmount { get; set; }
        public int CoinId { get; set; }
        public Guid MerchantInfoId { get; set; }
        public string Pin { get; set; }
    }

    /// <summary>
    /// 门店收入详情
    /// </summary>
    public class StoreIncomeDetail
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 订单金额，门店实收数量
        /// </summary>
        public string CryptoActualAmount { get; set; }
        /// <summary>
        /// 币种编码
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 客户账号
        /// </summary>
        public string UserAccountName { get; set; }
        /// <summary>
        /// 消费时间戳
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 法币编码
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        public string FiatAmount { get; set; }
        /// <summary>
        /// 溢价率
        /// </summary>
        public string MarkUp { get; set; }
        /// <summary>
        /// 法币交易金额
        /// </summary>
        public string TotalFiatAmount { get; set; }
        /// <summary>
        /// 交易时费率
        /// </summary>
        public string ExchangeRate { get; set; }
        /// <summary>
        /// 当前费率
        /// </summary>
        public string CurrentExchangeRate { get; set; }
        /// <summary>
        /// 费率涨幅
        /// </summary>
        public string IncreaseRate { get; set; }
        /// <summary>
        /// 加密货币数量，用户支付数量
        /// </summary>
        public string CryptoAmount { get; set; }
        /// <summary>
        /// 交易手续费
        /// </summary>
        public string TransactionFee { get; set; }
        /// <summary>
        /// 交易手续费币种
        /// </summary>
        public string FeeCryptoCode { get; set; }
    }

    /// <summary>
    /// 门店消费详情
    /// </summary>
    public class StoreConsumeDetail
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 订单金额，实际支付数量
        /// </summary>
        public string CryptoActualAmount { get; set; }
        /// <summary>
        /// 币种编码
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 法币编码
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 法币金额
        /// </summary>
        public string FiatAmount { get; set; }
        /// <summary>
        /// 溢价率
        /// </summary>
        public string MarkUp { get; set; }
        /// <summary>
        /// 交易时费率
        /// </summary>
        public string ExchangeRate { get; set; }
        /// <summary>
        /// 当前费率
        /// </summary>
        public string CurrentExchangeRate { get; set; }
        /// <summary>
        /// 费率涨幅
        /// </summary>
        public string IncreaseRate { get; set; }
        /// <summary>
        /// 消费时间
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        public string OrderNo { get; set; }
    }
}
