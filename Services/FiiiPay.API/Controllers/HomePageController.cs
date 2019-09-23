using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.DTO.HomePage;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.HomePageController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("HomePage")]
    public class HomePageController : ApiController
    {
        /// <summary>
        /// 获取首页数据，总金额已经由服务端计算
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Index")]
        public ServiceResult<IndexOM> Index()
        {
            return new ServiceResult<IndexOM>
            {
                Data = new HomePageComponent().Index(this.GetUser())
            };
        }

        /// <summary>
        /// 准备调整币种交易顺序
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PreReOrder")]
        public ServiceResult<PreReOrder1OM> PreReOrder()
        {
            return new ServiceResult<PreReOrder1OM>
            {
                Data = new HomePageComponent().PreReOrder(this.GetUser())
            };
        }

        /// <summary>
        /// 排序（功能尚未实现）
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ReOrder")]
        public ServiceResult<bool> ReOrder(ReOrder1IM im)
        {
            new HomePageComponent().ReOrder(this.GetUser(), im.IdList);

            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }

        /// <summary>
        /// 切换币种在首页的显示与隐藏
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ToggleShowInHomePage")]
        public ServiceResult<bool> ToggleShowInHomePage(ToggleShowInHomePageIM im)
        {
            new HomePageComponent().ToggleShowInHomePage(this.GetUser(), im.CoinId);

            return new ServiceResult<bool>
            {
                Data = true
                
            };
        }
    }
}
