using System.Collections;
using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.JPush.Model
{
    /// <summary>
    /// 自定义消息。
    /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_v3_push/#message"/>
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息内容本身（必填）。
        /// </summary>
        [JsonProperty("msg_content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the extras.
        /// </summary>
        /// <value>
        /// The extras.
        /// </value>
        [JsonProperty("extras")]
        public IDictionary Extras { get; set; }
    }
}
