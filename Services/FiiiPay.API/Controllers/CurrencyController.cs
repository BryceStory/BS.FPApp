using FiiiPay.Framework;
using System.Linq;
using System.Web.Http;
using FiiiPay.API.Models;
using FiiiPay.Foundation.Business;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 法币
    /// </summary>
    [RoutePrefix("Currency")]
    public class CurrencyController : ApiController
    {
        /// <summary>
        /// 获取法币列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetList"), AllowAnonymous]
        public ServiceResult<CurrencyListOM> GetList()
        {
            var result = new ServiceResult<CurrencyListOM>();

            var currencyList = new CurrencyComponent().GetList(this.IsZH());

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
