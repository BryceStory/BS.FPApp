using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class UserExTransferOrderDAC : BaseDataAccess
    {
        public UserExTransferOrder Create(UserExTransferOrder order)
        {
            const string sql =
                @"INSERT INTO [dbo].[UserExTransferOrders]([Timestamp],[OrderNo],[OrderType],[AccountId],[WalletId],[CryptoId],[CryptoCode],[Amount],[Status],[Remark],[ExId])
                       VALUES (@Timestamp,@OrderNo,@OrderType,@AccountId,@WalletId,@CryptoId,@CryptoCode,@Amount,@Status,@Remark,@ExId);SELECT SCOPE_IDENTITY()";
            using (var con = WriteConnection())
            {
                order.Id = con.ExecuteScalar<long>(sql, order);
                return order;
            }
        }

        public UserExTransferOrder GetById(long id)
        {
            const string sql = @"SELECT * FROM [UserExTransferOrders] WHERE Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserExTransferOrder>(sql, new { Id = id });
            }
        }
    }
}