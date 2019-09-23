using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.API.Models;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.CountryController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Country")]
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
        /// <param name="model">The identifier.</param>
        /// <returns></returns>
        [HttpPost, Route("GetById")]
        [AllowAnonymous]
        public ServiceResult<Country> GetById(GetCountryByIdModel model)
        {
            return new ServiceResult<Country>
            {
                Data = new CountryComponent().GetById(model.Id)
            };
        }

        /// <summary>
        /// 获取客服账号列表，格式：{"Phone": "PhoneTest", "WX": "WXTest"}
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("CustomerService")]
        public ServiceResult<Dictionary<string, List<string>>> CustomerService()
        {
            var result = new ServiceResult<Dictionary<string, List<string>>>();

            var cpt = new CountryComponent();
            var dic = cpt.GetCustomerService(this.IsZH());
            result.Data = dic;
            return result;
        }
    }
}