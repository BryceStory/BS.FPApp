using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using FiiiPay.Framework;
using FiiiPay.Framework.Component.Exceptions;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.Foundation.Business.Agent
{
    public class FiiiFinanceAgent
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(FiiiFinanceAgent));

        private readonly string URL;
        private readonly string ClientKey;
        private readonly string SecretKey;

        public FiiiFinanceAgent()
        {
            URL = ConfigurationManager.AppSettings.Get("Finance_URL");
            ClientKey = ConfigurationManager.AppSettings.Get("Finance_ClientKey");
            SecretKey = ConfigurationManager.AppSettings.Get("Finance_SecretKey");
        }

        public WalletAddressInfo CreateWallet(string cryptoCode, Guid accountId, AccountTypeEnum accountType, string email = null, string cellphone = null)
        {
            var url = $"{URL}/Wallet/Create";
            _log.Info($"URL--{url}");

            var parameters = new
            {
                CryptoName = cryptoCode,
                AccountID = accountId,
                AccountType = accountType,
                Email = email,
                Cellphone = cellphone
            };
            var paramString = JsonConvert.SerializeObject(parameters);
            _log.Info($"Parameters--{paramString}");

            var dic = new Dictionary<string, string> { { "Authorization", "Bearer " + GenerateToken() } };

            var result = RestUtilities.PostJson(url, dic, paramString);
            _log.Info($"Result--{result}");

            var data = JsonConvert.DeserializeObject<ServiceResult<WalletAddressInfo>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        ///// <summary>
        ///// 提币申请
        ///// </summary>
        ///// <param name="accountId">帐号</param>
        ///// <param name="accountType">帐号类型</param>
        ///// <param name="cryptoCode">加密币Code</param>
        ///// <param name="address">提现地址</param>
        ///// <param name="amount">提现金额</param>
        ///// <param name="iPAddress">ip</param>
        ///// <param name="transactionFee"></param>
        ///// <returns>RequestId</returns>
        //public WithdrawRequestInfo CreateWithdrawRequest(Guid accountId, AccountTypeEnum accountType, string cryptoCode, string address, string tag, decimal amount, string iPAddress, decimal transactionFee)
        //{
        //    var url = $"{URL}/Wallet/Withdraw/Request";
        //    _log.Info($"URL--{url}");

        //    var parameters = new
        //    {
        //        AccountID = accountId,
        //        CryptoName = cryptoCode,
        //        AccountType = accountType,
        //        ReceivingAddress = address,
        //        DestinationTag = tag,
        //        Amount = amount,
        //        IPAddress = iPAddress,
        //        TransactionFee = transactionFee
        //    };
        //    var paramString = JsonConvert.SerializeObject(parameters);
        //    _log.Info($"Parameters--{paramString}");

        //    var dic = new Dictionary<string, string> { { "Authorization", "Bearer " + GenerateToken() } };
        //    var result = RestUtilities.PostJson(url, dic, paramString);
        //    _log.Info($"result--{result}");

        //    var data = JsonConvert.DeserializeObject<ServiceResult<WithdrawRequestInfo>>(result);
        //    if (data.Code == 0)
        //    {
        //        return data.Data;
        //    }
        //    throw new CommonException(data.Code, data.Message);
        //}

        public WithdrawRequestInfo TryCreateWithdraw(CreateWithdrawModel model)
        {
            return CreateWithdraw(model);
            //var tryCount = 3;

            //for (var i = 0; i < tryCount; i++)
            //{
            //    try
            //    {
            //        return CreateWithdraw(model);
            //    }
            //    catch (FiiiFinanceException ex)
            //    {
            //        Thread.Sleep(1000);
            //        _log.Error(ex);
            //    }
            //}

            //throw new CommonException(ReasonCode.GENERAL_ERROR, "create withdraw request failed.");
        }

        //public WithdrawRequestInfo CreateWithdraw(Guid accountId, AccountTypeEnum accountType,
        //    string cryptoCode, string address, string tag, decimal amount, string iPAddress, decimal transactionFee,
        //    long withdrawId)
        //{
        //    try
        //    {
        //        return CreateWithdraw(accountId, accountType, cryptoCode, address, tag, amount, iPAddress, transactionFee, withdrawId);
        //    }
        //    catch (AggregateException aex)
        //    {
        //        if (aex.InnerException != null)
        //        {
        //            throw aex.InnerException;
        //        }
        //        throw;
        //    }
        //}

        //public WithdrawRequestInfo CreateWithdraw(Guid accountId, AccountTypeEnum accountType,
        //    string cryptoCode, string address, string tag, decimal amount, string iPAddress, decimal transactionFee, long withdrawId)
        //{
        //    var model = new CreateWithdrawModel
        //    {
        //        AccountID = accountId,
        //        CryptoName = cryptoCode,
        //        AccountType = accountType,
        //        ReceivingAddress = address,
        //        DestinationTag = tag,
        //        Amount = amount,
        //        IPAddress = iPAddress,
        //        TransactionFee = transactionFee,
        //        WithdrawalId = withdrawId
        //    };

        //    return CreateWithdraw(model);
        //}

        public WithdrawRequestInfo CreateWithdraw(CreateWithdrawModel model)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken());

                var jsonStr = model.ToString();
                HttpContent content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

                var response = client.PostAsync($"{URL}/Wallet/Withdraw/Request", content).Result;

                if (response.StatusCode >= HttpStatusCode.BadRequest)
                {
                    _log.Error($"Failed: Create withdraw bad request." +
                               $"URL: {response.RequestMessage.RequestUri}\r\n" +
                               $"Parameters: {jsonStr}\r\n" +
                               $"ErrorCode: {(int)response.StatusCode}");
                    throw new FiiiFinanceException((int)response.StatusCode, "Create withdraw request failed.");
                }

                var result = response.Content.ReadAsStringAsync().Result;
                _log.Info($"Success: Create withdraw ok request." +
                          $"URL: {response.RequestMessage.RequestUri}\r\n" +
                          $"Parameters: {jsonStr}\r\n" +
                          $"Result: {result}");
                var data = JsonConvert.DeserializeObject<ServiceResult<WithdrawRequestInfo>>(result);
                if (data.Code == 0)
                    return data.Data;
                else
                    throw new CommonException(data.Code, data.Message);
            }
        }

        public WithdrawStatusInfo GetStatus(long requestId)
        {
            var url = $"{URL}/Wallet/Withdraw/GetStatus";
            _log.Info($"URL--{url}");

            var parameters = new
            {
                RequestID = requestId
            };

            var dic = new Dictionary<string, string> { { "Authorization", "Bearer " + GenerateToken() } };

            var paramString = JsonConvert.SerializeObject(parameters);
            _log.Info($"Parameters--{paramString}");
            var result = RestUtilities.PostJson(url, dic, paramString);
            _log.Info($"Result--{result}");

            var data = JsonConvert.DeserializeObject<ServiceResult<WithdrawStatusInfo>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public DepositStatusInfo GetDepositStatus(long requestId)
        {
            var url = $"{URL}/Wallet/Deposit/GetStatus";
            _log.Info($"URL--{url}");

            var parameters = new
            {
                DepositRequestId = requestId
            };

            var dic = new Dictionary<string, string> { { "Authorization", "Bearer " + GenerateToken() } };
            var paramString = JsonConvert.SerializeObject(parameters);
            _log.Info($"Parameters--{paramString}");
            var result = RestUtilities.PostJson(url, dic, paramString);
            _log.Info($"Result--{result}");

            var data = JsonConvert.DeserializeObject<ServiceResult<DepositStatusInfo>>(result);
            if (data.Code == 0)
            {
                return data.Data;
            }
            throw new CommonException(10000, data.Message);
        }

        public bool ValidateAddress(string cryptoCode, string address)
        {
            var url = $"{URL}/ValidateAddress";
            _log.Info($"URL--{url}");
            var parameters = new
            {
                Address = address,
                CryptoName = cryptoCode
            };
            var dic = new Dictionary<string, string> { { "Authorization", "Bearer " + GenerateToken() } };
            var paramString = JsonConvert.SerializeObject(parameters);
            _log.Info($"Parameters--{paramString}");
            var result = RestUtilities.PostJson(url, dic, paramString);
            _log.Info($"Result--{result}");

            var data = JsonConvert.DeserializeObject<ServiceResult<bool?>>(result);
            if (data.Code == 0)
            {
                return data.Data ?? false;
            }

            throw new CommonException(data.Code, data.Message);
        }

        private string GenerateToken()
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ClientKey;
            var token = AES128.Encrypt(password, SecretKey);
            return token;
        }
    }
}