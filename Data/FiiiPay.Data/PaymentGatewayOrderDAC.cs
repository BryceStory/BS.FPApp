using System;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class PaymentGatewayOrderDAC : BaseDataAccess
    {
        public PaymentGatewayOrder Create(PaymentGatewayOrder order)
        {
            const string sql =
                @"INSERT INTO [dbo].[PaymentGatewayOrders]([Id],[MerchantOrderId],[MerchantUserName],[UserAccountId],[CryptoCode],[CryptoAmount],[ActualCryptoAmount],[ActualFiatAmount],[FiatAmount],[FiatCurrency],[Status],[Timestamp],[Remark],[ExchangeRate],[Markup],[ExpiredTime],[MerchantToken],[UserToken],[PaymentType],[PaymentTime])
                       VALUES (@Id,@MerchantOrderId,@MerchantUserName,@UserAccountId,@CryptoCode,@CryptoAmount,@ActualCryptoAmount,@ActualFiatAmount,@FiatAmount,@FiatCurrency,@Status,@Timestamp,@Remark,@ExchangeRate,@Markup,@ExpiredTime,@MerchantToken,@UserToken,@PaymentType,@PaymentTime)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, order);
            }

            return order;
        }

        public void UpdateStatus(Guid id, byte status)
        {
            const string sql = @"UPDATE [dbo].[PaymentGatewayOrders]
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

        public PaymentGatewayOrder GetById(Guid id)
        {
            const string sql = @"SELECT * FROM PaymentGatewayOrders
                                  WHERE [Id]=@Id";

            using (var con = WriteConnection())
            {
                return con.QueryFirstOrDefault<PaymentGatewayOrder>(sql, new { Id = id });
            }
        }

        public PaymentGatewayOrder GetByMerchantOrderId(string id)
        {
            const string sql = @"SELECT * FROM PaymentGatewayOrders
                                  WHERE [MerchantOrderId]=@MerchantOrderId";

            using (var con = WriteConnection())
            {
                return con.QueryFirstOrDefault<PaymentGatewayOrder>(sql, new { MerchantOrderId = id });
            }
        }
    }
}
