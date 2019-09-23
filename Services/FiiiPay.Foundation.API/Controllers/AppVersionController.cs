using System.Configuration;
using System.Linq;
using System.Web.Http;
using FiiiPay.Foundation.API.Models;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FiiiPay.Foundation.API.Controllers
{
    [RoutePrefix("AppVersion")]
    public class AppVersionController : ApiController
    {
        /// <summary>
        /// 版本号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Version"), AllowAnonymous]
        public ServiceResult<VersionModel> Version(VersionRequest model)
        {
            //var appVersion = new AppVersionComponent().GetLatestByPlatform((byte)model.Platform, (byte)model.App);

            var json = RedisHelper.StringGet(4, $"Foundation:Version:{model.App}:{model.Platform}");
            if (string.IsNullOrWhiteSpace(json))
            {
                var version = new AppVersionComponent().GetLatestByPlatform((byte)model.Platform, (byte)model.App);

                RedisHelper.StringSet(4, $"Foundation:Version:{model.App}:{model.Platform}",
                    JsonConvert.SerializeObject(version));
                return new ServiceResult<VersionModel>
                {
                    Data = new VersionModel
                    {
                        ForceToUpdate = version.ForceToUpdate,
                        Url = version.Url,
                        Version = version.Version
                    }
                };
            }
            var appVersion = JsonConvert.DeserializeObject<VersionModel>(json);
            appVersion.Version1 = appVersion.Version;
            appVersion.ForceToUpdate1 = appVersion.ForceToUpdate;
            appVersion.Url1 = appVersion.Url;

            return new ServiceResult<VersionModel>
            {
                Data = appVersion
            };
        }

        /// <summary>
        /// 获取Api地址
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("ApiAddress"), AllowAnonymous]
        public ServiceResult<JObject> ApiAddress(ApiAddressModel model)
        {
            var addressStr = ConfigurationManager.AppSettings.Get("ApiAddress");

            var result = new ServiceResult<JObject> { Data = JsonConvert.DeserializeObject<JObject>(addressStr) };

            result.Success();
            return result;
        }

        private bool IsZH()
        {
            var firstLng = Request.Headers.AcceptLanguage.FirstOrDefault()?.Value;
            return !string.IsNullOrEmpty(firstLng) && firstLng.Contains("zh");
        }
    }
}