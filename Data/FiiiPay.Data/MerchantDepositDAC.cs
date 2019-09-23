using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class MerchantDepositDAC : BaseDataAccess
    {
        public List<MerchantDepositES> GetByMerchantAccountId(Guid accountId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT a.CryptoId,b.* FROM [MerchantWallets] a 
                                   JOIN [MerchantDeposits] b ON a.Id=b.MerchantWalletId
                                  WHERE a.MerchantAccountId=@MerchantAccountId
                               ORDER BY b.[Timestamp] DESC
                                 OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var conn = ReadConnection())
            {
                var list = conn.Query<MerchantDepositES>(sql, new
                {
                    MerchantAccountId = accountId,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();
                return list;
            }
        }

        public MerchantDeposit GetById(long depositId)
        {
            const string sql = @"SELECT * FROM [MerchantDeposits]  WHERE Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantDeposit>(sql, new { Id = depositId });
            }
        }

        public MerchantDepositES GetById(Guid accountId, long depositId)
        {
            const string sql = @"SELECT a.CryptoId,b.* FROM [MerchantWallets] a 
                                   JOIN [MerchantDeposits] b ON a.Id=b.MerchantWalletId
                                  WHERE a.MerchantAccountId=@MerchantAccountId AND b.Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantDepositES>(sql, new { MerchantAccountId = accountId, Id = depositId });
            }
        }

        public MerchantDeposit Insert(MerchantDeposit merchantDeposit)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantDeposits] ([MerchantAccountId],[MerchantWalletId],[FromType],[FromAddress],[ToAddress],[Amount],[Status],[Timestamp],[Remark],[OrderNo],[TransactionId],[SelfPlatform],[RequestId],[CryptoCode])
                                    VALUES (@MerchantAccountId,@MerchantWalletId,@FromType,@FromAddress,@ToAddress,@Amount,@Status,@Timestamp,@Remark,@OrderNo,@TransactionId,@SelfPlatform,@RequestId,@CryptoCode); SELECT SCOPE_IDENTITY()";

            using (var conn = WriteConnection())
            {
                merchantDeposit.Id = conn.ExecuteScalar<long>(sql, merchantDeposit);
                return merchantDeposit;
            }
        }

        public MerchantDeposit GetByRequestId(Guid accountId, long requestId)
        {
            const string sql = @"SELECT * FROM MerchantDeposits WHERE MerchantAccountId=@accountId AND RequestId = @requestId";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantDeposit>(sql, new { accountId, requestId });
            }
        }

        public void CompletedByRequestId(Guid accountId, long walletId, long requestId)
        {
            const string sql = @"UPDATE [dbo].[MerchantDeposits]
                                    SET [Status] = @Status,
                                        [Timestamp]=@Timestamp
                                  WHERE [RequestId] = @RequestId AND [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = walletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Confirmed,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public void CancelByRequestId(Guid accountId, long walletId, long requestId)
        {
            const string sql = @"UPDATE [dbo].[MerchantDeposits]
                                    SET [Status] = @Status,
                                        [Timestamp]=@Timestamp
                                  WHERE [RequestId] = @RequestId AND [MerchantAccountId]=@MerchantAccountId AND [MerchantWalletId]=@MerchantWalletId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    MerchantAccountId = accountId,
                    MerchantWalletId = walletId,
                    RequestId = requestId,
                    Status = TransactionStatus.Cancelled,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}