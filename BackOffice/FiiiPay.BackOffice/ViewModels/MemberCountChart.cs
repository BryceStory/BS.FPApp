using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.ViewModels
{
    /// <summary>
    /// 会员和POS机商家数量统计
    /// </summary>
    public class MemberCountChart
    {
        public string DateRange { get; set; }
        public int UserCount { get; set; }
        public int MerchantCount { get; set; }
    }
}