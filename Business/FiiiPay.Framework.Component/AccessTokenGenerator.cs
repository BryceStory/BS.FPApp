using Newtonsoft.Json;

namespace FiiiPay.Framework.Component
{
    /// <summary>
    /// Access Token Generator whith AES128
    /// <see cref="FiiiPay.Framework.AES128"/>
    /// </summary>
    public static class AccessTokenGenerator
    {
        private const string DefaultKey = "4048716E-ED5F-44FA-869E-E9D058D2DDC5";
        /// <summary>
        /// 默认过期时间 （15 days）
        /// </summary>
        public const int DefaultExpiryTime = 2592000;

        /// <summary>
        /// base token issue
        /// </summary>
        /// <param name="identity">The identity</param>
        /// <returns></returns>
        public static string IssueToken(string identity)
        {
            var accessToken = new AccessToken
            {
                Identity = identity
            };

            var token = IssueToken(accessToken);
            return token;
        }

        /// <summary>
        /// token issue
        /// </summary>
        /// <param name="data">access token data</param>
        /// <typeparam name="T">type of access token</typeparam>
        /// <returns></returns>
        public static string IssueToken<T>(T data) where T : AccessToken
        {
            string dataString = JsonConvert.SerializeObject(data);
            string token = AES128.Encrypt(dataString, DefaultKey);
            return token;
        }

        /// <summary>
        /// decrypt basic access token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static AccessToken DecryptToken(string token)
        {
            return DecryptToken<AccessToken>(token);
        }

        /// <summary>
        /// decrypt access token
        /// </summary>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DecryptToken<T>(string token) where T : AccessToken
        {
            return JsonConvert.DeserializeObject<T>(AES128.Decrypt(token, DefaultKey));
        }
    }
}