using System.Configuration;
using System.Threading.Tasks;
using FiiiPay.Framework.MQTT.Model;

namespace FiiiPay.Framework.MQTT
{
    public static class MqttAgent
    {
        private static readonly string _baseUrl = ConfigurationManager.AppSettings.Get("MqttBaseUrl");
        private static readonly string _username = ConfigurationManager.AppSettings.Get("MqttUsername");
        private static readonly string _password = ConfigurationManager.AppSettings.Get("MqttPassword");
        private static readonly MqttClient _client;
        static MqttAgent()
        {
            _client = new MqttClient(_baseUrl, _username, _password);
        }

        public static string PushMessage(MqttMessage mqttMessage)
        {
            return _client.Push(mqttMessage);
        }

        public static async Task<string> PushMessageAsync(MqttMessage mqttMessage)
        {
            return await _client.PushAsync(mqttMessage);
        }
    }
}