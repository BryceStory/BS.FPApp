using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class StatementDetailDataModel
    {
        /// <summary>
        /// Id
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public int Type { get; set; }
    }
}