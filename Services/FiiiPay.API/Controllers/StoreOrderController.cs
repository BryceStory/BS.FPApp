using FiiiPay.API.Models;
using FiiiPay.Business;
using FiiiPay.DTO;
using FiiiPay.DTO.Order;
using FiiiPay.Framework;
using System.Threading.Tasks;
using System.Web.Http;

namespace FiiiPay.API.Controllers
{
    /// <summary>
    /// 门店订单
    /// </summary>
    [RoutePrefix("StoreOrder")]
    public class StoreOrderController : ApiController
    {
        /// <summary>
        /// 获取门店支付相关信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("PrePay")]
        public ServiceResult<PrePayOM> PrePay(PreStorePayModel model)
        {
            return new ServiceResult<PrePayOM>
            {
                Data = new StoreOrderComponent().PrePay(this.GetUser().Id, model.MerchantInfoId)
            };
        }

        /// <summary>
        /// 支付门店订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("Pay")]
        public async Task<ServiceResult<PayOrderOM>> Pay(StoreOrderPayModel model)
        {
            return new ServiceResult<PayOrderOM>
            {
                Data = await new StoreOrderComponent().PayAsync(this.GetUser(), new DTO.StoreOrderPayIM
                {
                    FiatAmount = model.FiatAmount,
                    CoinId = model.CoinId,
                    MerchantInfoId = model.MerchantInfoId,
                    Pin = model.Pin
                })
            };
        }

        /// <summary>
        /// 门店收入详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("IncomeDetail")]
        public async Task<ServiceResult<StoreIncomeDetail>> IncomeDetail(GetByGuidModel model)
        {
            return new ServiceResult<StoreIncomeDetail>
            {
                Data = await new StoreOrderComponent().GetIncomeDetailAsync(this.GetUser().Id, model.Id, this.IsZH())
            };
        }

        /// <summary>
        /// 门店消费详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("ConsumeDetail")]
        public async Task<ServiceResult<StoreConsumeDetail>> ConsumeDetail(GetByGuidModel model)
        {
            return new ServiceResult<StoreConsumeDetail>
            {
                Data = await new StoreOrderComponent().GetConsumeDetailAsync(this.GetUser().Id, model.Id, this.IsZH())
            };
        }
    }
}
