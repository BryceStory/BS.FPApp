using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.BackOffice.BLL
{
    public class ChartBLL : BaseBLL
    {
        public List<MemberCountChart> GetMemberCount(int timeType)
        {
            DateTime startTime, endTime;
            GetDateRange(timeType, out startTime, out endTime);

            string sqlUser = "SELECT DateRange,COUNT(1) AS UserCount FROM (";
            sqlUser += $"SELECT CONVERT(VARCHAR({(timeType == 3 ? 7 : 10)}),[RegistrationDate],120) AS DateRange FROM dbo.UserAccounts";
            sqlUser += " WHERE [RegistrationDate]>=@StartTime AND [RegistrationDate]<@EndTime) AS TABLE1";
            sqlUser += " GROUP BY DateRange";

            string sqlMerchant = "SELECT DateRange,COUNT(1) AS MerchantCount FROM (";
            sqlMerchant += $"SELECT CONVERT(VARCHAR({(timeType == 3 ? 7 : 10)}),[RegistrationDate],120) AS DateRange FROM dbo.MerchantAccounts";
            sqlMerchant += " WHERE [RegistrationDate]>=@StartTime AND [RegistrationDate]<@EndTime) AS TABLE1";
            sqlMerchant += " GROUP BY DateRange";

            var userCountList = FiiiPayDB.DB.Ado.SqlQuery<MemberCountChart>(sqlUser, new { StartTime = startTime, EndTime = endTime }).ToList();
            var merchantCountList = FiiiPayDB.DB.Ado.SqlQuery<MemberCountChart>(sqlMerchant, new { StartTime = startTime, EndTime = endTime }).ToList();
            
            List<MemberCountChart> resultList = new List<MemberCountChart>();
            string dateRange;
            MemberCountChart userCountEntity;
            MemberCountChart merchantCountEntity;
            if (timeType == 3)
            {
                for (DateTime dt = startTime; dt < endTime; dt = dt.AddMonths(1))
                {
                    dateRange = dt.ToString("yyyy-MM");
                    userCountEntity = userCountList.Find(t => t.DateRange == dateRange);
                    merchantCountEntity = merchantCountList.Find(t => t.DateRange == dateRange);
                    resultList.Add(new MemberCountChart()
                    {
                        DateRange = dateRange,
                        UserCount = userCountEntity == null ? 0 : userCountEntity.UserCount,
                        MerchantCount = merchantCountEntity == null ? 0 : merchantCountEntity.MerchantCount
                    });
                }
            }
            else
            {
                for (DateTime dt = startTime; dt < endTime; dt = dt.AddDays(1))
                {
                    dateRange = dt.ToString("yyyy-MM-dd");
                    userCountEntity = userCountList.Find(t => t.DateRange == dateRange);
                    merchantCountEntity = merchantCountList.Find(t => t.DateRange == dateRange);
                    resultList.Add(new MemberCountChart()
                    {
                        DateRange = dateRange,
                        UserCount = userCountEntity == null ? 0 : userCountEntity.UserCount,
                        MerchantCount = merchantCountEntity == null ? 0 : merchantCountEntity.MerchantCount
                    });
                }
            }
            
            return resultList;
        }

        public List<ConsumeCountChart> GetConsumeCount(int timeType)
        {
            DateTime startTime, endTime;
            GetDateRange(timeType, out startTime, out endTime);

            string sql = "SELECT DateRange,COUNT(1) AS ConsumeCount FROM (";
            sql += "SELECT CONVERT(VARCHAR(10),CASE WHEN t1.[Status]=2 THEN t1.PaymentTime WHEN t1.[Status]=3 THEN t2.Timestamp ELSE t1.PaymentTime END,120) AS DateRange FROM dbo.Orders t1";
            sql += " LEFT JOIN dbo.Refunds t2 ON t2.OrderId=t1.Id";
            sql += " WHERE t1.[Status]>1) AS TABLE1";
            sql += " GROUP BY DateRange";

            var consumeCountList = FiiiPayDB.DB.Ado.SqlQuery<ConsumeCountChart>(sql, new { StartTime = startTime, EndTime = endTime }).ToList();

            List<ConsumeCountChart> resultList = new List<ConsumeCountChart>();
            string dateRange;
            ConsumeCountChart entity;
            if (timeType == 3)
            {
                for (DateTime dt = startTime; dt < endTime; dt = dt.AddMonths(1))
                {
                    dateRange = dt.ToString("yyyy-MM");
                    entity = consumeCountList.Find(t => t.DateRange == dateRange);
                    if (entity == null)
                    {
                        resultList.Add(new ConsumeCountChart()
                        {
                            DateRange = dateRange,
                            ConsumeCount = 0
                        });
                    }
                    else
                    {
                        resultList.Add(entity);
                    }
                }
            }
            else
            {
                for (DateTime dt = startTime; dt < endTime; dt = dt.AddDays(1))
                {
                    dateRange = dt.ToString("yyyy-MM-dd");
                    entity = consumeCountList.Find(t => t.DateRange == dateRange);
                    if (entity == null)
                    {
                        resultList.Add(new ConsumeCountChart()
                        {
                            DateRange = dateRange,
                            ConsumeCount = 0
                        });
                    }
                    else
                    {
                        resultList.Add(entity);
                    }
                }
            }

            return resultList;
        }

        private void GetDateRange(int timeType,out DateTime startTime,out DateTime endTime)
        {
            var dt = DateTime.UtcNow;
            switch (timeType)
            {
                case 1://周
                    endTime = new DateTime(dt.Year,dt.Month,dt.Day);
                    startTime = endTime.AddDays(-7);
                    break;
                case 2://月
                    endTime = new DateTime(dt.Year, dt.Month, dt.Day);
                    startTime = endTime.AddDays(-30);
                    break;
                case 3://年
                    endTime = new DateTime(dt.Year, dt.Month, 1);
                    startTime = endTime.AddMonths(-12);
                    break;
                default:
                    startTime = dt;
                    endTime = dt;
                    break;
            }
        }
    }
}