using Dapper;
using FiiiPay.Entities;
using System;

namespace FiiiPay.Data
{
    public class GatewayRefundOrderDAC : BaseDataAccess
    {
        public void Insert(GatewayRefundOrder gatewayRefundOrder)
        {
            const string sql =
            @"INSERT INTO [dbo].[GatewayRefundOrders]([Id],[OrderId],[Timestamp],[Status],[RefundTradeNo],[Remark]) VALUES (@Id,@OrderId,@Timestamp,@Status,@RefundTradeNo,@Remark)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, gatewayRefundOrder);
            }
        }

        public GatewayRefundOrder GetByOrderId(Guid orderId)
        {
            const string sql = @"SELECT * FROM GatewayRefundOrders WHERE OrderId=@OrderId";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<GatewayRefundOrder>(sql, new { OrderId = orderId });
            }
        }

        public GatewayRefundOrder GetById(Guid id)
        {
            const string sql = @"SELECT * FROM GatewayRefundOrders WHERE Id=@Id";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<GatewayRefundOrder>(sql, new { Id = id });
            }
        }
    }
}
