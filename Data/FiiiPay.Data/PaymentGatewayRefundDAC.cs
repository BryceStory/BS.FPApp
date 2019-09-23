using System;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class PaymentGatewayRefundDAC : BaseDataAccess
    {
        public void Insert(PaymentGatewayRefund refundOrder)
        {
            const string sql =
                @"INSERT INTO [dbo].[PaymentGatewayRefunds]([Id],[OrderId],[Timestamp],[Status],[Remark]) VALUES (@Id,@OrderId,@Timestamp,@Status,@Remark)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, refundOrder);
            }
        }

        public PaymentGatewayRefund GetByOrderId(Guid orderId)
        {
            const string sql = @"SELECT * FROM PaymentGatewayRefunds WHERE OrderId=@OrderId";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<PaymentGatewayRefund>(sql, new { OrderId = orderId });
            }
        }

        public PaymentGatewayRefund GetById(Guid id)
        {
            const string sql = @"SELECT * FROM PaymentGatewayRefunds WHERE Id=@Id";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<PaymentGatewayRefund>(sql, new { Id = id });
            }
        }
    }
}
