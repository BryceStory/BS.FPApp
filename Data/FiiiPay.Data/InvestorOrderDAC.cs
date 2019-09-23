using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet.Investor;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class InvestorOrderDAC : BaseDataAccess
    {
        public InvestorOrder Insert(InvestorOrder investorOrder)
        {
            const string sql =
                @"INSERT INTO [dbo].[InvestorOrders]([Id],[OrderNo],[InverstorAccountId],[TransactionType],[Status],[UserAccountId],[CryptoId],[CryptoAmount],[Timestamp],[Remark])
                       VALUES (@Id,@OrderNo,@InverstorAccountId,@TransactionType,@Status,@UserAccountId,@CryptoId,@CryptoAmount,@Timestamp,@Remark)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, investorOrder);
            }

            return investorOrder;
        }

        public List<StatementES> SelectByUserCellphone(long accountId, long? startTimestamp, long? endTimestamp, string cellphone, int pageIndex, int pageSize)
        {
            string sql = "SELECT a.*,b.Cellphone FROM [InvestorOrders] a JOIN UserAccounts b ON a.UserAccountId=b.Id WHERE [InverstorAccountId]=@InverstorAccountId AND b.[Cellphone]=@Cellphone";
            Dictionary<string, object> paramDic = new Dictionary<string, object>
            {
                { "InverstorAccountId", accountId },
                { "Cellphone", cellphone }
            };
            if (startTimestamp.HasValue)
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime startTime = start.AddMilliseconds(startTimestamp.Value);
                sql += " AND a.[Timestamp]>=@startTime";
                paramDic.Add("startTime", startTime);
            }

            if (endTimestamp.HasValue)
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endTime = start.AddMilliseconds(endTimestamp.Value);
                sql += " AND a.[Timestamp]<@endTimestamp";
                paramDic.Add("endTimestamp", endTime);
            }

            sql += " ORDER BY a.Timestamp DESC";
            sql += $" OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using (var con = ReadConnection())
            {
                return con.Query<StatementES>(sql, paramDic).ToList();
            }
        }

        public InvestorOrder GetById(long accountId, Guid orderId)
        {
            const string sql = @"SELECT * FROM [InvestorOrders] WHERE Id=@Id AND InverstorAccountId=@InverstorAccountId";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<InvestorOrder>(sql, new { Id = orderId, InverstorAccountId = accountId });
            }
        }

        public List<StatementES> SelectAndStatments(long accountId, long? startTimestamp, long? endTimestamp, int pageIndex, int pageSize, InvestorTransactionType? transactionType)
        {
            string sql = @"SELECT * FROM 
	                        (SELECT Id, TransactionType, CryptoAmount Amount, [Timestamp]  
	                           FROM [InvestorOrders] 
	                          WHERE [InverstorAccountId]=@InvestorId {0} 
                              UNION ALL
                             SELECT Id,[Action] TransactionType, Amount,[Timestamp]
	                           FROM InvestorWalletStatements 
	                          WHERE InvestorId=@InvestorId AND [Action]<>@Action {1} 
	                         )a";
            Dictionary<string, object> paramDic =
                new Dictionary<string, object>
                {
                    {"InvestorId", accountId},
                    {"Action", InvestorTransactionType.Transfer}
                };

            string orderWhere = string.Empty,
               statementWhere = string.Empty;

            if (transactionType.HasValue)
            {
                orderWhere += " AND [TransactionType]=@TransactionType";
                statementWhere += " AND [Action]=@TransactionType";
                paramDic.Add("TransactionType", transactionType);
            }

            if (startTimestamp.HasValue)
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime startTime = start.AddMilliseconds(startTimestamp.Value);
                orderWhere += " AND [Timestamp]>=@startTime";
                statementWhere += " AND [Timestamp]>=@startTime";
                paramDic.Add("startTime", startTime);
            }

            if (endTimestamp.HasValue)
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endTime = start.AddMilliseconds(endTimestamp.Value);
                orderWhere += " AND [Timestamp]<@endTimestamp";
                statementWhere += " AND [Timestamp]<@endTimestamp";
                paramDic.Add("endTimestamp", endTime);
            }

            sql = string.Format(sql, orderWhere, statementWhere);

            sql += " ORDER BY [Timestamp] DESC";
            sql += $" OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using (var con = ReadConnection())
            {
                return con.Query<StatementES>(sql, paramDic).ToList();
            }
        }
    }
}