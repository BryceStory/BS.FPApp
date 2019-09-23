using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.JPush.Model
{
    /// <summary>
    /// Class FiiiPay.Data.Agents.JPush.Model.PushPayload
    /// </summary>
    public class PushPayload
    {
        /// <summary>
        /// Gets or sets the c identifier.
        /// </summary>
        /// <value>
        /// The c identifier.
        /// </value>
        [JsonProperty("cid", NullValueHandling = NullValueHandling.Ignore)]
        public string CId { get; set; }

        /// <summary>
        /// 推送平台。可以为 "android" / "ios" / "all"。
        /// </summary>
        [JsonProperty("platform", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Platform { get; set; } = "all";

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>
        /// The audience.
        /// </value>
        [JsonProperty("audience", DefaultValueHandling = DefaultValueHandling.Include)]
        public object Audience { get; set; } = "all";

        /// <summary>
        /// Gets or sets the notification.
        /// </summary>
        /// <value>
        /// The notification.
        /// </value>
        [JsonProperty("notification", NullValueHandling = NullValueHandling.Ignore)]
        public Notification Notification { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public Message Message { get; set; }

        /// <summary>
        /// Gets or sets the SMS message.
        /// </summary>
        /// <value>
        /// The SMS message.
        /// </value>
        [JsonProperty("sms_message", NullValueHandling = NullValueHandling.Ignore)]
        public SmsMessage SMSMessage { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        [JsonProperty("options", DefaultValueHandling = DefaultValueHandling.Include)]
        public Options Options { get; set; } = new Options
        {
            IsApnsProduction = false
        };

        internal string GetJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return GetJson();
        }
    }
}
