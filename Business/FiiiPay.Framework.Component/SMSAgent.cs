using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using log4net;

namespace FiiiPay.Data.Agents
{
    public class SMSAgent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(SMSAgent));
        
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="recipients">收件人，支持多收件人，使用逗号分割</param>
        /// <param name="content">内容</param>
        /// <param name="SendLevel">优先级</param>
        /// <returns>是否成功</returns>
        public bool Send(string recipients, string content, int SendLevel)
        {
            var result = SendByResult(recipients, content, SendLevel);
            return result.Contains("发送成功");
        }

        public string SendByResult(string recipients, string content, int SendLevel)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["FP_EMAIL_API__URL"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ConfigurationManager.AppSettings["Email_Token"]);
                    var model = new { Recipients = recipients, Content = content, SendLevel };
                    using (HttpResponseMessage response = client.PostAsJsonAsync("api/Message/PostSMS", model).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;
                        return result;
                    }
                }

                //var model = new { Recipients = recipients, Content = content, SendLevel };
                //var result = RestUtilities.PostJson(Constant.FP_EMAIL_API__URL + "/api/Message/PostSMS", new Dictionary<string, string> { { "bearer", Constant.EMAIL_TOKEN } }, JsonConvert.SerializeObject(model));

                //return result;
            }
            catch (Exception exception)
            {
                _log.Error(exception);
            }

            return string.Empty;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="recipients">收件人，支持多收件人，使用逗号分割</param>
        /// <param name="content">内容</param>
        /// <param name="SendLevel">优先级</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SendAsync(string recipients, string content, int SendLevel)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["FP_EMAIL_API__URL"]);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ConfigurationManager.AppSettings["Email_Token"]);
                    var model = new { Recipients = recipients, Content = content, SendLevel };
                    using (HttpResponseMessage response = await client.PostAsJsonAsync("api/Message/PostSMS", model))
                    {
                        response.EnsureSuccessStatusCode();
                        string result = await response.Content.ReadAsStringAsync();
                        return result.Contains("发送成功");
                    }
                }

                //var model = new { Recipients = recipients, Content = content, SendLevel };
                //var result = await RestUtilities.PostJsonAsync(Constant.FP_EMAIL_API__URL + "/api/Message/PostSMS", new Dictionary<string, string> { { "bearer", Constant.EMAIL_TOKEN } }, JsonConvert.SerializeObject(model));
                //return result.Contains("发送成功");
            }
            catch (Exception e)
            {
                _log.Error(e);
            }

            return false;
        }
    }
}