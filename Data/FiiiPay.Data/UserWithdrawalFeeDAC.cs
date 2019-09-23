using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class UserWithdrawalFeeDAC : BaseDataAccess
    {
        public void Create(UserWithdrawalFee UserWithdrawalFee)
        {
            const string sql = @"INSERT INTO [dbo].[UserWithdrawalFee] ([WithdrawalId],[Amount],[Fee],[Timestamp],[Remark])
                                    VALUES (@WithdrawalId,@Amount,@Fee,@Timestamp,@Remark)";

            using (var conn = WriteConnection())
            {
                conn.Execute(sql, UserWithdrawalFee);
            }
        }

        public UserWithdrawalFee GetByWithdrawalId(long WithdrawalId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<UserWithdrawalFee>("SELECT * FROM UserWithdrawalFee WHERE WithdrawalId = @WithdrawalId",new { WithdrawalId });
            }
        }
    }
}