using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPay.API.Models
{
    public class RedPocketDetailListModel
    {
        /// <summary>
        /// 红包ID
        /// </summary>
        /// <value>
        /// The pocket identifier.
        /// </value>
        public long PocketId { get; set; }

            /// <summary>
        /// 页码
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        [Required]
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        [Required]
        public int PageSize { get; set; }
    }
}