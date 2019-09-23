using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities
{
    public class UserTransaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public int CryptoId { get; set; }
        public string CryptoCode { get; set; }
        public UserTransactionType Type { get; set; }
        public string DetailId { get; set; }
        public byte Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public string OrderNo { get; set; }
        public string MerchantName { get; set; }
    }

    /// <summary>
    /// 单据类型
    /// </summary>
    public enum UserTransactionType
    {
        Deposit = 0,
        Withdrawal,
        /// <summary>
        /// 消费
        /// </summary>
        Order,
        /// <summary>
        /// 退款
        /// </summary>
        Refund,
        /// <summary>
        /// 转账
        /// </summary>
        TransferOut,
        TransferIn,
        /// <summary>
        /// 划转
        /// </summary>
        ExTransferIn,
        ExTransferOut,
        /// <summary>
        /// 奖励
        /// </summary>
        Profit,
        /// <summary>
        /// fiiishop商城订单
        /// </summary>
        GatewayOrder,
        RefundGatewayOrder,
        /// <summary>
        /// 生活缴费
        /// </summary>
        BillOrder,
        /// <summary>
        /// 门店消费
        /// </summary>
        StoreOrderConsume,
        /// <summary>
        /// 门店收入
        /// </summary>
        StoreOrderIncome,
        /// <summary>
        /// 发红包
        /// </summary>
        PushRedPocket,
        /// <summary>
        /// 领红包
        /// </summary>
        ReceiveRedPocket
    }
}
