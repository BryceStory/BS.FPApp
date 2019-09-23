using System;
using System.Collections.Generic;
using FiiiPay.Foundation.Data;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.Foundation.Business.Agent
{
    /// <summary>
    /// 处理Region KYC服务器图片
    /// </summary>
    /// <seealso cref="IImage" />
    public class RegionImageAgent : IImage
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(RegionImageAgent));
        private readonly ProfileRouter _router;

        public RegionImageAgent(int countryId)
        {
            _router = new ProfileRouterDAC().GetRouter(countryId);
        }

        public byte[] Download(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            ServiceResult<string> result;
            var index = 0;
            do
            {
                result = DownloadImage(id);
                index++;
            } while (string.IsNullOrWhiteSpace(result.Data) && index < 3);

            if (string.IsNullOrWhiteSpace(result.Data))
            {
                _log.ErrorFormat("Download region image error, id = {0}, profile route = {1}", id, _router);

                return null;
            }

            return Convert.FromBase64String(result.Data);
        }

        private ServiceResult<string> DownloadImage(string id)
        {
            var url = _router.ServerAddress + "/File/Download?id=" + id;

            var resultStr = RestUtilities.GetJson(url, GetHeaders());
            if (string.IsNullOrWhiteSpace(resultStr))
            {
                _log.Error($"Download region image error. url = {url}, profile route = {_router}");
            }
            var responseJson = JsonConvert.DeserializeObject<ServiceResult<string>>(resultStr);

            if (responseJson == null)
            {
                throw new ApplicationException();
            }

            if (responseJson.Code != 0)
            {
                throw new ApplicationException(responseJson.Data);
            }

            return responseJson;
        }

        public string Upload(string fileName, byte[] bytes)
        {
            var url = _router.ServerAddress + "/File/Upload";

            var json = JsonConvert.SerializeObject(new
            {
                FileName = fileName,
                File = bytes,
                FileType = "img"
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
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + _router.ClientKey;
            var token = AES128.Encrypt(password, _router.SecretKey);

            return new Dictionary<string, string> { { "Authorization", "bearer " + token } };
        }
    }
}
