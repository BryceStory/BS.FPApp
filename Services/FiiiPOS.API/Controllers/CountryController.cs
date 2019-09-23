using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using System.Collections.Generic;
using System.Web.Http;

namespace FiiiPOS.API.Controllers
{
    [RoutePrefix("api/Country")]
    public class CountryController : ApiController
    {
        /// <summary>
        /// 分组获取国家列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpGet, Route("GetList")]
        public ServiceResult<List<Country>> GetList()
        {
            return new ServiceResult<List<Country>>
            {
                Data = new CountryComponent().GetList(this.IsZH())
            };
        }

        /// <summary>
        /// 获取国家详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpGet, Route("GetById")]
        public ServiceResult<Country> GetById(int id)
        {
            return new ServiceResult<Country>
            {
                Data = new CountryComponent().GetById(id)
            };
        }
    }
}
