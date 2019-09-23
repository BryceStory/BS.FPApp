using System;

namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.ServiceResultExtension
    /// </summary>
    public static class ServiceResultExtension
    {
        /// <summary>
        /// Successfuls the specified service result.
        /// </summary>
        /// <param name="serviceResult">The service result.</param>
        public static void Success(this ServiceResult serviceResult)
        {
            serviceResult.Message = "Success";
            serviceResult.Code = 0;
        }

        /// <summary>
        /// Successfuls the specified service result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResult">The service result.</param>
        /// <param name="ext">The extension.</param>
        public static void SuccessfulWithExtension<T>(this ServiceResult<T> serviceResult, string ext)
        {
            serviceResult.Message = "Success";
            serviceResult.Code = 0;
            serviceResult.Extension = ext;
        }

        /// <summary>
        /// Failers the specified code.
        /// </summary>
        /// <param name="serviceResult">The service result.</param>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public static void Failer(this ServiceResult serviceResult, int code, string message)
        {
            serviceResult.Code = code;
            serviceResult.Message = message;
        }

        /// <summary>
        /// Systems the error.
        /// </summary>
        /// <param name="serviceResult">The service result.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        public static void SystemError(this ServiceResult serviceResult, string exceptionMessage)
        {
            serviceResult.Code = 10000;
            serviceResult.Message = exceptionMessage;
        }

        /// <summary>
        /// Systems the error.
        /// </summary>
        /// <param name="serviceResult">The service result.</param>
        /// <param name="exception">The exception.</param>
        public static void SystemError(this ServiceResult serviceResult, Exception exception)
        {
            SystemError(serviceResult, exception.Message);
        }
    }
}
