using Dapper;
using FiiiPay.Entities;
using System;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class StoreOrderDAC : BaseDataAccess
    {
        public async Task CreateAsync(StoreOrder order)
        {
            const string sql =
            @"INSERT INTO [dbo].[StoreOrders]([Id],[OrderNo],[Timestamp],[Status],[MerchantInfoId],[MerchantInfoName],[UserAccountId],[CryptoId],[CryptoCode],[CryptoAmount],[CryptoActualAmount],[ExchangeRate],[Markup],[FiatCurrency],[FiatAmount],[FiatActualAmount],[FeeRate],[TransactionFee],[PaymentTime],[Remark])
                VALUES (@Id,@OrderNo,@Timestamp,@Status,@MerchantInfoId,@MerchantInfoName,@UserAccountId,@CryptoId,@CryptoCode,@CryptoAmount,@CryptoActualAmount,@ExchangeRate,@Markup,@FiatCurrency,@FiatAmount,@FiatActualAmount,@FeeRate,@TransactionFee,@PaymentTime,@Remark)";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, order);
            }
        }

        public async Task<StoreOrder> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM [dbo].[StoreOrders] WHERE Id=@Id";
            using (var con = await ReadConnectionAsync())
            {
                return await con.QuerySingleOrDefaultAsync<StoreOrder>(sql, new { Id=id });
            }
        }
    }
}
