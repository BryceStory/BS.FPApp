using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class UserTransferDAC : BaseDataAccess
    {
        public UserTransfer GetTransfer(long transferId)
        {
            string sql = "SELECT * FROM [dbo].[UserTransfers] WHERE Id=@TransferId";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserTransfer>(sql, new { TransferId = transferId });
            }
        }

        public long Insert(UserTransfer transfer)
        {
            string sql = "INSERT INTO [dbo].[UserTransfers]([Timestamp],[OrderNo],[FromUserAccountId],[FromUserWalletId],[CoinId],[CoinCode],[ToUserAccountId],[ToUserWalletId],[Amount],[Status],[Remark])";
            sql += " VALUES ";
            sql += "(@Timestamp,@OrderNo,@FromUserAccountId,@FromUserWalletId,@CoinId,@CoinCode,@ToUserAccountId,@ToUserWalletId,@Amount,@Status,@Remark); SELECT SCOPE_IDENTITY()";
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(sql, transfer);
            }
        }
    }
}
