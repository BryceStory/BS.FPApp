using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class USDPriceModel
    {
        /// <summary>
        /// 红包最大数
        /// </summary>
        /// <value>
        /// The maximum count.
        /// </value>
        public int MaxCount { get; set; }

        /// <summary>
        /// 单个红包最大金额
        /// </summary>
        /// <value>
        /// The maximum amount.
        /// </value>
        public int MaxAmount { get; set; }

        /// <summary>
        /// USD
        /// </summary>
        /// <value>
        /// The usd price.
        /// </value>
        public string USDPrice { get; set; }
    }
}