using System;

namespace FiiiPOS.Web.API.Models.Output
{

    /// <summary>
    /// 订单实体
    /// </summary>
    public class OrderRecordListOutModel
    {
        /// <summary>
		/// 订单Id
		/// </summary>
		public Guid Id { set; get; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// POS机序列号
        /// </summary>
        public string PostSN { set; get; }

        /// <summary>
        /// 客户手机号
        /// </summary>
        public string Cellphone { set; get; }

        /// <summary>
        /// 法币类型
        /// </summary>
        public string FiatCurrency { get; set; }

        /// <summary>
        /// 法定货币金额[消费金额、实收金额]
        /// </summary>
        public decimal FiatAmount { set; get; }

        /// <summary>
        /// 溢价率
        /// </summary>
        public decimal Markup { set; get; }

        /// <summary>
        /// 订单金额（FiatAmount + Markup）
        /// </summary>
        public decimal ActualFiatAmount { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string CryptoName { set; get; }

        /// <summary>
        /// 交易汇率
        /// </summary>
        public decimal ExchangeRate { set; get; }

        /// <summary>
        /// 加密币金额[交易数量]
        /// </summary>
        public decimal CryptoAmount { set; get; }

        /// <summary>
        /// 手续费
        /// </summary>
        public string TransactionFee { set; get; }

        /// <summary>
        /// 商家实际收款（CryptoAmount - TransactionFee）
        /// </summary>
        public decimal ActualCryptoAmount { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public int Status { set; get; }

        /// <summary>
        /// 时间
        /// </summary>
        public long Timestamp { set; get; }

        /// <summary>
        /// 当前汇率
        /// </summary>
        public string CurrentExchangeRate { set; get; }

        /// <summary>
        /// 汇率涨幅
        /// </summary>
        public string IncreaseRate { set; get; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string WithdrawalFee { set; get; }
        /// <summary>
        /// 手续费币种
        /// </summary>
        public string WithdrawalCryptoCode { set; get; }
    }
}