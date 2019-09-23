using Dapper;
using FiiiPay.Entities;
using System;

namespace FiiiPay.Data
{
    public class RefundGatewayOrderDAC : BaseDataAccess
    {
        public void Insert(RefundGatewayOrders refundOrder)
        {
            const string sql =
            @"INSERT INTO [dbo].[GatewayRefundOrders]([OrderId],[Timestamp],[Status],[Remark]) VALUES (@OrderId,@Timestamp,@Status,@Remark)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, refundOrder);
            }
        }

        public RefundGatewayOrders GetByOrderId(Guid orderId)
        {
            const string sql = @"SELECT * FROM GatewayRefundOrders WHERE OrderId=@OrderId";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<RefundGatewayOrders>(sql, new { OrderId = orderId });
            }
        }

        public RefundGatewayOrders GetById(long id)
        {
            const string sql = @"SELECT * FROM RefundGatewayOrders WHERE Id=@Id";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<RefundGatewayOrders>(sql, new { Id = id });
            }
        }
    }
}
