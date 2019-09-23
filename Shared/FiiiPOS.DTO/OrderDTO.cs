using System;
using FiiiPay.Entities;

namespace FiiiPOS.DTO
{
    public class OrderDTO
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
        /// 订单状态 已完成 Completed=2,已退款 Refunded=3
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 加密币Code
        /// </summary>
        public string CryptoCode { get; set; }
        /// <summary>
        /// 加密币金额（未扣除手续费）
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
        /// 客户帐号
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// 实际加密币数量（扣除手续费）
        /// </summary>
        public string ActualCryptoAmount { get; set; }
    }
}