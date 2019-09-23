using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;

namespace FiiiPay.Data
{
    public class InvestorWalletStatementDAC : BaseDataAccess
    {
        public void Insert(InvestorWalletStatement investorWalletStatement)
        {
            const string sql = @"
INSERT INTO [dbo].[InvestorWalletStatements] ([Id],[InvestorId],[TagAccountId],[Action],[Amount],[Balance],[Timestamp],[Remark])
     VALUES (@Id,@InvestorId,@TagAccountId,@Action,@Amount,@Balance,@Timestamp,@Remark); SELECT SCOPE_IDENTITY()";
            using (var con = WriteConnection())
            {
                con.Execute(sql, investorWalletStatement);
            }
        }

        public InvestorWalletStatement GetById(long accountId, Guid statementId)
        {
            const string sql = @"SELECT * FROM [InvestorWalletStatements] WHERE Id=@Id AND InvestorId=@InvestorId";
            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<InvestorWalletStatement>(sql, new { Id = statementId, InvestorId = accountId });
            }
        }

        public List<InvestorWalletStatement> Select(long accountId, long? startTimestamp, long? endTimestamp, int pageIndex, int pageSize, InvestorTransactionType? transactionType = null)
        {
            string sql = @"SELECT * FROM [InvestorWalletStatements] WHERE [InvestorId]=@InvestorId";

            Dictionary<string, object> paramDic = new Dictionary<string, object> { { "InvestorId", accountId } };
            if (startTimestamp.HasValue)
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime startTime = start.AddMilliseconds(startTimestamp.Value);
                sql += " AND [Timestamp]>=@startTime";
                paramDic.Add("startTime", startTime);
            }

            if (endTimestamp.HasValue)
            {
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endTime = start.AddMilliseconds(endTimestamp.Value);
                sql += " AND [Timestamp]<@endTimestamp";
                paramDic.Add("endTimestamp", endTime);
            }
            if (transactionType.HasValue)
            {
                sql += " AND [Action]=@Action";
                paramDic.Add("Action", transactionType.ToString());
            }
            else
            {
                sql += " AND [Action]<>@Action";
                paramDic.Add("Action", InvestorTransactionType.Transfer.ToString());
            }

            sql += " ORDER BY [Timestamp] DESC";
            sql += $" OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using (var conn = ReadConnection())
            {
                return conn.Query<InvestorWalletStatement>(sql, paramDic).ToList();
            }
        }
    }
}