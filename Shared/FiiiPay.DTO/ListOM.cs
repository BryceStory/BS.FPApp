using System;
using System.Collections.Generic;

namespace FiiiPay.DTO.Statement
{
    public class ListOM
    {
        public List<ListOMItem> List { get; set; }
        public int CurrentPageIndex { get; set; }
    }

    public class ListOMItem
    {
        public string OrderId { get; set; }
        public Guid? IconUrl { get; set; }

        /// <summary>
        /// 加密货币币种名称：比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 状态字符串，比如：已完成
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        /// 状态，比如：1
        /// Order 1:Pending, 2:Completed, 3:Refunded
        /// Deposit 以及 WithDraw 1:Confirmed, 2:Pendin, 3:Cancelled
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 红包退款状态 0无退款 1部分退款 2全额退款
        /// </summary>
        public int RefundStatus { get; set; }
        /// <summary>
        /// 红包退款状态名称
        /// </summary>
        public string RefundStatusStr { get; set; }
        public string Timestamp { get; set; }

        /// <summary>
        /// 法币金额
        /// </summary>
        //public string FiatAmount { get; set; }

        /// <summary>
        /// 法币：比如：MRY
        /// </summary>
        //public string FiatCurrency { get; set; }

        /// <summary>
        /// 加密货币金额
        /// </summary>
        public string CryptoAmount { get; set; }

        /// <summary>
        /// 0：充币，1：提币，2：消费，3：退款，4：转账转出，5：转账转入 客户端根据这个调用对应的接口查询详情，消费和退款都调用Order/Detail接口查询详情
        /// </summary>
        public int Type { get; set; }
    }
}
