using FiiiPay.Framework;
using System.Web.Http;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// 上下文属性
        /// </summary>
        protected WebWorkContext WorkContext {
            get {
                WebIdentity identity = User.Identity as WebIdentity;

                WebWorkContext content = new WebWorkContext();
                content.MerchantId = identity.Id;
                content.Username = identity.Name;
                content.CountryId = identity.CountryId;
                return content;
            }
        }

        /// <summary>
        /// 返回成功数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message">默认null</param>
        /// <returns></returns>
        protected ServiceResult<T> Result_OK<T>(T data, string message = "Successful")
        {
            return new ServiceResult<T>
            {
                Code = 0,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// 返回失败数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ServiceResult<int> Result_Fail(int code,string message)
        {
            return new ServiceResult<int>
            {
                Code = code,
                Message = message
            };
        }

        /// <summary>
        /// 返回失败数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ServiceResult<T> Result_Fail<T>(T data, int code, string message = "Fail")
        {
            return new ServiceResult<T>
            {
                Code = code,
                Data = data,
                Message = message
            };
        }

    }
}
