using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.Foundation.API.Models;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Entities;
using FiiiPay.Framework;

namespace FiiiPay.Foundation.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.CountryController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Country")]
    public class CountryController : BaseFoundationController
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
                Data = new CountryComponent().GetList(IsZH())
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
            var dic = cpt.GetCustomerService(IsZH());
            result.Data = dic;
            return result;
        }

        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpPost, Route("GetCountryList")]
        public ServiceResult<List<BillerCountryOM>> GetCountryList()
        {
            return new ServiceResult<List<BillerCountryOM>>()
            {
                Data = new CountryComponent().GetList(false).Select(item => new BillerCountryOM()
                {
                    Name_CN = item.Name_CN,
                    Name = item.Name,
                    NationalFlagURL = item.NationalFlagURL,
                    Id = item.Id,
                    Enabled = string.Equals(item.Code, "aus", StringComparison.InvariantCultureIgnoreCase),
                    FiatCurrencySymbol = item.NationalFlagURL,
                    FiatCurrency = item.FiatCurrency
                }).Where(w => w.Enabled).ToList()
            };
        }

        [AllowAnonymous, HttpPost, Route("GetStoreCountryList")]
        public ServiceResult<List<Country>> GetStoreCountryList()
        {
            return new ServiceResult<List<Country>>
            {
                Data = new CountryComponent().GetList(IsZH()).Where(item => item.IsSupportStore).ToList()
            };
        }
    }
}