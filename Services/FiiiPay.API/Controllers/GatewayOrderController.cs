using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.GatewayOrder;
using FiiiPay.Framework;
using System;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 网关支付
    /// </summary>
    [RoutePrefix("GatewayOrder")]
    public class GatewayOrderController : ApiController
    {
        /// <summary>
        /// 获取网关订单详情
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<GatewayOrderDetailModel> Detail(GetDetailByIdIM<Guid> im)
        {
            var om = new GatewayOrderComponent().Detail(im.Id, this.GetUser(), this.IsZH());
            return new ServiceResult<GatewayOrderDetailModel>
            {
                Data = new GatewayOrderDetailModel
                {
                    Id=om.Id,
                    Code=om.Code,
                    Timestamp = om.Timestamp,
                    FiatAmount=om.FiatAmount,
                    FiatCurrency = om.FiatCurrency,
                    CryptoAmount = om.CryptoAmount,
                    MerchantName = om.MerchantName,
                    Status = om.Status,
                    MarkUp = om.MarkUp,
                    ExchangeRate = om.ExchangeRate,
                    RefundTimestamp = om.RefundTimestamp,
                    Type = om.Type,
                    OrderNo = om.OrderNo,
                    TradeNo = om.TradeNo,
                    CurrentExchangeRate = om.CurrentExchangeRate,
                    IncreaseRate = om.IncreaseRate
                }
            };
        }
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("PrePay")]
        public ServiceResult<GatewayOrderInfoOM> PrePay(GatewayPrePayIM im)
        {
            return new ServiceResult<GatewayOrderInfoOM>
            {
                Data = new GatewayOrderComponent().PrePay(im.Code, this.GetUser())
            };
        }
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="im">二维码字符串</param>
        /// <returns></returns>
        [HttpPost, Route("Pay")]
        public ServiceResult<GatewayOrderInfoOM> Pay(GatewayPayIM im)
        {
            return new ServiceResult<GatewayOrderInfoOM>
            {
                Data = new GatewayOrderComponent().Pay(im, this.GetUser())
            };
        }

        /// <summary>
        /// 支付成功消息
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, Route("MessagePaymentSuccess")]
        public ServiceResult<MessageGatewayPaymentOM> MessagePaymentSuccess(Guid id)
        {
            return new ServiceResult<MessageGatewayPaymentOM>
            {
                Data = new GatewayOrderComponent().MessagePaymentSuccess(this.GetUser(), id)
            };
        }

        /// <summary>
        /// 退款消息
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, Route("MessagePaymentRefund")]
        public ServiceResult<MessageGatewayPaymentOM> MessagePaymentRefund(Guid id)
        {
            return new ServiceResult<MessageGatewayPaymentOM>
            {
                Data = new GatewayOrderComponent().MessagePaymentRefund(this.GetUser(),id)
            };
        }
    }
}