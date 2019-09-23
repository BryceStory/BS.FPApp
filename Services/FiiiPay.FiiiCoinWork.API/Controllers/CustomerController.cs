using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.FiiiCoinWork.API.Extensions;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework;

namespace FiiiPay.FiiiCoinWork.API.Controllers
{
    /// <summary>
    /// 客服相关
    /// </summary>
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiController
    {
        /// <summary>
        /// 获取客服信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous, HttpGet]
        public ServiceResult<Dictionary<string, List<string>>> Get()
        {
            var result = new ServiceResult<Dictionary<string, List<string>>>();

            var cpt = new CountryComponent();

            var dic = cpt.GetCustomerService(this.IsZH());

            var CustomerServices = new MasterSettingComponent().GetSettingByGroupName("CustomerService");
            if (CustomerServices != null)
            {
                foreach (var masterSetting in CustomerServices)
                {
                    dic.Add(masterSetting.Name, new List<string> { masterSetting.Value });
                }
            }

            result.Data = dic;

            result.Successful();
            return result;
        }
    }
}
