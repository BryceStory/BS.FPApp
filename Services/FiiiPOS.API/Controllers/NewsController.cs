using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.Business;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 用于显示新闻详情
    /// </summary>
    [RoutePrefix("News")]
    public class NewsController : ApiController
    {
        /// <summary>
        /// 用于显示新闻详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Detail")]
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