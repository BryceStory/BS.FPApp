using System.Configuration;
using System.Threading.Tasks;
using FiiiPay.Data.Agents.JPush;
using FiiiPay.Data.Agents.JPush.Model;
using log4net;

namespace FiiiPay.Data.Agents
{
    /// <summary>
    /// Class FiiiPay.Data.Agents.JPushAgent
    /// </summary>
    public class JPushAgent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(JPushAgent));

        private static readonly string appkey = ConfigurationManager.AppSettings.Get("JPush.AppKey");
        private static readonly string masterSecret = ConfigurationManager.AppSettings.Get("JPush.MasterSecret");

        private readonly JPushClient client = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="JPushAgent"/> class.
        /// </summary>
        public JPushAgent()
        {
            client = new JPushClient(appkey, masterSecret);
        }

        /// <summary>
        /// Updates the device information.
        /// </summary>
        /// <param name="registrationId">The registration identifier.</param>
        /// <param name="devicePayload">The device payload.</param>
        /// <returns></returns>
        public string UpdateDeviceInfo(string registrationId, DevicePayload devicePayload)
        {
            return client.Device.UpdateDeviceInfo(registrationId, devicePayload).Content;
        }
        
        /// <summary>
        /// Deletes the device by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public string DeleteDeviceByTag(string tag)
        {
            return client.Device.DeleteTag(tag, null).Content;
        }

        /// <summary>
        /// Pushes the specified push payload.
        /// </summary>
        /// <param name="pushPayload">The push payload.</param>
        /// <returns></returns>
        public string Push(PushPayload pushPayload)
        {
            return client.SendPush(pushPayload).Content;
        }

        /// <summary>
        /// Pushes the asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        public async Task PushAsync(PushPayload payload)
        {
            var jsonBody = payload.ToString();
            _log.InfoFormat("Push argument {0}", jsonBody);
            var result = await client.SendPushAsync(jsonBody);
            if (result != null)
            {
                _log.InfoFormat("Push response code :{0},cotent:{1}", result.StatusCode, result.Content);
            }
        }
    }
}
