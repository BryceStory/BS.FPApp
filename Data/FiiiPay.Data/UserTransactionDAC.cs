using Dapper;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class UserTransactionDAC : BaseDataAccess
    {
        public void Insert(UserTransaction tran)
        {
            const string sql = @"INSERT INTO [dbo].[UserTransactions]([Id],[AccountId],[CryptoId],[CryptoCode],[Type],[DetailId],[Status],[Timestamp],[Amount],[OrderNo],[MerchantName])
 VALUES (@Id,@AccountId,@CryptoId,@CryptoCode,@Type,@DetailId,@Status,@Timestamp,@Amount,@OrderNo,@MerchantName)";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, tran);
            }
        }

        public async Task InsertAsync(UserTransaction tran)
        {
            const string sql = @"INSERT INTO [dbo].[UserTransactions]([Id],[AccountId],[CryptoId],[CryptoCode],[Type],[DetailId],[Status],[Timestamp],[Amount],[OrderNo],[MerchantName])
 VALUES (@Id,@AccountId,@CryptoId,@CryptoCode,@Type,@DetailId,@Status,@Timestamp,@Amount,@OrderNo,@MerchantName)";
            using (var conn = await WriteConnectionAsync())
            {
                await conn.ExecuteAsync(sql, tran);
            }
        }

        public void UpdateStatus(UserTransactionType userTransactionType, string detailId,Guid accountId, byte transactionStatus)
        {
            const string sql = @"UPDATE [dbo].[UserTransactions] SET [Status]=@Status WHERE [Type]=@Type AND [DetailId]=@DetailId AND [AccountId]=@AccountId";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { Status = transactionStatus, Type = (byte)userTransactionType, DetailId = detailId, AccountId = accountId });
            }
        }

        public async Task UpdateStatusAsync(UserTransactionType userTransactionType, string detailId, Guid accountId, byte transactionStatus)
        {
            const string sql = @"UPDATE [dbo].[UserTransactions] SET [Status]=@Status WHERE [Type]=@Type AND [DetailId]=@DetailId AND [AccountId]=@AccountId";
            using (var conn = await WriteConnectionAsync())
            {
                await conn.ExecuteAsync(sql, new { Status = transactionStatus, Type = (byte)userTransactionType, DetailId = detailId, AccountId = accountId });
            }
        }

        public async Task<List<UserTransaction>> GetListAsync(Guid UserAccountId, int? type, int? coinId, int pageSize, int pageIndex, string mounth, string startDate, string endDate, string keyword, int maxType)
        {
            string sql = "SELECT * FROM [dbo].[UserTransactions] WHERE 1=1 ";
            sql += " AND [AccountId]=@UserAccountId";
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND ([MerchantName] LIKE @keyword OR [OrderNo] LIKE @keyword)";
            }
            if (type.HasValue)
            {
                string typeCondition = " AND [Type]";
                switch (type)
                {
                    case 2://消费
                        typeCondition += " IN (2,3,12)";
                        break;
                    case 3://退款
                        typeCondition += " IN (3,10)";
                        break;
                    case 4://转账
                        typeCondition += " IN (4,5)";
                        break;
                    case 9://FiiiShop订单
                        typeCondition += " IN (9,10)";
                        break;
                    case 14://红包
                        typeCondition += " IN (14,15)";
                        break;
                    default:
                        typeCondition += "=" + type;
                        break;
                }
                sql += typeCondition;
            }
            if (coinId.HasValue)
            {
                sql += $" AND [CryptoId] = {coinId}";
            }
            if (!string.IsNullOrEmpty(mounth))
            {
                sql += " AND DATEPART(YY,[Timestamp]) = @yystr";
                sql += " AND DATEPART(MM,[Timestamp]) = @mmstr";
            }
            else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                if (endDate.Length == 10)
                {
                    endDate += " 23:59:59";
                }
                sql += " AND [Timestamp] > @startDate";
                sql += " AND [Timestamp] <= @endDate";
            }
            sql += $" AND [Type] <= {maxType}";
            sql += " ORDER BY [Timestamp] DESC";
            sql += $" OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            int yy=0,mm=0;
            if (!string.IsNullOrEmpty(mounth))
            {
                var arr = mounth.Split('-');
                if (arr.Length != 2)
                    return new List<UserTransaction>();
                yy = int.Parse(arr[0]);
                mm = int.Parse(arr[1]);
            }

            using (var con = await ReadConnectionAsync())
            {
                var list = await con.QueryAsync<UserTransaction>(sql, new { UserAccountId, startDate, endDate, yystr = yy, mmstr = mm, keyword = string.IsNullOrEmpty(keyword) ? null : $"%{keyword}%" });
                return list.AsList();
            }
        }
    }
}
