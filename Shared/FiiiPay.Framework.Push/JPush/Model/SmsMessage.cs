using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.JPush.Model
{
    /// <summary>
    /// 短信补充。
    /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_v3_push/#sms_message"/>
    /// </summary>
    public class SmsMessage
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the delay time.
        /// </summary>
        /// <value>
        /// The delay time.
        /// </value>
        [JsonProperty("delay_time", DefaultValueHandling = DefaultValueHandling.Include)]
        public int DelayTime { get; set; }
    }
}
