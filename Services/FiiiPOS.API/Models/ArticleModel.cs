using FiiiPay.Entities.EntitySet;

namespace FiiiPOS.API.Models
{
    public class ArticleModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Timestamp { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 消息的类型，根据这个来调用不同的详情页
        /// </summary>
        public SystemMessageESType Type { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool Read { get; set; }
    }
}
