using System;

namespace FiiiPay.Entities.EntitySet
{
    public class SystemMessageES
    {
        public string Id { get; set; }
        public SystemMessageESType Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Read { get; set; }
        public string Intro { get; set; }

        /// <summary>
        /// 用于临时保存不同表的特殊的信息
        /// </summary>
        public string Attach { get; set; }
    }

    public enum SystemMessageESType
    {
        /// <summary>
        /// 公告
        /// </summary>
        Article = 0,

        /// <summary>
        /// 审核通知
        /// </summary>
        Verify
    }
}
