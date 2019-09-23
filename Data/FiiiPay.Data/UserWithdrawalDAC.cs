using System;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class UserWithdrawalDAC : BaseDataAccess
    {
        public UserWithdrawalES GetById(Guid accountId, long id)
        {
            const string sql = @"SELECT b.*,c.Fee WithdrawalFee FROM [UserWithdrawals] b 
                                   JOIN [UserWithdrawalFee] c ON b.Id=c.WithdrawalId
                                  WHERE b.UserAccountId=@UserAccountId AND b.Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserWithdrawalES>(sql, new { UserAccountId = accountId, Id = id });
            }
        }

        public UserWithdrawal GetById(long id)
        {
            const string sql = @"SELECT * FROM UserWithdrawals WHERE Id = @Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserWithdrawal>(sql, new { Id = id });
            }
        }

        public long Create(UserWithdrawal UserWithdrawal)
        {
            const string sql = @"INSERT INTO [dbo].[UserWithdrawals] ([UserAccountId], [UserWalletId], [Address],[Tag], [Amount], [Status], [Timestamp], [Remark], [OrderNo], [TransactionId], [SelfPlatform],[RequestId],[CryptoId],[CryptoCode])
                                      VALUES (@UserAccountId, @UserWalletId, @Address,@Tag, @Amount, @Status, @Timestamp, @Remark, @OrderNo, @TransactionId, @SelfPlatform,@RequestId,@CryptoId,@CryptoCode); SELECT SCOPE_IDENTITY()";

            using (var conn = WriteConnection())
            {
                return conn.ExecuteScalar<long>(sql, UserWithdrawal);
            }
        }

        public decimal DailyWithdrawal(Guid accountId, int cryptoId, DateTime day)
        {
            const string sql = @"SELECT ISNULL(SUM(a.Amount),0)
                                   FROM UserWithdrawals a
								   JOIN UserWallets b on a.UserWalletId=b.Id
                                  WHERE b.UserAccountId=@UserAccountId and b.CryptoId=@CryptoId and  DateDiff(dd,[Timestamp],@Date)=0";
            using (var conn = ReadConnection())
            {
                return conn.ExecuteScalar<decimal>(sql, new { UserAccountId = accountId, CryptoId = cryptoId, Date = day });
            }
        }

        public decimal MonthlyWithdrawal(Guid accountId, int cryptoId, DateTime month)
        {
            const string sql = @"SELECT ISNULL(SUM(a.Amount),0)
                                   FROM UserWithdrawals a
								   JOIN UserWallets b on a.UserWalletId=b.Id
                                  WHERE b.UserAccountId=@UserAccountId and b.CryptoId=@CryptoId and  DateDiff(mm,[Timestamp],@Date)=0";
            using (var conn = ReadConnection())
            {
                return conn.ExecuteScalar<decimal>(sql, new { UserAccountId = accountId, CryptoId = cryptoId, Date = month });
            }
        }

        public UserWithdrawal GetByRequestId(Guid accountId, long userWalletId, long requestId)
        {
            const string sql = @"SELECT * 
                                   FROM UserWithdrawals 
                                  WHERE UserAccountId=@UserAccountId AND UserWalletId=@UserWalletId AND RequestId=@RequestId";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<UserWithdrawal>(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = userWalletId,
                    RequestId = requestId
                });
            }
        }

        public void CompletedByRequestId(Guid accountId, long userWalletId, long requestId)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [Status] = @Status
                                  WHERE [RequestId] = @RequestId AND [UserAccountId]=@UserAccountId AND [UserWalletId]=@UserWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = userWalletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Confirmed
                });
            }
        }

        public void CompletedByRequestId(Guid accountId, long userWalletId, long requestId,string transactionId)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [Status] = @Status,[TransactionId]=@TransactionId
                                  WHERE [RequestId] = @RequestId AND [UserAccountId]=@UserAccountId AND [UserWalletId]=@UserWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = userWalletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Confirmed,
                    TransactionId = transactionId
                });
            }
        }

        public void UpdateTransactionId(long Id,long requestId,string transactionId)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [TransactionId] = @TransactionId,
                                        [RequestId] = @RequestId
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = Id,
                    TransactionId = transactionId,
                    RequestId = requestId
                });
            }
        }

        public void WithdrawalSubmited(long Id, long requestId, string transactionId)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [TransactionId] = @TransactionId,[Status]=@Status,
                                        [RequestId] = @RequestId
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = Id,
                    TransactionId = transactionId,
                    RequestId = requestId,
                    Status = (byte)TransactionStatus.Pending
                });
            }
        }

        public void RejectById(long Id,string reason)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [Status] = @Status,
                                        [Remark] = @Remark
                                  WHERE [Id] = @Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = Id,
                    Status = TransactionStatus.Cancelled,
                    Remark = reason
                });
            }
        }

        public void RejectByRequestId(Guid accountId, long userWalletId, long requestId, string reason)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [Status] = @Status,
                                        [Remark] = @Remark
                                  WHERE [RequestId] = @RequestId AND [UserAccountId]=@UserAccountId AND [UserWalletId]=@UserWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = userWalletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Cancelled,
                    Remark = reason
                });
            }
        }
        public void InitTransactionId(Guid accountId, long walletId, long requestId, string transactionId)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [TransactionId] = @TransactionId
                                  WHERE UserAccountId=@UserAccountId AND UserWalletId=@UserWalletId AND RequestId=@RequestId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = accountId,
                    UserWalletId = walletId,
                    RequestId = requestId,
                    TransactionId = transactionId
                });
            }
        }

        public void InitTransactionId(UserWithdrawal withdrawal)
        {
            const string sql = @"UPDATE [dbo].[UserWithdrawals]
                                    SET [TransactionId] = @TransactionId
                                  WHERE UserAccountId=@UserAccountId AND UserWalletId=@UserWalletId AND RequestId=@RequestId";
            using (var conn = ReadConnection())
            {
                conn.Execute(sql, new
                {
                    UserAccountId = withdrawal.UserAccountId,
                    UserWalletId = withdrawal.UserWalletId,
                    RequestId = withdrawal.RequestId.Value,
                    withdrawal.TransactionId
                });
            }
        }
    }
}