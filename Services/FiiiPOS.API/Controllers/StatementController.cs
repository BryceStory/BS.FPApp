using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.DTO;
using Newtonsoft.Json;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 各种记录api
    /// </summary>
    [RoutePrefix("api/Statement")]
    public class StatementController : ApiController
    {
        /// <summary>
        /// 收款记录
        /// </summary>
        /// <param name="startTime">时间段start</param>
        /// <param name="endTime">时间段end</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("ReceiptStatement")]
        public ServiceResult<List<OrderDTO>> ReceiptStatement(string startTime,string endTime, int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<OrderDTO>>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();

            if (!DateTime.TryParse(startTime, out DateTime dtStart))
            {
                result.Failer(ReasonCode.MISSING_REQUIRED_FIELDS, "");
                return result;
            }
            if (!DateTime.TryParse(endTime, out DateTime dtEnd))
            {
                result.Failer(ReasonCode.MISSING_REQUIRED_FIELDS, "");
                return result;
            }

            result.Data = component.ReceiptStatement(accountId, dtStart, dtEnd, pageIndex, pageSize);
            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize= pageSize, PageCount = result.Data.Count }));
            return result;
        }
        /// <summary>
        /// 搜索记录
        /// </summary>
        /// <param name="orderno">订单号</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("SearchReceiptStatement")]
        public ServiceResult<List<OrderDTO>> SearchReceiptStatement(string orderno, int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<OrderDTO>>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }

            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();

            result.Data = component.SearchReceiptStatement(accountId, orderno, pageIndex, pageSize);
            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, PageCount = result.Data.Count }));
            return result;
        }

        /// <summary>
        /// 当日实收总额
        /// </summary>
        /// <param name="startTime">时间段start</param>
        /// <param name="endTime">时间段end</param>
        /// <returns></returns>
        [HttpGet, Route("ActualReceipt")]
        public ServiceResult<string> ActualReceipt(string startTime, string endTime)
        {
            var result = new ServiceResult<string>();
            if (!ModelState.IsValid)
            {
                result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
                foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
                    result.Message += error + Environment.NewLine;

                return result;
            }
            if (!DateTime.TryParse(startTime, out DateTime dtStart))
            {
                result.Failer(ReasonCode.MISSING_REQUIRED_FIELDS, "");
                return result;
            }
            if (!DateTime.TryParse(endTime, out DateTime dtEnd))
            {
                result.Failer(ReasonCode.MISSING_REQUIRED_FIELDS, "");
                return result;
            }

            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();
            result.Data = component.ActualReceipt(accountId, dtStart, dtEnd);
            
            return result;
        }

        /// <summary>
        /// 当日实收总额
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //[HttpGet, Route("TotalReceiptCount")]
        //public ServiceResult<int> TotalReceiptCount(string date)
        //{
        //    var result = new ServiceResult<int>();
        //    if (!ModelState.IsValid)
        //    {
        //        result.Code = ReasonCode.MISSING_REQUIRED_FIELDS;
        //        foreach (string error in ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)))
        //            result.Message += error + Environment.NewLine;

        //        return result;
        //    }
        //    if (!DateTime.TryParse(date, out DateTime dt))
        //    {
        //        result.Failer(ReasonCode.MISSING_REQUIRED_FIELDS, "");
        //        return result;
        //    }

        //    var accountId = this.GetMerchantAccountId();

        //    var component = new StatementComponent();
        //    result.Data = component.TotalReceiptCount(accountId, dt);
        //    result.Success();
        //    return result;
        //}

        /// <summary>
        /// 退款记录
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("RefundStatement")]
        public ServiceResult<List<OrderDTO>> RefundStatement(int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<OrderDTO>>();

            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();
            result.Data = component.RefundStatement(accountId, pageIndex, pageSize);

            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, PageCount = result.Data.Count }));
            return result;
        }

        /// <summary>
        /// 提币记录
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("WithdrawalStatement")]
        public ServiceResult<List<MerchantWithdrawalDTO>> WithdrawalStatement(int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<MerchantWithdrawalDTO>>();
            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();
            result.Data = component.WithdrawalStatement(accountId, pageIndex, pageSize);
            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, PageCount = result.Data.Count }));
            return result;
        }

        /// <summary>
        /// 充币记录
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("DepositStatement")]
        public ServiceResult<List<MerchantDepositDTO>> DepositStatement(int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<MerchantDepositDTO>>();
            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();
            result.Data = component.DepositStatement(accountId, pageIndex, pageSize);
            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, PageCount = result.Data.Count }));
            return result;
        }

        /// <summary>
        /// 钱包提现/充值记录
        /// </summary>
        /// <param name="walletId">钱包ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("TransferStatement")]
        public ServiceResult<List<MerchantTransferDTO>> TransferStatement(long walletId, int pageIndex = 1, int pageSize = 20)
        {
            var result = new ServiceResult<List<MerchantTransferDTO>>();
            var component = new StatementComponent();
            result.Data = component.TransferStatement(this.GetMerchantAccountId(), walletId, pageIndex, pageSize);
            result.SuccessfulWithExtension(JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, PageCount = result.Data.Count }));
            return result;
        }

        /// <summary>
        /// 提币详情
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("WithdrawalDetail")]
        public ServiceResult<MerchantWithdrawalDTO> WithdrawalDetail(long withdrawalId)
        {
            var result = new ServiceResult<MerchantWithdrawalDTO>();


            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();
            result.Data = component.WithdrawalDetail(accountId, withdrawalId);
            
            return result;
        }

        /// <summary>
        /// 充币详情
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("DepositDetail")]
        public ServiceResult<MerchantDepositDTO> DepositDetail(long depositId)
        {
            var result = new ServiceResult<MerchantDepositDTO>();

            var accountId = this.GetMerchantAccountId();

            var component = new StatementComponent();
            result.Data = component.DepositDetail(accountId, depositId);
            
            return result;
        }

    }
}
