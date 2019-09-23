namespace FiiiPOS.Web.API.Models.Input
{
    /// <summary>
    /// 收款记录输入参数
    /// </summary>
    public class OrderRecordListInModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 交易状态 0= 1= 2=
        /// </summary>
        public int States { set; get; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartDate { set; get; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { set; get; }

        /// <summary>
        /// 当前页面
        /// </summary>
        public int PageIndex { set; get; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { set; get; }

    }
}