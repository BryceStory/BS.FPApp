using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Foundation.Entities.Enum
{
    [Flags]
    public enum CountryStatus
    {
        //该国家是否可用
        Enable = 1,
        //该国家是否支持生活缴费
        BillerEnable = 1 << 1,
    }
}
