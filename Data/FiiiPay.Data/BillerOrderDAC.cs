using System;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class BillerOrderDAC : BaseDataAccess
    {
        public void Insert(BillerOrder order)
        {
            const string sql =
                @"INSERT INTO [dbo].[BillerOrders]([Id],[OrderNo],[FiatAmount],[CryptoAmount],[BillerCode],[ReferenceNumber],[CryptoId],[CryptoCode],[ExchangeRate],[Timestamp],[Discount],[FiatCurrency],[Status],[Tag],[AccountId],[CountryId],[PayTime])
                       VALUES (@Id,@OrderNo,@FiatAmount,@CryptoAmount,@BillerCode,@ReferenceNumber,@CryptoId,@CryptoCode,@ExchangeRate,@Timestamp,@Discount,@FiatCurrency,@Status,@Tag,@AccountId,@CountryId,@PayTime)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, order);
            }
        }

        public BillerOrder GetById(Guid id)
        {
            const string sql = @"SELECT * FROM [dbo].[BillerOrders] WHERE Id = @Id";
            using (var con = ReadConnection())
            {
                return con.QueryFirst<BillerOrder>(sql,new {Id = id});
            }
        }

        public void UpdateStatus(BillerOrder order)
        {
            const string sql = @"UPDATE [dbo].[BillerOrders]
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

        public decimal GetDayAmount(DateTime date)
        {
            const string sql =
                "SELECT ISNULL(SUM(FiatAmount),0) FROM [dbo].[BillerOrders] WHERE DateDiff(dd, [Timestamp], @Date)=0 AND Status != 1 ";

            using (var con = ReadConnection())
            {
                return con.QueryFirst<decimal>(sql, new { Date = date });
            }
        }
        public decimal GetMonthAmount(DateTime date)
        {
            const string sql =
                "SELECT ISNULL(SUM(FiatAmount),0) FROM [dbo].[BillerOrders] WHERE DateDiff(mm, [Timestamp], @Date)=0 AND Status != 1 ";

            using (var con = ReadConnection())
            {
                return con.QueryFirst<decimal>(sql, new { Date = date });
            }
        }
    }
}
