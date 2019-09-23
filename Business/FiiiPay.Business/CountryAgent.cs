using FiiiPay.Framework;
using System;
using System.Collections.Generic;

namespace FiiiPay.Data.Agents.BO
{
    public class CountryAgent
    {
        public bool CheckBlobRouters(string ServerAddress, string ClientKey, string SecretKey)
        {
            var token = GenerateToken(ClientKey, SecretKey);
            var url = $"{ServerAddress}/Security/Check";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + token);
            var result = RestUtilities.GetJson(url, headers);
            if (result == "\"OK\"")
                return true;
            return false;
        }

        public bool CheckFiiiFinanceRouters(string ServerAddress, string ClientKey, string SecretKey)
        {
            var token = GenerateToken(ClientKey, SecretKey);
            var url = $"{ServerAddress}/Security/Check";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + token);
            var result = RestUtilities.GetJson(url, headers);
            if (result == "\"OK\"")
                return true;
            return false;
        }

        public bool CheckProfileRouters(string ServerAddress, string ClientKey, string SecretKey)
        {
            var token = GenerateToken(ClientKey, SecretKey);
            var url = $"{ServerAddress}/Security/Check";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + token);
            var result = RestUtilities.GetJson(url, headers);
            if (result == "\"OK\"")
                return true;
            return false;
        }
        private string GenerateToken(string ClientKey, string SecretKey)
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ClientKey;
            var token = AES128.Encrypt(password, SecretKey);
            return token;
        }
    }
}