using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Foundation.Business;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using MasterSettingComponent = FiiiPay.Foundation.Business.MasterSettingComponent;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 
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
            if(CustomerServices!=null)
            {
                foreach (var masterSetting in CustomerServices)
                {
                    if (!dic.ContainsKey(masterSetting.Name))
                    {
                        dic.Add(masterSetting.Name, new List<string> { masterSetting.Value });
                    }
                    else
                    {
                        dic[masterSetting.Name].Add(masterSetting.Value);
                    }
                }
            }

            result.Data = dic;
            
            
            return result;
        }


    }
}
