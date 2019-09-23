using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.DTO;
using FiiiPay.FiiiCoinWork.API.Extensions;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;

namespace FiiiPay.FiiiCoinWork.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.FiiiCoinWork.API.Controllers.CountryController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api/Country")]
    public class CountryController : ApiController
    {
        /// <summary>
        /// 获取国家列表，已经根据客户端语言排序
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetList")]
        [AllowAnonymous]
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
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("GetById")]
        [AllowAnonymous]
        public ServiceResult<Country> GetById(GetDetailByIdIM<int> im)
        {
            return new ServiceResult<Country>
            {
                Data = new CountryComponent().GetById(im.Id)
            };
        }

    }
}