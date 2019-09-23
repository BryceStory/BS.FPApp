using FiiiPay.Framework;
using System.Linq;
using System.Web.Http;
using FiiiPay.Foundation.API.Controllers;
using FiiiPay.Foundation.API.Models;
using FiiiPay.Foundation.Business;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 法币
    /// </summary>
    [RoutePrefix("Currency")]
    public class CurrencyController : BaseFoundationController
    {
        /// <summary>
        /// 获取法币列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetList"), AllowAnonymous]
        public ServiceResult<CurrencyListOM> GetList()
        {
            var result = new ServiceResult<CurrencyListOM>();

            var currencyList = new CurrencyComponent().GetList(IsZH());

            var list = currencyList.Select(s => new CurrencyItem
            {
                Code = s.Code,
                Id = s.ID,
                Name = s.Name
            }).ToList();

            result.Data = new CurrencyListOM
            {
                DefaultCurrency = list.FirstOrDefault(),
                CurrencyList = list
            };

            return result;
        }
    }
}
