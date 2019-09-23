using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Foundation.Entities.Enum
{
    [Flags]
    public enum CryptoStatus
    {
        //支付功能
        Pay = 1,
        //提币功能
        Withdrawal = 2,
        //充币功能
        Deposit = 4,
        //转账功能
        Transfer = 8,
        //生活缴费功能
        Biller = 16,
        //红包
        RedPocket=32
    }
}
