using FiiiPay.API.Models;
using FiiiPay.Business.FiiiPay;
using FiiiPay.Framework;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// FiiiShop
    /// </summary>
    [RoutePrefix("FiiiShop")]
    public class FiiiShopController : ApiController
    {
        /// <summary>
        /// Fiiipay 扫码登录
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ScanLogin")]
        public ServiceResult<bool> ScanLogin(ScanQRCodeIM im)
        {
            return new ServiceResult<bool>
            {
                Data = new PosShopComponent().ScanLogin(im.Code, this.GetUser().Id)
            };
        }
    }
}
