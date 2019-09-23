using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FiiiPay.Framework.MQTT.Model;

namespace FiiiPay.Framework.MQTT
{
    public class MqttClient
    {
        private static readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        static MqttClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public MqttClient(string baseUrl, string username, string password)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            _baseUrl = baseUrl;

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        }


        public async Task<string> PushAsync(MqttMessage mqttMessage)
        {
            if (mqttMessage == null)
                throw new ArgumentNullException(nameof(mqttMessage));

            string messageJson = mqttMessage.ToString();

            HttpContent content = new StringContent(messageJson, Encoding.UTF8);

            string url = _baseUrl + "/api/v3/mqtt/publish";
            HttpResponseMessage msg = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            if (!msg.IsSuccessStatusCode)
            {
                throw new HttpException((int)msg.StatusCode, "Call mqtt borker exception happened");
            }

            var result = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
            return result;
        }

        public string Push(MqttMessage mqttMessage)
        {
            Task<string> task = Task.Run(() => PushAsync(mqttMessage));
            task.Wait();
            return task.Result;
        }
    }
}