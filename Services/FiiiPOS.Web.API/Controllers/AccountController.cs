using FiiiPay.Framework;
using FiiiPOS.Web.API.Base;
using FiiiPOS.Web.API.Models.Input;
using FiiiPOS.Web.Business;
using log4net;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace FiiiPOS.Web.API.Controllers
{
    /// <summary>
    /// 登录相关接口
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : BaseApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected ILog Logger = WebLog.GetInstance();

        /// <summary>
        /// 获取登录二维码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetLoginQRcode"), AllowAnonymous]
        public ServiceResult<string> GetLoginQRcode()
        {
            try
            {
                Logger.Info("Get code");
                string strCode = new MerchantComponent().GetLoginQRcode();
                return Result_OK(strCode);
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat("Get QR code error {0}", exception.Message);
            }

            return new ServiceResult<string>();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>0=还没有扫码 -1=qrcode已失效 -2=创建token失败 -3=用户信息不存在  1=成功</returns>
        [HttpPost, Route("Login"), AllowAnonymous]
        public async Task<ServiceResult<string>> Login(LoginInModel model)
        {

            ServiceResult<string> result = new ServiceResult<string>();
            Logger.Info(string.Format("model.QRCode:{0}", model.QRCode));

            return await Task.Run(() =>
            {
                DateTime startDateTime = DateTime.Now;

                string token = string.Empty;
                int isOk = 0;
                int i = 0;
                while (true)
                {
                    //要保持长链接
                    i++;
                    isOk = new MerchantComponent().GetLoginData(model.QRCode, out token);
                    result.Data = token;
                    if (isOk != 0)
                        break;

                    TimeSpan ts = DateTime.Now - startDateTime;
                    if (ts.TotalSeconds >= 120)
                    {
                        isOk = -1;
                        break;
                    }

                    Thread.Sleep(2000);
                }
                if (isOk > 0)
                    return Result_OK(token);
                else
                    return Result_Fail(isOk.ToString(), 10000, "获取token失败");
            });
        }

        ///// <summary>
        ///// 测试扫码
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost, Route("ScanQRLogin"), AllowAnonymous]
        //public ServiceResult<int> ScanQRLogin(ScanQRLoginInModel model)
        //{
        //    bool result = new MerchantComponent().ScanQRLogin(model.MerchantId,model.QRCode);
        //    if(result)
        //        return Result_OK(1);
        //    return Result_Fail(-1, "二维码已无效");
        //}

        ///// <summary>
        ///// 测试Merchant账号登录
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost, Route("TestLogin"), AllowAnonymous]
        //public ServiceResult<string> TestLogin(TestLoginInModel model)
        //{
        //    string token = string.Empty;
        //    bool result = FiiiPOS.Web.Business.WebRedis.SetWebTokenIndRedis(model.MerchantId, out token);
        //    if (result)
        //        return Result_OK(token);
        //    return Result_Fail(token, -1, "获取token失败");
        //}
    }
}
