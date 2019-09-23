namespace FiiiPOS.API.Models
{
    public class WithdrawalResult
    {
        /// <summary>
        /// 提现ID，根据这个ID获取详情
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 提交时间戳
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 提现订单号
        /// </summary>
        public string OrderNo { get; set; }
    }
}