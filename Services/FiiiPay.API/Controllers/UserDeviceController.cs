using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Business.FiiiPay;
using FiiiPay.DTO;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 闪屏广告
    /// </summary>
    [RoutePrefix("UserDevice")]
    public class UserDeviceController : ApiController
    {
        /// <summary>
        /// 删除设备信息
        /// </summary>
        /// <returns>状态码0则代表成功</returns>
        [HttpPost, Route("DeleteDevice")]
        public ServiceResult<bool> DeleteDevice(UserDeviceDeleteIM im)
        {
            new UserDeviceComponent().DeleteDevice(im);
            return new ServiceResult<bool>() { Data = true };
        }
        /// <summary>
        /// 更新设备
        /// </summary>
        /// <returns>状态码0则代表成功</returns>
        [HttpPost, Route("UpdateDeviceInfo")]
        public ServiceResult<bool> UpdateDeviceInfo(UserDeviceUpdateIM im)
        {
            var result = new ServiceResult<bool>();
            string deviceNumber = this.GetDeviceNumber();
            if (string.IsNullOrEmpty(deviceNumber))
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                return result;
            }

            string ip = this.GetClientIPAddress();
            //    HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            new UserDeviceComponent().UpdateDeviceInfo(this.GetUser().Id, im, ip, deviceNumber);
            result.Data = true;
            return result;
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetDeviceList")]
        public ServiceResult<List<UserDeviceItemOM>> GetDeviceList()
        {
            return new ServiceResult<List<UserDeviceItemOM>>()
            {
                Data = new UserDeviceComponent().GetDeviceList(this.GetUser())
            };
        }
    }
}