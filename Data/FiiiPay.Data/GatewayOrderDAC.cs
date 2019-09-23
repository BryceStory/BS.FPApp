using Dapper;
using FiiiPay.Entities;
using System;

namespace FiiiPay.Data
{
    public class GatewayOrderDAC : BaseDataAccess
    {
        public void Insert(GatewayOrder order)
        {
            const string sql =
            @"INSERT INTO [dbo].[GatewayOrders]([Id],[OrderNo],[TradeNo],[MerchantAccountId],[MerchantName],[UserAccountId],[CryptoId],[CryptoAmount],[ActualCryptoAmount],[ActualFiatAmount],[FiatAmount],[FiatCurrency],[Status],[Timestamp],[Remark],[ExchangeRate],[Markup],[ExpiredTime],[TransactionFee],[PaymentTime])
                       VALUES (@Id,@OrderNo,@TradeNo,@MerchantAccountId,@MerchantName,@UserAccountId,@CryptoId,@CryptoAmount,@ActualCryptoAmount,@ActualFiatAmount,@FiatAmount,@FiatCurrency,@Status,@Timestamp,@Remark,@ExchangeRate,@Markup,@ExpiredTime,@TransactionFee,@PaymentTime)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, order);
            }
        }

        public void Delete(Guid id)
        {
            const string sql =
            @"DELETE FROM GatewayOrders WHERE Id=@id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, id);
            }
        }

        public void Update(GatewayOrder model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE GatewayOrders SET Status=@Status WHERE Id=@Id", new { Status = model.Status, Id = model.Id} );
            }
        }

        public void UpdateStatus(Guid id, byte status)
        {
            const string sql = @"UPDATE [dbo].[GatewayOrders]
                                    SET [Status] = @Status
                                  WHERE [Id]=@Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new
                {
                    Id = id,
                    Status = status
                });
            }
        }

        /// <summary>
        /// 查找订单
        /// </summary>
        /// <param name="tradeNo">映射gateway表中的订单唯一标识</param>
        /// <returns></returns>
        public GatewayOrder GetByTradeNo(string tradeNo)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<GatewayOrder>("SELECT * FROM GatewayOrders WHERE TradeNo = @tradeNo", new { tradeNo });
            }
        }

        public GatewayOrder GetByOrderNo(string id)
        {
            const string sql = @"SELECT * FROM GatewayOrders
                                  WHERE [OrderNo]=@OrderNo";

            using (var con = WriteConnection())
            {
                return con.QueryFirstOrDefault<GatewayOrder>(sql, new { OrderNo = id });
            }
        }

        public GatewayOrder GetByOrderId(Guid orderId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<GatewayOrder>("SELECT * FROM GatewayOrders WHERE Id = @orderId", new { orderId });
            }
        }
    }
}
