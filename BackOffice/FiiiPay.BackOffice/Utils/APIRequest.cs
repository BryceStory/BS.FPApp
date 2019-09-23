using FiiiPay.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    [Obsolete("Please use the method in BlobBLL")]
    public class APIRequest
    {
        public byte[] FileGet(string endpoint, string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GenerateToken());

                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();

                    string content = response.Content.ReadAsStringAsync().Result;
                    ServiceResult<object> result = JsonConvert.DeserializeObject<ServiceResult<object>>(content);

                    if (result.Code == (int)HttpStatusCode.OK || result.Code == 0)
                    {
                        return Convert.FromBase64String(result.Data.ToString());
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public Guid FileUpload(string endpoint, string url, object model)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GenerateToken());

                using (HttpResponseMessage response = client.PostAsJsonAsync(url,model).Result)
                {
                    response.EnsureSuccessStatusCode();

                    string content = response.Content.ReadAsStringAsync().Result;
                    ServiceResult<Guid> result = JsonConvert.DeserializeObject<ServiceResult<Guid>>(content);

                    if (result.Code == (int)HttpStatusCode.OK || result.Code == 0)
                    {
                        return result.Data;
                    }
                    else
                    {
                        throw new ApplicationException("Error occurred while uploading file to server. Error Message:");
                    }
                }
            }
        }

        private string GenerateToken()
        {
            string clientKey = ConfigurationManager.AppSettings["Blob_ClientKey"];
            string secretKey = ConfigurationManager.AppSettings["Blob_SecretKey"];

            string password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + clientKey;
            string token = AES128.Encrypt(password, secretKey);

            return token;
        }
    }
}