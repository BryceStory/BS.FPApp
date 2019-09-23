using Newtonsoft.Json;

namespace FiiiPay.Framework.MQTT.Model
{
    public class MqttMessage
    {
        /// <summary>
        /// 主题（必填）
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("payload", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Payload { get; set; }

        [JsonProperty("qos", DefaultValueHandling = DefaultValueHandling.Include)]
        public byte Qos { get; set; } = 2;

        [JsonProperty("retain", DefaultValueHandling = DefaultValueHandling.Include)]
        public bool Retain { get; set; } = false;

        [JsonProperty("client_id", DefaultValueHandling = DefaultValueHandling.Include)]
        public string ClientId { get; set; } = "http";

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }
    }

    public class Topic
    {
        /// <summary>
        /// 平台
        /// </summary>
        public string Paltform { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        public string Business { get; set; }
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }
    }
}