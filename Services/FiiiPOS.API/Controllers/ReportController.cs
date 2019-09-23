using System.Web.Http;
using FiiiPay.Framework;
using FiiiPOS.API.Extensions;
using FiiiPOS.Business.FiiiPOS;
using FiiiPOS.DTO;

namespace FiiiPOS.API.Controllers
{
    /// <summary>
    /// 报表
    /// </summary>
    [RoutePrefix("api/Report")]
    public class ReportController : ApiController
    {
        /// <summary>
        /// 周报
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("WeeklyTrading")]
        public ServiceResult<TradingReportDTO> WeeklyTrading()
        {
            var result = new ServiceResult<TradingReportDTO>();

            result.Data = new ReportComponent().WeeklyTrading(this.GetMerchantAccountId());

            
            return result;
        }

        /// <summary>
        /// 月报
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MonthlyTrading")]
        public ServiceResult<TradingReportDTO> MonthlyTrading()
        {
            var result = new ServiceResult<TradingReportDTO>();
            result.Data = new ReportComponent().MonthlyTrading(this.GetMerchantAccountId());
            
            return result;
        }

        /// <summary>
        /// 年报
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("YearlyTrading")]
        public ServiceResult<TradingReportDTO> YearlyTrading()
        {
            var result = new ServiceResult<TradingReportDTO>();

            result.Data = new ReportComponent().YearlyTrading(this.GetMerchantAccountId());
            
            return result;
        }
    }
}
