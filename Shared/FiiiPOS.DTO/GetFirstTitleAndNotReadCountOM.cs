using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPOS.DTO
{
    public class GetFirstTitleAndNotReadCountOM
    {
        /// <summary>
        /// 第一条的Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 第一条的时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 未读的系统消息条数
        /// </summary>
        public int SysCount { get; set; }

        /// <summary>
        /// 未读的消息条数（不包含系统消息）
        /// </summary>
        public long Count { get; set; }
    }
}
