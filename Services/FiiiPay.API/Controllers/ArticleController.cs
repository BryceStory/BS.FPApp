using FiiiPay.Business;
using FiiiPay.DTO.Article;
using FiiiPay.Entities;
using FiiiPay.Framework;
using System.Linq;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.ArticleController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Article")]
    public class ArticleController : ApiController
    {

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("List")]
        public ServiceResult<ArticleListOM> List(ArticleListIM im)
        {
            var list = new ArticleComponent().List(im.PageSize, im.PageIndex + 1, Entities.ArticleAccountType.FiiiPay, this.GetUser().Id, this.IsZH());
            return new ServiceResult<ArticleListOM>
            {
                Data = new ArticleListOM
                {
                    List = list.Select(a => new ArticleListOMItem
                    {
                        Id = a.Id,
                        Intro = a.Intro,
                        Read = a.Read,
                        Timestamp = a.CreateTime.ToUnixTime().ToString(),
                        Title = a.Title,
                        Type = a.Type
                    }).ToList()
                }
            };
        }

        /// <summary>
        /// 将新闻状态设置为已读
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Read")]
        public ServiceResult<bool> Read(ReadIM im)
        {
            new ArticleComponent().Read(im.Id, this.GetUser().Id, im.Type.Value);

            return new ServiceResult<bool>
            {
                Data = true
            };
        }

        /// <summary>
        /// 获取第一条文章的Title以及未读条数
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetFirstTitleAndNotReadCount")]
        public ServiceResult<GetFirstTitleAndNotReadCountOM> GetFirstTitleAndNotReadCount()
        {
            var om = new ArticleComponent().GetFirstTitleAndNotReadCount(ArticleAccountType.FiiiPay, this.GetUser().Id, this.IsZH());
            return new ServiceResult<GetFirstTitleAndNotReadCountOM>
            {
                Data = om
            };
        }
    }
}
