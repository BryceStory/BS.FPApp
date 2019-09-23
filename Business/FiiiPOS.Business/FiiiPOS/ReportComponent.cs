using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Data;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPOS.DTO;

namespace FiiiPOS.Business.FiiiPOS
{
    public class ReportComponent
    {
        public TradingReportDTO WeeklyTrading(Guid merchantAccountId)
        {
            //if (!new ProfileComponent().ValidateLv1(merchantAccountId))
            //    throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.需要Lv1认证才能使用相关功能);

            var date = DateTime.UtcNow.Date;
            var startDate = date.AddDays(-7);
            var endDate = date.AddDays(-1);

            var account = new MerchantAccountDAC().GetById(merchantAccountId);
            var country = new CountryComponent().GetById(account.CountryId);

            OrderDAC dac = new OrderDAC();
            List<OrderDayStat> list = dac.GetTradingStatInDay(merchantAccountId, startDate, date);
            
            var volume = list.Sum(e => e.OrderCount);
            var sum = list.Sum(e => e.OrderAmount);
            var avg = volume == 0 ? 0 : sum / volume;

            var result = new TradingReportDTO
            {
                FormDate = startDate.ToUnixTime(),
                ToDate = endDate.ToUnixTime(),
                Volume = volume,
                SumAmount = sum.ToString("F"),
                AvgAmount = avg.ToString("F"),
                FiatCurrency = country.FiatCurrency ?? "USD",
                Stats = new List<Stat>()
            };

            var cusorDate = startDate;
            while (cusorDate <= endDate)
            {
                var item = list.FirstOrDefault(e => DateTime.Parse(e.OrderDay) == cusorDate);
                var stat = new Stat
                {
                    Date = cusorDate.ToUnixTime(),
                    Count = item?.OrderCount ?? 0,
                    Amount = item?.OrderAmount ?? 0M
                };
                result.Stats.Add(stat);
                cusorDate = cusorDate.AddDays(1);
            }

            return result;
        }

        public TradingReportDTO MonthlyTrading(Guid merchantAccountId)
        {
            //if (!new ProfileComponent().ValidateLv1(merchantAccountId))
            //    throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.需要Lv1认证才能使用相关功能);

            var date = DateTime.UtcNow.Date;
            var startDate = date.AddDays(-31);
            var endDate = date.AddDays(-1);

            var account = new MerchantAccountDAC().GetById(merchantAccountId);
            var country = new CountryComponent().GetById(account.CountryId);

            OrderDAC dac = new OrderDAC();
            List<OrderDayStat> list = dac.GetTradingStatInDay(merchantAccountId, startDate, date);

            var volume = list.Sum(e => e.OrderCount);
            var sum = list.Sum(e => e.OrderAmount);
            var avg = volume == 0 ? 0 : sum / volume;

            var result = new TradingReportDTO
            {
                FormDate = startDate.ToUnixTime(),
                ToDate = endDate.ToUnixTime(),
                Volume = volume,
                SumAmount = sum.ToString("F"),
                AvgAmount = avg.ToString("F"),
                FiatCurrency = country.FiatCurrency ?? "USD",
                Stats = new List<Stat>()
            };

            var cusorDate = startDate;
            while (cusorDate <= endDate)
            {
                var item = list.FirstOrDefault(e => DateTime.Parse(e.OrderDay) == cusorDate);
                var stat = new Stat
                {
                    Date = cusorDate.ToUnixTime(),
                    Count = item?.OrderCount ?? 0,
                    Amount = item?.OrderAmount ?? 0M
                };
                result.Stats.Add(stat);
                cusorDate = cusorDate.AddDays(1);
            }

            return result;
        }

        public TradingReportDTO YearlyTrading(Guid merchantAccountId)
        {
            //if (!new ProfileComponent().ValidateLv1(merchantAccountId))
            //    throw new CommonException(ReasonCode.NOT_VERIFY_LV1, Resources.需要Lv1认证才能使用相关功能);

            var date = DateTime.UtcNow.Date;

            var endDate = new DateTime(date.Year, date.Month, date.Day - 1);
            var startDate = new DateTime(date.Year, date.Month, 1).AddMonths(-11);
           
            var account = new MerchantAccountDAC().GetById(merchantAccountId);
            var country = new CountryComponent().GetById(account.CountryId);

            OrderDAC dac = new OrderDAC();
            List<OrderMonthStat> list = dac.GetTradingStatInMonth(merchantAccountId, startDate, endDate);

            var volume = list.Sum(e => e.OrderCount);
            var sum = list.Sum(e => e.OrderAmount);
            var avg = volume == 0 ? 0 : sum / volume;

            var result = new TradingReportDTO
            {
                FormDate = startDate.ToUnixTime(),
                ToDate = endDate.ToUnixTime(),
                Volume = volume,
                SumAmount = sum.ToString("F"),
                AvgAmount = avg.ToString("F"),
                FiatCurrency = country.FiatCurrency ?? "USD",
                Stats = new List<Stat>()
            };

            var cusorDate = startDate;
            while (cusorDate <= endDate)
            {
                var item = list.FirstOrDefault(e => DateTime.Parse($"{e.OrderYear}/{e.OrderMonth}") == cusorDate);
                var stat = new Stat
                {
                    Date = cusorDate.ToUnixTime(),
                    Count = item?.OrderCount ?? 0,
                    Amount = item?.OrderAmount ?? 0M
                };
                result.Stats.Add(stat);
                cusorDate = cusorDate.AddMonths(1);
            }

            return result;
        }
    }
}