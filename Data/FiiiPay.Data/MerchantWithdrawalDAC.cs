using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class MerchantWithdrawalDAC : BaseDataAccess
    {
        public List<MerchantWithdrawalES> GetByMerchantAccountId(Guid accountId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT a.CryptoId,b.*,c.Fee WithdrawalFee FROM [MerchantWallets] a 
                                   JOIN [MerchantWithdrawals] b ON a.Id=b.MerchantWalletId
                                   JOIN [MerchantWithdrawalFee] c ON b.Id=c.WithdrawalId
                                  WHERE a.MerchantAccountId=@MerchantAccountId
                               ORDER BY b.[Timestamp] DESC
                                 OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var conn = ReadConnection())
            {
                var list = conn.Query<MerchantWithdrawalES>(sql, new
                {
                    MerchantAccountId = accountId,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();
                return list;
            }
        }
        public MerchantWithdrawal GetById(long depositId)
        {
            const string sql = @"SELECT * FROM [MerchantWithdrawals]  WHERE Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantWithdrawal>(sql, new { Id = depositId });
            }
        }

        public MerchantWithdrawalES GetById(Guid accountId, long id)
        {
            const string sql = @"SELECT a.CryptoId,b.*,c.Fee WithdrawalFee FROM [MerchantWallets] a 
                                   JOIN [MerchantWithdrawals] b ON a.Id=b.MerchantWalletId
                                   JOIN [MerchantWithdrawalFee] c ON b.Id=c.WithdrawalId
                                  WHERE a.MerchantAccountId=@MerchantAccountId AND b.Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantWithdrawalES>(sql, new { MerchantAccountId = accountId, Id = id });
            }
        }

        public MerchantWithdrawal Create(MerchantWithdrawal merchantWithdrawal)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantWithdrawals] ([MerchantAccountId], [MerchantWalletId],  [Address],[Tag], [Amount], [Status], [Timestamp], [Remark],[OrderNo],[TransactionId],[SelfPlatform],[RequestId],[CryptoId],[CryptoCode])
                                      VALUES (@MerchantAccountId, @MerchantWalletId, @Address,@Tag, @Amount, @Status, @Timestamp, @Remark, @OrderNo, @TransactionId,@SelfPlatform,@RequestId,@CryptoId,@CryptoCode); SELECT SCOPE_IDENTITY()";

            using (var conn = WriteConnection())
            {
                merchantWithdrawal.Id = conn.ExecuteScalar<long>(sql, merchantWithdrawal);
                return merchantWithdrawal;
            }
        }

        public void WithdrawalSubmited(long Id, long requestId, string transactionId)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
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

        public decimal DailyWithdrawal(Guid accountId, int cryptoId, DateTime day)
        {
            const string sql = @"SELECT ISNULL(SUM(a.Amount),0)
                                   FROM MerchantWithdrawals a
								   JOIN MerchantWallets b on a.MerchantWalletId=b.Id
                                  WHERE a.[Status]<>3 AND b.MerchantAccountId=@MerchantAccountId and b.CryptoId=@CryptoId and  DateDiff(dd,[Timestamp],@Date)=0";
            using (var conn = ReadConnection())
            {
                return conn.ExecuteScalar<decimal>(sql, new { MerchantAccountId = accountId, CryptoId = cryptoId, Date = day });
            }
        }

        public decimal MonthlyWithdrawal(Guid accountId, int cryptoId, DateTime month)
        {
            const string sql = @"SELECT ISNULL(SUM(a.Amount),0)
                                   FROM MerchantWithdrawals a
								   JOIN MerchantWallets b on a.MerchantWalletId=b.Id
                                  WHERE a.[Status]<>3 AND b.MerchantAccountId=@MerchantAccountId and b.CryptoId=@CryptoId and  DateDiff(mm,[Timestamp],@Date)=0";
            using (var conn = ReadConnection())
            {
                return conn.ExecuteScalar<decimal>(sql, new { MerchantAccountId = accountId, CryptoId = cryptoId, Date = month });
            }
        }

        public MerchantWithdrawal GetByRequestId(Guid accountId, long merchantWalletId, long requestId)
        {
            const string sql = @"SELECT * 
                                   FROM MerchantWithdrawals 
                                  WHERE MerchantAccountId=@MerchantAccountId AND MerchantWalletId=@MerchantWalletId AND RequestId=@RequestId";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantWithdrawal>(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = merchantWalletId,
                    RequestId = requestId
                });
            }
        }

        public void CompletedByRequestId(Guid accountId, long merchantWalletId, long requestId)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
                                    SET [Status] = @Status
                                  WHERE [RequestId] = @RequestId AND [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = merchantWalletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Confirmed
                });
            }
        }

        public void CompletedByRequestId(Guid accountId, long merchantWalletId, long requestId,string transactionId)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
                                    SET [Status] = @Status,[TransactionId]=@TransactionId
                                  WHERE [RequestId] = @RequestId AND [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = merchantWalletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Confirmed,
                    TransactionId = transactionId
                });
            }
        }

        public void UpdateTransactionId(long Id, long requestId, string transactionId)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
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

        public void RejectByRequestId(Guid accountId, long merchantWalletId, long requestId, string reason)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
                                    SET [Status] = @Status,
                                        [Remark] = @Remark
                                  WHERE [RequestId] = @RequestId AND [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = merchantWalletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Cancelled,
                    Remark = reason
                });
            }
        }

        public void RejectById(long id, string reason)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
                                    SET [Status] = @Status,
                                        [Remark] = @Remark
                                  WHERE [Id]=@Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    Id = id,
                    Status = TransactionStatus.Cancelled,
                    Remark = reason
                });
            }
        }

        public void InitTransactionId(Guid accountId, long merchantWalletId, long requestId, string transactionId)
        {
            const string sql = @"UPDATE [dbo].[MerchantWithdrawals]
                                    SET [TransactionId] = @TransactionId
                                  WHERE MerchantAccountId=@MerchantAccountId AND MerchantWalletId=@MerchantWalletId AND RequestId=@RequestId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = merchantWalletId,
                    RequestId = requestId,
                    TransactionId = transactionId
                });
            }
        }
    }
}