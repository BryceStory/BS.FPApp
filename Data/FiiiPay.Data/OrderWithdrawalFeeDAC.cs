using System;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class OrderWithdrawalFeeDAC : BaseDataAccess
    {
        public int Insert(OrderWithdrawalFee fee)
        {
            var sql = "INSERT INTO OrderWithdrawalFee (CryptoId, Amount, Timestamp, OrderId) VALUES (@CryptoId, @Amount, @Timestamp, @OrderId);SELECT SCOPE_IDENTITY()";
            using (var con = ReadConnection())
            {
                return con.Query<int>(sql, new {fee.CryptoId, fee.Amount, fee.Timestamp,fee.OrderId}).FirstOrDefault();
            }
        }

        public OrderWithdrawalFee GetById(long feeId)
        {
            var sql = "SELECT * FROM OrderWithdrawalFee WHERE Id = @Id";
            using (var con = ReadConnection())
            {
                return con.Query<OrderWithdrawalFee>(sql, new { Id = feeId }).FirstOrDefault();
            }
        }

        public OrderWithdrawalFee GetByOrderId(Guid orderId)
        {
            var sql = "SELECT * FROM OrderWithdrawalFee WHERE OrderId = @OrderId";
            using (var con = ReadConnection())
            {
                return con.Query<OrderWithdrawalFee>(sql, new { OrderId = orderId }).FirstOrDefault();
            }
        }
    }
}
