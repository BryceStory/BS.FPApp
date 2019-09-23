using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.MessageWorkerService
{
    public class FiiiFinanceAgent
    {
        private readonly string URL;
        private readonly string ClientKey;
        private readonly string SecretKey;
        public FiiiFinanceAgent()
        {
            URL = ConfigurationManager.AppSettings.Get("Finance_URL");
            ClientKey = ConfigurationManager.AppSettings.Get("Finance_ClientKey");
            SecretKey = ConfigurationManager.AppSettings.Get("Finance_SecretKey");
        }

        public WithdrawRequestInfo CreateWithdraw(CreateWithdrawModel model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken());

                var jsonStr = model.ToString();
                HttpContent content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync($"{URL}/Wallet/Withdraw/Request", content).Result;

                    if (response.StatusCode >= HttpStatusCode.BadRequest)
                    {
                        LogHelper.Error($"Failed: Create withdraw bad request." +
                                   $"URL: {response.RequestMessage.RequestUri}\r\n" +
                                   $"Parameters: {jsonStr}\r\n" +
                                   $"ErrorCode: {(int)response.StatusCode}");
                        throw new CommonException((int)response.StatusCode, "create withdraw request failed.");
                    }

                    var result = response.Content.ReadAsStringAsync().Result;
                    LogHelper.Info($"Success: Create withdraw ok request." +
                              $"URL: {response.RequestMessage.RequestUri}\r\n" +
                              $"Parameters: {jsonStr}\r\n" +
                              $"Result: {result}");
                    var data = JsonConvert.DeserializeObject<ServiceResult<WithdrawRequestInfo>>(result);
                    if (data.Code == 0)
                    {
                        return data.Data;
                    }

                    if (data.Data == null)
                    {
                        data.Data = new WithdrawRequestInfo();
                    }

                    data.Data.Code = data.Code;
                    return data.Data;
                }
                catch (Exception exception)
                {
                    LogHelper.ErrorFormat("Request Finance exception {0}, detail exception {1}", exception.Message, exception.InnerException?.Message);
                    //LogHelper.Error(exception);
                    throw exception;
                }

                //throw new CommonException(10000, "Create Withdraw Finance Error");
            }
        }

        private string GenerateToken()
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ClientKey;
            var token = AES128.Encrypt(password, SecretKey);
            return token;
        }
    }

    public class WithdrawRequestInfo
    {
        public long RequestID { get; set; }
        public string TransactionId { get; set; }

        public int Code { get; set; }
    }

    public class CreateWithdrawModel
    {
        public Guid AccountID { get; set; }
        public string CryptoName { get; set; }
        public AccountTypeEnum AccountType { get; set; }
        public string ReceivingAddress { get; set; }
        public string DestinationTag { get; set; }
        public decimal Amount { get; set; }
        public string IPAddress { get; set; }
        public decimal TransactionFee { get; set; }
        public long WithdrawalId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum AccountTypeEnum
    {
        User = 1,
        Merchant = 2
    }
}
