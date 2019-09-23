using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Component.Enums
{
    /// <summary>
    /// 单据类型
    /// </summary>
    public enum BillType
    {
        Deposit=0,
        Withdrawal,
        Order,
        Refund,
        TransferOut,
        TransferIn,
        ExTransferIn,
        ExTransferOut,
        /// <summary>
        /// 奖励
        /// </summary>
        Profit,
        GatewayOrder,
        RefundGatewayOrder,
    }
}
