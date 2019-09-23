using System;
using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Business;
using FiiiPay.DTO.Investor;
using FiiiPay.FiiiCoinWork.API.Extensions;
using FiiiPay.FiiiCoinWork.API.Models;
using FiiiPay.Framework;
using Newtonsoft.Json;

namespace FiiiPay.FiiiCoinWork.API.Controllers
{
    /// <summary>
    /// 账单相关
    /// </summary>
    [RoutePrefix("api/Statement")]
    public class StatementController : ApiController
    {
        /// <summary>
        /// 账单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("List")]
        public ServiceResult<List<StatementDTO>> List(QueryStatementModel model)
        {
            ServiceResult<List<StatementDTO>> result = new ServiceResult<List<StatementDTO>>();
            InvestorOrderComponent component = new InvestorOrderComponent();
            result.Data = component.List(
                this.GetAccountId(),
                model.StartTimestamp,
                model.EndTimestamp,
                model.TransactionType,
                model.Cellphone,
                model.PageIndex,
                model.PageSize);

            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { model.PageIndex, model.PageSize }));
            return result;
        }

        /// <summary>
        /// 账单详情（获取充币，扣币详情）
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("StatementDetail")]
        public ServiceResult<StatementDetailDTO> StatementDetail(Guid statementId)
        {
            ServiceResult<StatementDetailDTO> result = new ServiceResult<StatementDetailDTO>();
            InvestorOrderComponent component = new InvestorOrderComponent();
            result.Data = component.StatementDetail(this.GetAccountId(), statementId);
            result.Successful();
            return result;
        }

        /// <summary>
        /// 订单详情（获取转账订单详情）
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("OrderDetail")]
        public ServiceResult<OrderDetailDTO> OrderDetail(Guid orderId)
        {
            ServiceResult<OrderDetailDTO> result = new ServiceResult<OrderDetailDTO>();
            InvestorOrderComponent component = new InvestorOrderComponent();
            result.Data = component.OrderDetail(this.GetAccountId(), orderId);
            result.Successful();
            return result;
        }
    }
}
