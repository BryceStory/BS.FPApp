using FiiiPay.Framework;
using FiiiPOS.Web.API.Base;
using FiiiPOS.Web.API.Models.Input;
using FiiiPOS.Web.API.Models.Output;
using FiiiPOS.Web.Business;
using System;
using System.Collections.Generic;
using System.Web.Http;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Foundation.Business;
using System.Linq;

namespace FiiiPOS.Web.API.Controllers
{
    /// <summary>
    /// 订单相关接口
    /// </summary>
    [RoutePrefix("api/Order")]
    public class OrderController : BaseApiController
    {
        /// <summary>
        /// 获取收款记录列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("GetOrderRecordList")]
        public ServiceResult<DataListWithPageResultModel<OrderRecordListOutModel>> GetOrderRecordList(OrderRecordListInModel model)
        {
            DataListWithPageResultModel<OrderRecordListOutModel> outputModel = new DataListWithPageResultModel<OrderRecordListOutModel>();

            int count;
            List<OrderByPage> list = new OrderComponent().GetOrderRecordList(WorkContext.MerchantId,  model.OrderNo, model.States, model.StartDate, model.EndDate, model.PageIndex, model.PageSize, out count);
            var cryptoList = new CryptoComponent().GetList();
            List<OrderRecordListOutModel> outputList = new List<OrderRecordListOutModel>();
            foreach (OrderByPage order in list)
            {
                outputList.Add(new OrderRecordListOutModel
                {
                    Id = order.Id,
                    OrderNo = order.OrderNo,
                    PostSN = order.PostSN,
                    Cellphone = order.Cellphone,
                    FiatCurrency = order.FiatCurrency,
                    FiatAmount = order.FiatAmount,
                    Markup = order.Markup,
                    ActualCryptoAmount = order.ActualCryptoAmount,
                    CryptoName = order.CryptoName,
                    ExchangeRate = order.ExchangeRate,
                    CryptoAmount = order.CryptoAmount,
                    TransactionFee = order.TransactionFee.ToString(),
                    ActualFiatAmount = order.ActualFiatAmount,
                    Status = order.Status,
                    Timestamp = order.Timestamp.ToLocalTime().ToUnixTime(),
                    CurrentExchangeRate = order.CurrentExchangeRate,
                    IncreaseRate = order.IncreaseRate,
                    WithdrawalFee = order.WithdrawalFee,
                    WithdrawalCryptoCode = cryptoList.Where(t => t.Id == order.WithdrawalCryptoId).Select(t =>t.Code).FirstOrDefault()
                });
            }

            outputModel.TotalCount = count;
            outputModel.DataList = outputList;

            return Result_OK(outputModel);
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="model"></param>
        /// <returns>-1=账号不存在 -2=PIN码错误 -3=订单不存在 -4=商家账户不对 -5=订单状态不符合要求 -6=商家不支持的币种 -7=商家金额不够 -8=用户钱包不支持币种 -9=退款失败 -10=日期超过3天</returns>
        [HttpPost, Route("RefundOrder")]
        public ServiceResult<int> RefundOrder(RefundOrderInModel model)
        {

            if (string.IsNullOrEmpty(model.OrderNo))
                return Result_Fail(-11, "订单Id不存在");
            ;
            //验证pin码、退款
            int result = new OrderComponent().RefundOrder(WorkContext.MerchantId, model.OrderNo, model.PinPassword);

            if (result > 0)
                return Result_OK(result);

            return Result_Fail(result, "退款失败");

        }

        /// <summary>
        /// 统计周返回日统计
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetOrderStatInWeek")]
        public ServiceResult<List<OrderDayStatDataOutModel>> GetOrderStatInWeek()
        {
            DateTime date = DateTime.UtcNow;
            DateTime today = date.AddDays(-1).Date;
            DateTime fromday = date.AddDays(-7).Date;
            var component = new OrderComponent();
            List<OrderDayStat> list = component.GetOrderStatInDay(WorkContext.MerchantId, fromday, today);
            var fiatCurrency = new MerchantComponent().GetMerchantAccount(WorkContext.MerchantId).FiatCurrency;

            List<OrderDayStatDataOutModel> ouputList = new List<OrderDayStatDataOutModel>();
            for (int i = 7; i > 0; i--)
            {
                bool bFlag = false;
                string OrderDay = date.AddDays(-i).ToString("yyyy-MM-dd");
                foreach (OrderDayStat day in list)
                {
                    if (OrderDay.Equals(day.OrderDay))
                    {
                        ouputList.Add(new OrderDayStatDataOutModel()
                        {

                            OrderDay = day.OrderDay,
                            OrderCount = day.OrderCount,
                            OrderAmount = Convert.ToDecimal(component.DayActualReceipt(WorkContext.MerchantId, date.AddDays(-i))),
                            FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
                        });

                        bFlag = true;
                        break;
                    }
                }
                if (!bFlag)
                {//没有数据时
                    ouputList.Add(new OrderDayStatDataOutModel()
                    {
                        OrderDay = OrderDay,
                        OrderCount = 0,
                        OrderAmount = 0,
                        FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
                    });
                }
            }

            return Result_OK(ouputList);
        }

        /// <summary>
        /// 统计月返回日统计
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetOrderStatInMonth")]
        public ServiceResult<List<OrderDayStatDataOutModel>> GetOrderStatInMonth()
        {
            DateTime date = DateTime.UtcNow;
            DateTime today = date.AddDays(-1).Date;
            DateTime fromday = date.AddDays(-31).Date;
            var component = new OrderComponent();
            List<OrderDayStat> list = component.GetOrderStatInDay(WorkContext.MerchantId, fromday, today);
            var fiatCurrency = new MerchantComponent().GetMerchantAccount(WorkContext.MerchantId).FiatCurrency;

            List<OrderDayStatDataOutModel> ouputList = new List<OrderDayStatDataOutModel>();
            for (int i = 31; i > 0; i--)
            {
                bool bFlag = false;
                string OrderDay = date.AddDays(-i).ToString("yyyy-MM-dd");
                foreach (OrderDayStat day in list)
                {
                    if (OrderDay.Equals(day.OrderDay))
                    {
                        ouputList.Add(new OrderDayStatDataOutModel()
                        {

                            OrderDay = day.OrderDay,
                            OrderCount = day.OrderCount,
                            OrderAmount = Convert.ToDecimal(component.DayActualReceipt(WorkContext.MerchantId, date.AddDays(-i))),
                            FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
                        });

                        bFlag = true;
                        break;
                    }
                }
                if (!bFlag)
                {//没有数据时
                    ouputList.Add(new OrderDayStatDataOutModel()
                    {
                        OrderDay = OrderDay,
                        OrderCount = 0,
                        OrderAmount = 0,
                        FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
                    });
                }
            }
            return Result_OK(ouputList);
        }

        /// <summary>
        /// 统计年返回月份统计
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetOrderStatInYear")]
        public ServiceResult<List<OrderDayStatDataOutModel>> GetOrderStatInYear()
        {

            DateTime dt = DateTime.UtcNow;
            DateTime today = dt.AddDays(1 - dt.Day).Date; //2017/12/1 0:00:00
            DateTime fromday = today.AddMonths(-12).Date; //2016/12/1 0:00:00
            var component = new OrderComponent();

            List<OrderMonthStat> list = component.GetOrderStatInMonth(WorkContext.MerchantId, fromday, today);
            var fiatCurrency = new MerchantComponent().GetMerchantAccount(WorkContext.MerchantId).FiatCurrency;

            List<OrderDayStatDataOutModel> ouputList = new List<OrderDayStatDataOutModel>();
            for (int i = 12; i > 0; i--)
            {
                bool bFlag = false;
                int OrderYear = today.AddMonths(-i).Year;
                int OrderMonth = today.AddMonths(-i).Month;
                foreach (OrderMonthStat month in list)
                {
                    if (OrderYear == month.OrderYear && OrderMonth == month.OrderMonth)
                    {
                        ouputList.Add(new OrderDayStatDataOutModel()
                        {
                            OrderDay = OrderYear.ToString() + "-" + OrderMonth.ToString().PadLeft(2, '0'),
                            OrderCount = month.OrderCount,
                            OrderAmount = Convert.ToDecimal(component.MonthActualReceipt(WorkContext.MerchantId, today.AddMonths(-i))),
                            FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
                        });

                        bFlag = true;
                        break;
                    }
                }
                if (!bFlag)
                {
                    ouputList.Add(new OrderDayStatDataOutModel()
                    {
                        OrderDay = OrderYear.ToString() + "-" + OrderMonth.ToString().PadLeft(2, '0'),
                        OrderCount = 0,
                        OrderAmount = 0,
                        FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
                    });
                }
            }
            return Result_OK(ouputList);
        }


        /// <summary>
        /// 获取订单统计
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("GetOrderStat")]
        public ServiceResult<OrderStatOutModel> GetOrderStat()
        {
            var component = new OrderComponent();
            var fiatCurrency = new MerchantComponent().GetMerchantAccount(WorkContext.MerchantId).FiatCurrency;
            OrderStat stat = component.GetOrderStat(WorkContext.MerchantId, 2);
            OrderStatOutModel outputModel = new OrderStatOutModel()
            {
                TotalCount = stat.TotalCount,
                TotalMoney = component.AllActualReceipt(WorkContext.MerchantId),
                TodayCount = stat.TodayCount,
                TodayMoney = Convert.ToDecimal(component.DayActualReceipt(WorkContext.MerchantId, DateTime.Today)),
                FiatCurrency = string.IsNullOrWhiteSpace(fiatCurrency) ? "" : fiatCurrency
            };
            return Result_OK(outputModel);
        }

    }
}
