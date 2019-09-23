using System.Collections.Generic;

namespace FiiiPay.DTO.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageListOM
    {
        /// <summary>
        /// 
        /// </summary>
        public List<MessageListOMItem> List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageListOMItem
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public string NoticeId { get; set; }

        /// <summary>
        /// 消息类型[1=收款成功 3=退款成功 5=充币成功 6=提币成功 7=提币失败 
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 订单Id相关
        /// </summary>
        public string QueryId { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 0=未读 1=已读
        /// </summary>
        public int Status { get; set; }
    }
}
