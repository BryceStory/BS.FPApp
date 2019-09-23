using FiiiPay.Framework.Enums;

namespace FiiiPay.FiiiCoinWork.API.Models
{
    /// <summary>
    /// 查询账单
    /// </summary>
    public class QueryStatementModel
    {
        /// <summary>
        /// 查询开始时间戳
        /// </summary>
        public long? StartTimestamp { get; set; }
        /// <summary>
        /// 查询结束时间戳
        /// </summary>
        public long? EndTimestamp { get; set; }
        /// <summary>
        /// 用户手机号（精确搜索）
        /// </summary>
        public string Cellphone { get; set; }
        /// <summary>
        /// 类型（不传获取所有）
        /// </summary>
        public InvestorTransactionType? TransactionType { get; set; }

        /// <summary>
        /// 页码（默认: 0）
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 每页返回多少（默认: 10）
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}