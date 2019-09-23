using System;
using System.Collections.Generic;
using System.Configuration;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using Newtonsoft.Json;

namespace FiiiPay.Foundation.Business.Agent
{
    public class LogFileAgent
    {
        private readonly string _uploadUrl = ConfigurationManager.AppSettings["Foundation_URL"];

        private readonly string _clientKey = ConfigurationManager.AppSettings["Foundation_ClientKey"];

        private readonly string _secretKey = ConfigurationManager.AppSettings["Foundation_SecretKey"];


        public string Upload(string fileName, byte[] bytes)
        {
            var url = _uploadUrl + "/File/Upload";

            var json = JsonConvert.SerializeObject(new
            {
                FileName = fileName,
                File = bytes,
                FileType = "txt"
            });
            var result = RestUtilities.PostJson(url, GetHeaders(), json);
            var data = JsonConvert.DeserializeObject<ServiceResult<string>>(result);

            if (data.Code != 0)
            {
                throw new CommonException(ReasonCode.GENERAL_ERROR, data.Message);
            }
            return data.Data;
        }

        private Dictionary<string, string> GetHeaders()
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + _clientKey;
            var token = AES128.Encrypt(password, _secretKey);

            return new Dictionary<string, string> { { "Authorization", "bearer " + token } };
        }
    }
}
