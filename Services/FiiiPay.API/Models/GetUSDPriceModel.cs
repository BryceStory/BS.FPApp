using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class GetUSDPriceModel
    {
        /// <summary>
        /// 币种
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }
    }
}