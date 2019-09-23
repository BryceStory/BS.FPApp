using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class MerchantExTransferOrderDAC : BaseDataAccess
    {
        public MerchantExTransferOrder Create(MerchantExTransferOrder order)
        {
            const string sql =
                @"INSERT INTO [dbo].[MerchantExTransferOrders]([Timestamp],[OrderNo],[OrderType],[AccountId],[WalletId],[CryptoId],[CryptoCode],[Amount],[Status],[Remark],[ExId])
                       VALUES (@Timestamp,@OrderNo,@OrderType,@AccountId,@WalletId,@CryptoId,@CryptoCode,@Amount,@Status,@Remark,@ExId);SELECT SCOPE_IDENTITY()";
            using (var con = WriteConnection())
            {
                order.Id = con.ExecuteScalar<long>(sql, order);
            }
            return order;
        }

        public MerchantExTransferOrder GetById(long id)
        {
            const string sql = @"SELECT * FROM [MerchantExTransferOrders] WHERE Id=@Id";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<MerchantExTransferOrder>(sql, new { Id = id });
            }
        }

        public List<MerchantExTransferOrder> PagedByAccountId(Guid accountId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT * FROM MerchantExTransferOrders 
								  WHERE AccountId=@AccountId
                               ORDER BY [Timestamp] DESC
                                 OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var conn = ReadConnection())
            {
                var list = conn.Query<MerchantExTransferOrder>(sql, new
                {
                    AccountId = accountId,
                    Offset = (pageIndex - 1) * pageSize,
                    Limit = pageSize
                }).ToList();
                return list;
            }
        }
    }
}