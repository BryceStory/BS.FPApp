using System.Collections.Generic;

namespace FiiiPOS.Web.API.Models.Output
{
    /// <summary>
    /// 带总记录数、带分页
    /// </summary>
    public class DataListWithPageResultModel<T>
    {
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 数据记录
        /// </summary>
        public List<T> DataList { get; set; }
    }
}