using Newtonsoft.Json;

namespace FiiiPay.Data.Agents.JPush.Model
{
    /// <summary>
    /// Class FiiiPay.Data.Agents.JPush.Model.DevicePayload
    /// </summary>
    public class DevicePayload
    {
        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        [JsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        /// <value>
        /// The mobile.
        /// </value>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        [JsonProperty("tags")]
        public object Tags { get; set;}

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

        private string GetJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }
    }
}
