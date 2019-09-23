using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;

namespace FiiiPay.Data
{
    public class OrderDAC : BaseDataAccess
    {
        public Order Create(Order order)
        {
            const string sql =
            @"INSERT INTO [dbo].[Orders]([Id],[OrderNo],[MerchantAccountId],[UserAccountId],[CryptoId],[CryptoCode],[CryptoAmount],[ActualCryptoAmount],[ActualFiatAmount],[FiatAmount],[FiatCurrency],[Status],[Timestamp],[Remark],[ExchangeRate],[Markup],[ExpiredTime],[TransactionFee],[MerchantIP],[MerchantToken],[UserIP],[UserToken],[PaymentType],[PaymentTime],[UnifiedFiatAmount],[UnifiedFiatCurrency],[UnifiedExchangeRate],[UnifiedActualFiatAmount])
                       VALUES (@Id,@OrderNo,@MerchantAccountId,@UserAccountId,@CryptoId,@CryptoCode,@CryptoAmount,@ActualCryptoAmount,@ActualFiatAmount,@FiatAmount,@FiatCurrency,@Status,@Timestamp,@Remark,@ExchangeRate,@Markup,@ExpiredTime,@TransactionFee,@MerchantIP,@MerchantToken,@UserIP,@UserToken,@PaymentType,@PaymentTime,@UnifiedFiatAmount,@UnifiedFiatCurrency,@UnifiedExchangeRate,@UnifiedActualFiatAmount)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, order);
            }

