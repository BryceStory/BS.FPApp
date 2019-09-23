using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPOS.API.Models;
using FiiiPOS.API.Extensions;
using FiiiPOS.Business;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    [RoutePrefix("api/Article")]
    public class ArticleController : ApiController
    {

        /// <summary>
        /// 文章列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("List")]
        public ServiceResult<List<ArticleModel>> List(int pageIndex = 1, int pageSize = 10)
        {
            var result = new ServiceResult<List<ArticleModel>>();
            var list = new ArticleComponent().List(pageSize, pageIndex, ArticleAccountType.FiiiPos, this.GetMerchantAccountId(), this.IsZH());
            result.Data = list.Select(a => new ArticleModel
            {
                Id = a.Id,
                Timestamp = a.CreateTime.ToUnixTime().ToString(),
                Title = a.Title,
                Read = a.Read,
                Intro = a.Intro,
                Type = a.Type
            }).ToList();            
            return result;
        }

        /// <summary>
        /// 将新闻状态设置为已读
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Read")]
        public ServiceResult<bool> Read(ReadModel model)
        {
            new ArticleComponent().Read(model.Id, this.GetMerchantAccountId(), model.Type);

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
            var om = new ArticleComponent().GetFirstTitleAndNotReadCount(ArticleAccountType.FiiiPos, this.GetMerchantAccountId(), this.IsZH());
            return new ServiceResult<GetFirstTitleAndNotReadCountOM>
            {
                Data = om
            };
        }
    }
}
