using System;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class UserDepositDAC : BaseDataAccess
    {
        public UserDeposit GetById(Guid accountId, long id)
        {
            const string sql = @"SELECT * FROM [UserDeposits] WHERE UserAccountId=@UserAccountId AND Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserDeposit>(sql, new { UserAccountId = accountId, Id = id });
            }
        }

        public UserDeposit GetById(long id)
        {
            const string sql = @"SELECT * FROM UserDeposits WHERE Id = @Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserDeposit>(sql, new { Id = id });
            }
        }

        public UserDeposit Insert(UserDeposit userDeposit)
        {
            const string sql = @"INSERT INTO [dbo].[UserDeposits] ([UserAccountId],[UserWalletId],[FromType],[FromAddress],[FromTag],[ToAddress],[ToTag],[Amount],[Status],[Timestamp],[Remark],[OrderNo],[TransactionId],[SelfPlatform],[RequestId],[CryptoCode])
                                    VALUES (@UserAccountId,@UserWalletId,@FromType,@FromAddress,@FromTag,@ToAddress,@ToTag,@Amount,@Status,@Timestamp,@Remark,@OrderNo,@TransactionId,@SelfPlatform,@RequestId,@CryptoCode); SELECT SCOPE_IDENTITY()";

            using (var conn = WriteConnection())
            {
                userDeposit.Id = conn.ExecuteScalar<long>(sql, userDeposit);
                return userDeposit;
            }
        }

        public UserDeposit GetByRequestId(Guid accountId,long requestId)
        {
            const string sql = @"SELECT * FROM UserDeposits WHERE [UserAccountId]=@accountId AND RequestId = @requestId";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserDeposit>(sql, new { accountId, requestId });
            }
        }

        public void CompletedByRequestId(Guid accountId, long walletId, long requestId)
        {
            const string sql = @"UPDATE [dbo].[UserDeposits]
                                    SET [Status] = @Status,
                                        [Timestamp]=@Timestamp
                                  WHERE [RequestId] = @RequestId AND [UserAccountId]=@UserAccountId AND [UserWalletId]=@UserWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = walletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Confirmed,
                    Timestamp= DateTime.UtcNow
                });
            }
        }

        public void CancelByRequestId(Guid accountId, long walletId, long requestId)
        {
            const string sql = @"UPDATE [dbo].[UserDeposits]
                                    SET [Status] = @Status,
                                        [Timestamp]=@Timestamp
                                  WHERE [RequestId] = @RequestId AND [UserAccountId]=@UserAccountId AND [UserWalletId]=@UserWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = walletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Cancelled,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}