            return order;
        }

        public void Update(Order model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE Orders SET OrderNo=@OrderNo,MerchantAccountId=@MerchantAccountId,UserAccountId=@UserAccountId,CryptoId=@CryptoId,CryptoCode=@CryptoCode,CryptoAmount=@CryptoAmount,ActualCryptoAmount=@ActualCryptoAmount,ActualFiatAmount=@ActualFiatAmount,FiatAmount=@FiatAmount,FiatCurrency=@FiatCurrency,Status=@Status,Timestamp=@Timestamp,Remark=@Remark,ExchangeRate=@ExchangeRate,Markup=@Markup,TransactionFee=@TransactionFee,MerchantIP=@MerchantIP,MerchantToken=@MerchantToken,UserIP=@UserIP,UserToken=@UserToken,PaymentType=@PaymentType,ExpiredTime=@ExpiredTime,PaymentTime=@PaymentTime WHERE Id=@Id", model);
            }
        }

        public Order GetByOrderNo(string orderNo)
        {
            const string sql = @"SELECT * FROM Orders WHERE OrderNo=@OrderNo";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Order>(sql, new { OrderNo = orderNo });
            }
        }

        public Order GetById(Guid orderId)
        {
            const string sql = @"SELECT * FROM Orders WHERE Id=@Id";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Order>(sql, new { Id = orderId });
            }
        }

        public void UpdateStatus(Order order)
        {
            const string sql = @"UPDATE [dbo].[Orders]
                                    SET [Status] = @Status
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    order.Id,
                    order.Status,
                    order.Timestamp
                });
            }
        }


        public List<Order> GetMerchantOrderByDate(Guid merchantAccountId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            const string sql =
                @"SELECT * FROM [dbo].[Orders]
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]>@Status AND [Timestamp]>=@StartTime AND [Timestamp]<=@EndTime
                ORDER BY [Timestamp] DESC 
                  OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var con = ReadConnection())
            {
                var list = con.Query<Order>(sql, new
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    MerchantAccountId = merchantAccountId,
                    Status = OrderStatus.Pending,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();

                return list;
            }
        }


        public List<Order> GetMerchantOrderByOrderNo(Guid accountId, string orderno, int pageIndex, int pageSize)
        {
            const string sql =
                @"SELECT * FROM [dbo].[Orders]
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]>@Status AND OrderNo LIKE @OrderNo
                ORDER BY [Timestamp] DESC
                  OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var con = ReadConnection())
            {
                var list = con.Query<Order>(sql, new
                {
                    OrderNo = $"%{orderno}%",
                    MerchantAccountId = accountId,
                    Status = OrderStatus.Pending,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();

                return list;
            }
        }


        public decimal DayOfActualReceipt(Guid accountId, DateTime startTime, DateTime endTime)
        {
            const string sql =
                @"SELECT SUM([UnifiedFiatAmount])
                    FROM [dbo].[Orders] a
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]=@Status AND [Timestamp]>=@StartTime AND [Timestamp]<=@EndTime";
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<decimal>(sql, new { StartTime = startTime, EndTime = endTime, MerchantAccountId = accountId, Status = OrderStatus.Completed });
            }
        }

        public decimal DayOfOrderAmount(Guid accountId, DateTime date)
        {
            const string sql =
                @"SELECT SUM([UnifiedFiatAmount])
                    FROM [dbo].[Orders] a
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]=@Status AND DateDiff(dd,[Timestamp],@Date)=0";

            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<decimal>(sql, new { Date = date, MerchantAccountId = accountId, Status = OrderStatus.Completed });
            }
        }

        public decimal MonthOfOrderAmount(Guid accountId, DateTime date)
        {
            const string sql =
                @"SELECT SUM([UnifiedFiatAmount])
                    FROM [dbo].[Orders] a
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]=@Status AND DateDiff(mm,[Timestamp],@Date)=0";

            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<decimal>(sql, new { Date = date, MerchantAccountId = accountId, Status = OrderStatus.Completed });
            }
        }

        public decimal DateOfOrderAmount(Guid accountId, DateTime startDate, DateTime endDate)
        {
            const string sql =
                @"SELECT SUM([UnifiedFiatAmount])
                    FROM [dbo].[Orders] a
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]=@Status AND Timestamp BETWEEN @StartDate AND @EndDate";

            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<decimal>(sql, new { StartDate = startDate, EndDate = endDate, MerchantAccountId = accountId, Status = OrderStatus.Completed });
            }
        }

        public decimal ALLOfOrderAmount(Guid accountId)
        {
            const string sql =
                @"SELECT SUM([UnifiedFiatAmount])
                    FROM [dbo].[Orders] a
                   WHERE [MerchantAccountId]=@MerchantAccountId AND [Status]=@Status";

            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<decimal>(sql, new { MerchantAccountId = accountId, Status = OrderStatus.Completed });
            }
        }

        public List<Order> GetMerchantRefundOrder(Guid accountId, int pageIndex, int pageSize)
        {
            const string sql =
                @"SELECT a.* FROM [dbo].[Orders]  a
                    JOIN Refunds b ON a.Id=b.OrderId
                   WHERE a.[MerchantAccountId]=@MerchantAccountId AND a.[Status]=@Status 
                ORDER BY b.[Timestamp] DESC
                  OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var con = ReadConnection())
            {
                var list = con.Query<Order>(sql, new
                {
                    MerchantAccountId = accountId,
                    Status = OrderStatus.Refunded,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();
                return list;
            }
        }

        public List<OrderDayStat> GetTradingStatInDay(Guid merchantId, DateTime startDate, DateTime endDate)
        {
            string strSql = @"SELECT CONVERT(varchar(100), [Timestamp], 23) as OrderDay,
                              COUNT(MerchantAccountId) as OrderCount,
                              SUM([UnifiedFiatAmount]) as OrderAmount 
                              FROM [dbo].[Orders]  
                              WHERE [MerchantAccountId]=@MerchantAccountId AND [Timestamp] BETWEEN @StartDate AND @EndDate AND [Status] = @Status 
                              GROUP BY CONVERT(varchar(100), [Timestamp], 23)";

            using (var con = ReadConnection())
            {
                List<OrderDayStat> list = con.Query<OrderDayStat>(strSql, new { MerchantAccountId = merchantId, StartDate = startDate, EndDate = endDate, Status = OrderStatus.Completed }).AsList();
                return list;
            }
        }

        public List<OrderMonthStat> GetTradingStatInMonth(Guid merchantId, DateTime startDate, DateTime endDate)
        {
            string strSql = @"SELECT YEAR([Timestamp]) as OrderYear,MONTH([Timestamp]) as OrderMonth,
                              COUNT(MerchantAccountId) as OrderCount,
                              SUM([UnifiedFiatAmount]) as OrderAmount 
                              FROM [dbo].[Orders]  
                              WHERE [MerchantAccountId]=@MerchantAccountId AND Timestamp BETWEEN @StartDate AND @EndDate AND Status = @Status
                              GROUP BY YEAR([Timestamp]),MONTH([Timestamp])  order by OrderYear,OrderMonth ASC";

            using (var con = ReadConnection())
            {
                List<OrderMonthStat> list = con.Query<OrderMonthStat>(strSql, new { MerchantAccountId = merchantId, StartDate = startDate, EndDate = endDate, Status = OrderStatus.Completed }).AsList();
                return list;
            }
        }

        #region Merchant_Web

        /// <summary>
        /// 根据orderId获取实体
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Order GetOrderByOrderId(Guid orderId)
        {
            string strSql = "SELECT * FROM [dbo].[Orders] WHERE [Id]=@Id";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Order>(strSql, new { Id = orderId });
            }
        }

        /// <summary>
        /// 获取分页数据列表
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="orderSN"></param>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<OrderByPage> GetOrderRecordListByPage(Guid merchantId, string orderSN, int status, string startDate, string endDate, int pageIndex, int pageSize)
        {
            string strSql = @"SELECT a.*,c.SN as PostSN,d.Cellphone,a.CryptoCode as CryptoName,
                             (case when f.Id is null or f.Amount = 0 then a.TransactionFee else f.Amount end) as WithdrawalFee,
                             (case when f.Id is null then a.CryptoId else f.CryptoId end) as WithdrawalCryptoId FROM Orders a 
                              LEFT JOIN MerchantAccounts b ON a.MerchantAccountId=b.Id
                              LEFT JOIN POS c ON b.POSId =c.Id
                              LEFT JOIN UserAccounts d ON a.UserAccountId=d.Id
							  LEFT JOIN OrderWithdrawalFee f on a.Id = f.OrderId
                              WHERE 1=1";

            var p = new DynamicParameters();
            strSql += " AND a.MerchantAccountId=@MerchantAccountId";
            p.Add("MerchantAccountId", merchantId);

            if (!string.IsNullOrEmpty(orderSN))
            {
                strSql += " AND a.OrderNo=@OrderNo";
                p.Add("OrderNo", orderSN);
            }
            if (status > 1)
            {
                strSql += " AND a.Status=@Status";
                p.Add("Status", status);
            }
            else
            {
                strSql += " AND a.Status > 1";
            }
            bool result = DateTime.TryParse(startDate, out DateTime startTime);
            if (result)
            {
                startTime = startTime.ToUniversalTime(); //转utc
                strSql += " AND a.Timestamp >= @StartDate";
                p.Add("StartDate", startTime);
            }
            result = DateTime.TryParse(endDate + " 23:59:59", out DateTime endTime);
            if (result)
            {
                endTime = endTime.ToUniversalTime(); //转utc
                strSql += " AND a.Timestamp <= @EndDate";
                p.Add("EndDate", endTime);
            }

            string sortOrderBy = "Timestamp DESC";
            long startIndex = (pageIndex - 1) * pageSize + 1;
            string cmdText = "WITH p_pager_sorted AS";
            cmdText += string.Format("(SELECT ROW_NUMBER() OVER (ORDER BY {0}) AS RowNumber, *, count(1) over() as TotalCount from (", sortOrderBy);
            cmdText += strSql;
            cmdText += ") p_pager_pagertable) SELECT * FROM p_pager_sorted";
            cmdText += string.Format(" WHERE RowNumber BETWEEN {0} and {1}", startIndex, startIndex + pageSize - 1);

            using (var con = ReadConnection())
            {
                List<OrderByPage> list = con.Query<OrderByPage>(cmdText, p).AsList();

                return list;
            }
        }


        /// <summary>
        /// 统计今天、所有交易的订单量和金额
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public OrderStat GetOrderStat(Guid merchantId, int status = 2)
        {
            string strSql = @"SELECT
                            (SELECT COUNT(1) FROM Orders WHERE MerchantAccountId=@MerchantAccountId AND Status=@Status) as TotalCount,
                            (SELECT ISNULL(SUM(ActualFiatAmount),0) FROM Orders WHERE MerchantAccountId=@MerchantAccountId AND Status=@Status) as TotalMoney,
                            (SELECT COUNT(1) FROM Orders WHERE MerchantAccountId=@MerchantAccountId AND Status=@Status AND  CONVERT(varchar(100),Timestamp, 23)= CONVERT(varchar(100), GetUTCDate(), 23)) as TodayCount,
                            (SELECT ISNULL(SUM(ActualFiatAmount),0) FROM Orders WHERE MerchantAccountId=@MerchantAccountId AND Status=@Status  AND  CONVERT(varchar(100),Timestamp, 23)= CONVERT(varchar(100), GetUTCDate(), 23)) as TodayMoney";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<OrderStat>(strSql, new { MerchantAccountId = merchantId, Status = status });
            }
        }
        #endregion

    }
}