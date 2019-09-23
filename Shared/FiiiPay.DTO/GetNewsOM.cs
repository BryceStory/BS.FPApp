using System;

namespace FiiiPay.DTO.Messages
{
    public class GetNewsOM
    {
        public string Title { get; set; }
        public long Timestamp { get; set; }
        public string Content { get; set; }
    }

    public class VerifyMessageOM
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息推送时间
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public Guid MerchantInfoId { get; set; }
    }
}
