using System;
using System.Linq;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.API.Models;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 订单
    /// </summary>
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        /// <summary>
        /// 商户创单
        /// </summary>
        /// <param name="model"></param>
        /// <returns>订单token</returns>
        [HttpPost, Route("Create")]
        public ServiceResult<string> Create(OrderCreateModel model)
        {
            var result = new ServiceResult<string>();

            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var orderComponent = new OrderComponent();

            var merchantAccountId = this.GetMerchantAccountId();

            //兼容之前版本，没传入法币，则取商家设置的法币
            if(string.IsNullOrEmpty(model.FiatCurrency))
            {
                var merchant = new MerchantAccountComponent().GetById(merchantAccountId);
                model.FiatCurrency = merchant.FiatCurrency;
            }

            var orderNo = orderComponent.CreateOrder(merchantAccountId,model.FiatCurrency, model.CryptoId, model.Amount, model.PaymentType, model.UserToken, this.GetClientIPAddress());
            result.Data = orderNo;
            
            return result;
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOrderInfo")]
        public ServiceResult<OrderDetailDTO> GetOrderInfo(string orderNo)
        {
            var result = new ServiceResult<OrderDetailDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var orderComponent = new OrderComponent();

            var merchantAccountId = this.GetMerchantAccountId();
            result.Data = orderComponent.GetByOrderNo(merchantAccountId, orderNo);
            
            return result;
        }

        /// <summary>
        /// 根据订单Id获取详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOrderInfo")]
        public ServiceResult<OrderDetailDTO> GetOrderInfo(Guid orderId)
        {
            var result = new ServiceResult<OrderDetailDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var orderComponent = new OrderComponent();

            var merchantAccountId = this.GetMerchantAccountId();
            result.Data = orderComponent.GetById(merchantAccountId, orderId);
            
            return result;
        }

        /// <summary>
        /// 根据订单号获取状态
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOrderStatus")]
        public ServiceResult<OrderStatusDTO> GetOrderStatus(string orderNo)
        {
            var result = new ServiceResult<OrderStatusDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var orderComponent = new OrderComponent();

            var merchantAccountId = this.GetMerchantAccountId();
            result.Data = orderComponent.GetStatusByOrderNo(merchantAccountId, orderNo);
            
            return result;
        }

        /// <summary>
        /// 打印订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet, Route("PrintOrder")]
        public ServiceResult<PrintOrderInfoDTO> PrintOrder(string orderNo)
        {
            var result = new ServiceResult<PrintOrderInfoDTO>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var orderComponent = new OrderComponent();

            var merchantAccountId = this.GetMerchantAccountId();
            result.Data = orderComponent.PrintOrder(merchantAccountId, orderNo);
            
            return result;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Refund")]
        public ServiceResult Refund(OrderRefundModel model)
        {
            var result = new ServiceResult();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var component = new OrderComponent();

            var accountId = this.GetMerchantAccountId();

            component.Refund(accountId, model.OrderNo, model.PINToken);

            
            return result;
        }
    }
}
