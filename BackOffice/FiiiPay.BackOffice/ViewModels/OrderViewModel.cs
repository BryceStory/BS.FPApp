using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    public class OrderViewModel
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
        /// 商家名称
        /// </summary>
        public string MerchantName { set; get; }        

        /// <summary>
        /// POS机序列号 ? MerchantAccountId
        /// </summary>
        public string PostSN { set; get; }

        /// <summary>
        /// 客户手机号 ?  UserAccountId
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
        /// 溢价
        /// </summary>
        public decimal Markup { set; get; }

        /// <summary>
        /// 订单金额（FiatAmount + Markup）
        /// </summary>
        public decimal ActualFiatAmount { get; set; }

        /// <summary>
        /// 交易币种 ?  CryptoId
        /// </summary>
        public int CryptoId { set; get; }
        /// <summary>
        /// 交易币种 ?  CryptoName
        /// </summary>
        public string CryptoName { set; get; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { set; get; }

        /// <summary>
        /// 加密币金额[交易数量]
        /// </summary>
        public decimal CryptoAmount { set; get; }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal TransactionFee { set; get; }

        /// <summary>
        /// 商家实际收款（CryptoAmount - TransactionFee）
        /// </summary>
        public decimal ActualCryptoAmount { set; get; }


        public OrderStatus Status { set; get; }


        public DateTime Timestamp { set; get; }

    }
}