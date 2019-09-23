using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 闪屏广告
    /// </summary>
    [RoutePrefix("Advertising")]
    public class AdvertisingController : ApiController
    {
        /// <summary>
        /// 获取当前闪屏广告信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Get"), AllowAnonymous]
        public async Task<ServiceResult<AdvertisingModelcs>> Get()
        {
            ServiceResult<AdvertisingModelcs> result = new ServiceResult<AdvertisingModelcs>();
            
            var advertising = await new AdvertisingComponent().GetActiveSingle();

            if (advertising == null)
            {
                result.Data = null;
                return result;
            }
            
            result.Data = new AdvertisingModelcs
            {
                SingleId = advertising.Id,
                Version = advertising.Version.ToString(),
                OpenByAPP = advertising.LinkType == Entities.Enums.LinkType.APP,
                LinkUrl = advertising.Link,
                ImageId = advertising.PictureEn,
                CNImageId = advertising.PictureZh,
                StartTime = advertising.StartDate.ToLocalTime().ToUnixTime(),
                EndTime = advertising.EndDate.ToLocalTime().ToUnixTime(),
                Status = advertising.Status
            };

            return result;
        }

        /// <summary>
        /// 获取当前闪屏广告信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetStoreBanners"), AllowAnonymous]
        public async Task<ServiceResult<List<StoreBannerOM>>> GetStoreBanners()
        {
            ServiceResult<List<StoreBannerOM>> result = new ServiceResult<List<StoreBannerOM>>();

            var account = this.GetUser();
            int countryId = account == null ? 0 : account.CountryId;
            var bannerList = await new AdvertisingComponent().GetStoreBanners(countryId);

            if (bannerList == null)
            {
                result.Data = null;
                return result;
            }

            List<StoreBannerOM> resultList = new List<StoreBannerOM>();
            foreach (var item in bannerList)
            {
                resultList.Add(new StoreBannerOM
                {
                    OpenByAPP = item.OpenByAPP,
                    StartTime = item.StartTime.ToUnixTime(),
                    EndTime = item.EndTime.ToUnixTime(),
                    ViewPermission = (byte)item.ViewPermission,
                    Title = item.Title,
                    LinkUrl = item.LinkUrl,
                    ImageId = item.PictureId
                });
            }

            result.Data = resultList;

            return result;
        }
    }
}
