using FiiiPay.Framework;
using FiiiPay.Profile.API.Models;

namespace FiiiPay.Profile.API.Common
{
    internal class ResultHelper
    {
        public static ServiceResult<T> OKResult<T>(T data, int code = 0, string msg = null)
        {
            var resultData = new ServiceResult<T>();
            resultData.Data = data;
            resultData.Code = code;
            resultData.Message = msg;
            return resultData;
        }

        public static ServiceResult<T> FailResult<T>(string message, int code = -1)
        {
            var resultData = new ServiceResult<T>();
            resultData.Message = message;
            resultData.Code = code;
            return resultData;
        }
    }
}