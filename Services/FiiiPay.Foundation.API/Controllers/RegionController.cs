using FiiiPay.Foundation.API.Models;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FiiiPay.Foundation.API.Controllers
{
    [RoutePrefix("Region")]
    public class RegionController : ApiController
    {
        private RegionComponent rc = new RegionComponent();
        /// <summary>
        /// 获取省/州列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet, Route("StateList")]
        [AllowAnonymous]
        public async Task<ServiceResult<List<RegionListOM>>> StateList(int CountryId)
        {
            List<RegionListOM> list = new List<RegionListOM>();
            var regionList = await rc.GetStateListAsync(CountryId);
            if (regionList.Any())
            {
                foreach (var item in regionList)
                {
                    list.Add(new RegionListOM
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Name_CN = item.NameCN
                    });
                }
            }
            return new ServiceResult<List<RegionListOM>>
            {
                Data = list
            };
        }

        /// <summary>
        /// 获取市级区域列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet, Route("CityList")]
        [AllowAnonymous]
        public async Task<ServiceResult<List<RegionListOM>>> CityList(long ParentId)
        {
            List<RegionListOM> list = new List<RegionListOM>();
            var regionList = await rc.GetCityListAsync(ParentId);
            if (regionList.Any())
            {
                foreach (var item in regionList)
                {
                    list.Add(new RegionListOM
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Name_CN = item.NameCN
                    });
                }
            }
            return new ServiceResult<List<RegionListOM>>
            {
                Data = list
            };
        }
    }
}
