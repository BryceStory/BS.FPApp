using System;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class RefundDAC : BaseDataAccess
    {
        public void Insert(Refund refund)
        {
            const string sql = @"INSERT INTO [dbo].[Refunds]([OrderId],[Timestamp],[Status],[Remark])
                                      VALUES (@OrderId,@Timestamp,@Status,@Remark)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, refund);
            }
        }

        public Refund GetByOrderId(Guid orderId)
        {
            const string sql = @"SELECT * FROM Refunds WHERE OrderId=@OrderId";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Refund>(sql, new {OrderId = orderId});
            }
        }
    }
}