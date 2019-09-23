using System;
using System.Linq;
using System.Web.Http;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Queue;
using FiiiPay.ShopPayment.API.Components;
using log4net;
using FiiiPay.ShopPayment.API.Models;

namespace FiiiPay.ShopPayment.API.Controllers
{
    /// <summary>
    /// Class Order
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Order")]
    public class OrderController : ApiController
    {
        private const int Param_Error = 10000;
        
        /// <summary>
        /// 支付。
        /// 10001 帐号不存在,
        /// 10002 钱包数据无效,
        /// 10003 余额不足,
        /// 10004 未知的系统异常,
        /// 10005 订单已支付,
        /// 10006 PIN错误
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("Payment")]
        [HttpPost]
        public ResultDto Payment(PaymentVo model)
        {
            var result = new ResultDto();

            if (!ModelState.IsValid)
            {
                result.Code = Param_Error;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            return new OrderComponent().Payment(model);
        }

        /// <summary>
        /// 退款。
        /// 10011 退款金额与订单金额不符,
        /// 10012 订单已退款,
        /// 10013 订单正在支付中
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [Route("Refund")]
        [HttpPost]
        public ResultDto Refund(RefundVo model)
        {
            var result = new ResultDto();

            if (!ModelState.IsValid)
            {
                result.Code = Param_Error;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            return new OrderComponent().Refund(model);
        }
    }
}
