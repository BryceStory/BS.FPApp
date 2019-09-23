using System.Collections.Generic;
using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.JPush.Model
{
    /// <summary>
    /// <see cref="https://docs.jiguang.cn/jpush/server/push/rest_api_v3_push/#notification"/>
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the alert.
        /// </summary>
        /// <value>
        /// The alert.
        /// </value>
        [JsonProperty("alert")]
        public string Alert { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        [JsonProperty("android", NullValueHandling = NullValueHandling.Ignore)]
        public Android Android { get; set; }

        /// <summary>
        /// Gets or sets the ios.
        /// </summary>
        /// <value>
        /// The ios.
        /// </value>
        [JsonProperty("ios", NullValueHandling = NullValueHandling.Ignore)]
        public IOS IOS { get; set; }
    }

    /// <summary>
    /// Class FiiiPay.Data.Agents.JPush.Model.Android
    /// </summary>
    public class Android
    {
        /// <summary>
        /// Gets or sets the alert.
        /// </summary>
        /// <value>
        /// The alert.
        /// </value>
        [JsonProperty("alert")]
        public string Alert { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the builder identifier.
        /// </summary>
        /// <value>
        /// The builder identifier.
        /// </value>
        [JsonProperty("builder_id")]
        public int BuilderId { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        [JsonProperty("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        [JsonProperty("style")]
        public int Style { get; set; }

        /// <summary>
        /// Gets or sets the type of the alert.
        /// </summary>
        /// <value>
        /// The type of the alert.
        /// </value>
        [JsonProperty("alert_type")]
        public int AlertType { get; set; }

        /// <summary>
        /// Gets or sets the big text.
        /// </summary>
        /// <value>
        /// The big text.
        /// </value>
        [JsonProperty("big_text")]
        public string BigText { get; set; }

        /// <summary>
        /// Gets or sets the inbox.
        /// </summary>
        /// <value>
        /// The inbox.
        /// </value>
        [JsonProperty("inbox")]
        public Dictionary<string, object> Inbox { get; set; }

        /// <summary>
        /// Gets or sets the big picture path.
        /// </summary>
        /// <value>
        /// The big picture path.
        /// </value>
        [JsonProperty("big_pic_path")]
        public string BigPicturePath { get; set; }

        /// <summary>
        /// Gets or sets the extras.
        /// </summary>
        /// <value>
        /// The extras.
        /// </value>
        [JsonProperty("extras")]
        public Dictionary<string, object> Extras { get; set; }
    }

    /// <summary>
    /// Class FiiiPay.Data.Agents.JPush.Model.IOS
    /// </summary>
    public class IOS
    {
        /// <summary>
        /// 可以是 string，也可以是 Apple 官方定义的 alert payload 结构。
        /// <para><see ="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html#//apple_ref/doc/uid/TP40008194-CH17-SW5"/></para>
        /// </summary>
        [JsonProperty("alert")]
        public object Alert { get; set; }

        /// <summary>
        /// Gets or sets the sound.
        /// </summary>
        /// <value>
        /// The sound.
        /// </value>
        [JsonProperty("sound")]
        public string Sound { get; set; }

        /// <summary>
        /// Gets or sets the badge.
        /// </summary>
        /// <value>
        /// The badge.
        /// </value>
        [JsonProperty("badge")]
        public string Badge { get; set; } = "+1";

        /// <summary>
        /// Gets or sets a value indicating whether [content available].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [content available]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("content-available")]
        public bool ContentAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [mutable content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mutable content]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("mutable-content")]
        public bool MutableContent { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the extras.
        /// </summary>
        /// <value>
        /// The extras.
        /// </value>
        [JsonProperty("extras")]
        public Dictionary<string, object> Extras { get; set; }
    }
}
