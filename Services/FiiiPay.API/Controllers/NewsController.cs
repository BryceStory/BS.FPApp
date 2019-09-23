using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 用于显示新闻详情
    /// </summary>
    [RoutePrefix("News")]
    public class NewsController : ApiController
    {
        /// <summary>
        /// 显示新闻详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<string> Detail(int id)
        {
            var result = new ServiceResult<string>
            {
                Data = new ArticleComponent().GetNews(id)
            };
            
            return result;
        }
    }
}