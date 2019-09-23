using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;

namespace FiiiPay.Data.Agents
{
    public class EmailAgent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(EmailAgent));

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipients">收件人，支持多收件人，使用逗号分割</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="SendLevel">优先级</param>
        /// <param name="merchant"></param>
        /// <returns>是否成功</returns>
        public bool Send(string recipients, string subject, string content, int SendLevel,string merchant = "FiiiPOS")
        {
            var model = new { Recipients = recipients, Subject = subject, Content = content, Merchant = merchant, SendLevel = SendLevel };
            //var result = RestUtilities.PostJson(Constant.FP_EMAIL_API__URL + "/api/Message/PostEmail", new Dictionary<string, string> { { "bearer", Constant.EMAIL_TOKEN } }, JsonConvert.SerializeObject(model));
            //return result.Contains("send success");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["FP_EMAIL_API__URL"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ConfigurationManager.AppSettings["Email_Token"]);
                using (HttpResponseMessage response = client.PostAsJsonAsync("api/Message/PostEmail", model).Result)
                {
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    var resl = result.Contains("send success");

                    if (!resl)
                    {
                        _log.ErrorFormat("send sms {0} ---- {1}", JsonConvert.SerializeObject(model), result);
                    }

                    return resl;
                }
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipients">收件人，支持多收件人，使用逗号分割</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="SendLevel">优先级</param>
        /// <param name="merchant"></param>
        /// <returns>是否成功</returns>
        public async Task<bool> SendAsync(string recipients, string subject, string content, int SendLevel,string merchant = "FiiiPOS")
        {
            //var model = new { Recipients = recipients, Subject = subject, Content = content, Merchant = "FiiiPOS", SendLevel = SendLevel };
            //var result = await RestUtilities.PostJsonAsync(Constant.FP_EMAIL_API__URL + "/api/Message/PostEmail", new Dictionary<string, string> { { "bearer", Constant.EMAIL_TOKEN } }, JsonConvert.SerializeObject(model));
            //return result.Contains("send success");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["FP_EMAIL_API__URL"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ConfigurationManager.AppSettings["Email_Token"]);
                var model = new { Recipients = recipients, Subject = subject, Content = content, Merchant = merchant, SendLevel = SendLevel };

                using (HttpResponseMessage response = await client.PostAsJsonAsync("api/Message/PostEmail", model))
                {
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    
                    var resl= result.Contains("send success");

                    if (!resl)
                    {
                        _log.ErrorFormat("send email {0} ---- {1}", JsonConvert.SerializeObject(model), result);
                    }

                    return resl;
                }
            }
        }
    }
}
