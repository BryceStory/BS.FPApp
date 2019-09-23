using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Common
{
    public class GridPager
    {
        /// <summary>
        /// 排序的列名
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 排序方式,asc或desc
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 每页显示的行数
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count { get; set; }
    }
}