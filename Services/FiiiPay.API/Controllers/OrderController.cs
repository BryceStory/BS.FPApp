using System;
using System.Threading.Tasks;
using System.Web.Http;
using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Order;
using FiiiPay.Framework;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// Class FiiiPay.API.Controllers.OrderController
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("Order")]
    public class OrderController : ApiController
    {
        /// <summary>
        /// 准备支付推送过来的订单，通过订单号获取订单金额等信息
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("PrePayExistedOrder")]
        public ServiceResult<PrePayExistedOrderOM> PrePayExistedOrder(PrePayExistedOrderIM im)
        {
            return new ServiceResult<PrePayExistedOrderOM>
            {
                Data = new OrderComponent().PrePayExistedOrder(this.GetUser(), im)
            };
        }

        /// <summary>
        /// 支付已经存在的订单
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("PayExistedOrder")]
        public ServiceResult<PayOrderOM> PayExistedOrder(PayOrderIM im)
        {
            return new ServiceResult<PayOrderOM>
            {
                Data = new OrderComponent().PayExistedOrder(this.GetUser(), im.OrderNo, im.Pin)
            };
        }

        /// <summary>
        /// 主动支付（蓝牙、二维码、NFC都走这个流程） - 准备支付，将会传递溢价等参数
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("PrePay")]
        public ServiceResult<PrePayOM> PrePay(PrePayIM im)
        {
            return new ServiceResult<PrePayOM>
            {
                Data = new OrderComponent().PrePay(this.GetUser(), im)
            };
        }

        /// <summary>
        /// 扫描商家固态二维码
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("ScanMerchantQRCode")]
        public async Task<ServiceResult<ScanMerchantQRCodeModel>> ScanMerchantQRCode(ScanQRCodeIM im)
        {
            ScanMerchantQRCodeOM om = await new OrderComponent().ScanMerchantQRCode(this.GetUser(), im.Code);
            ScanMerchantQRCodeModel result = new ScanMerchantQRCodeModel();

            result.MerchantId = om.MerchantId;
            result.MerchantName = om.MerchantName;
            result.Avatar = om.Avatar;
            result.L1VerifyStatus = om.L1VerifyStatus;
            result.L2VerifyStatus = om.L2VerifyStatus;
            result.FiatCurrency = om.FiatCurrency;
            result.MarkupRate = om.MarkupRate;
            if (om.WaletInfoList != null)
            {
                result.WaletInfoList = new System.Collections.Generic.List<WalletInfo>();
                foreach (var item in om.WaletInfoList)
                {
                    result.WaletInfoList.Add(new WalletInfo
                    {
                        Id = item.Id,
                        IconUrl = item.IconUrl,
                        NewStatus = item.NewStatus,
                        Code = item.Code,
                        Name = item.Name,
                        UseableBalance = item.UseableBalance,
                        FrozenBalance = item.FrozenBalance,
                        ExchangeRate = item.ExchangeRate,
                        FiatBalance = item.FiatBalance,
                        MerchantSupported = item.MerchantSupported,
                        DecimalPlace = item.DecimalPlace,
                        CryptoEnable = item.CryptoEnable
                    });
                }
            }

            return new ServiceResult<ScanMerchantQRCodeModel>
            {
                Data = result
            };
        }

        /// <summary>
        /// 主动支付（蓝牙、二维码、NFC都走这个流程）
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Pay")]
        public ServiceResult<PayOrderOM> Pay(PayIM im)
        {
            return new ServiceResult<PayOrderOM>
            {
                Data = new OrderComponent().Pay(this.GetUser(), im)
            };
        }

        /// <summary>
        /// 获取订单详情（用于支付完成后）
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost, Route("Detail")]
        public ServiceResult<OrderDetailOM> Detail(GetDetailByIdIM<Guid> im)
        {
            return new ServiceResult<OrderDetailOM>
            {
                Data = new OrderComponent().Detail(this.GetUser(), im.Id, this.IsZH())
            };
        }

        /// <summary>
        /// 获取支付码
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetPaymentCode")]
        public ServiceResult<PaymentCodeDTO> GetPaymentCode()
        {
            return new ServiceResult<PaymentCodeDTO>
            {
                Data = new OrderComponent().GetPaymentCode(this.GetUser())
            };
        }
    }
}
