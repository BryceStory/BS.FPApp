using System;
using FiiiPay.Framework.MQTT.Model;
using Newtonsoft.Json;

namespace FiiiPay.Framework.MQTT
{
    public class MqttMessageBuilder
    {
        private readonly MqttMessage _mqttMessage = new MqttMessage();
        private Topic _topic;

        public MqttMessageBuilder WithClientId(string value = "http")
        {
            this._mqttMessage.ClientId = value;
            return this;
        }

        public MqttMessageBuilder WithQos(byte value = 2)
        {
            this._mqttMessage.Qos = value;
            return this;
        }

        public MqttMessageBuilder WithRetain(bool value = true)
        {
            this._mqttMessage.Retain = value;
            return this;
        }

        public MqttMessageBuilder WithPayload<T>(T value) where T : class
        {
            this._mqttMessage.Payload = JsonConvert.SerializeObject(value);
            return this;
        }

        /// <summary>
        /// 消息主题（必填）
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="business"></param>
        /// <param name="cliendId"></param>
        /// <returns></returns>
        public MqttMessageBuilder WithTopic(string platform, string business, string cliendId)
        {
            this._topic = new Topic
            {
                Paltform = platform,
                Business = business,
                ClientId = cliendId
            };

            return this;
        }

        public MqttMessage Build()
        {
            if (_topic == null)
                throw new InvalidOperationException("A topic must be set.");
            string topic = $"{_topic.Paltform}/{_topic.Business}/{_topic.ClientId}";

            this._mqttMessage.Topic = topic;

            return this._mqttMessage;
        }
    }
}