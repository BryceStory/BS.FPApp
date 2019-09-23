using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class InviteRecordDAC : BaseDataAccess
    {
        public List<ProfitDetailES> GetFiiiPayProfitDetails(Guid accountId, Tuple<int,int> pageTuples = null)
        {
            string sql = string.Empty;
            if(pageTuples == null)
            {
                sql = $"SELECT a.*, b.[AccountId] AS InvitedAccountId, c.[PhoneCode] AS PhoneCode, c.[CellPhone] FROM ProfitDetails a JOIN InviteRecords b ON a.InvitationId = b.Id JOIN UserAccounts c ON b.AccountId = c.Id WHERE a.AccountId=@AccountId and a.Type!={(int)ProfitType.InvitePiiiPos}";
            }
            else
            {
                sql = $"SELECT a.*, b.[AccountId] AS InvitedAccountId, c.[PhoneCode] AS PhoneCode, c.[CellPhone] FROM ProfitDetails a JOIN InviteRecords b ON a.InvitationId = b.Id JOIN UserAccounts c ON b.AccountId = c.Id WHERE a.AccountId=@AccountId and a.Type!={(int)ProfitType.InvitePiiiPos} ORDER BY Timestamp asc OFFSET {pageTuples.Item1} ROWS FETCH NEXT {pageTuples.Item2} ROWS ONLY";
            }
            
            using (var con = ReadConnection())
            {
                return con.Query<ProfitDetailES>(sql, new { AccountId = accountId }).AsList();
            }
        }

        public List<FiiiposBonusRecord> GetFiiiPosProfitDetailList(Guid accountId, int type)
        {
            var collection = new List<FiiiposBonusRecord>();

            //var sql = "SELECT b.Timestamp,ISNULL(b.CryptoAmount,0) as CryptoAmount , c.MerchantName, c.Id AS MerchantId  FROM (SELECT * FROM InviteRecords WHERE InviterAccountId = @AccountId and Type = @Type) a LEFT JOIN  ProfitDetails b ON a.Id = b.InvitationId JOIN MerchantAccounts c on a.AccountId = c.Id";
            var sql =
                @"SELECT Sum(ISNULL(b.CryptoAmount,0)) as TotalCryptoAmount, c.MerchantName, c.Id AS MerchantId  
FROM InviteRecords a 
LEFT JOIN  ProfitDetails b ON a.Id = b.InvitationId 
LEFT JOIN MerchantAccounts c on a.AccountId = c.Id
WHERE a.InviterAccountId = @AccountId and a.[Type] = @Type
group by a.Timestamp,c.MerchantName,c.Id";
            using (var con = ReadConnection())
            {
                collection = collection.Concat(con.Query<FiiiposBonusRecord>(sql, new { AccountId = accountId, Type = type }).AsList()).ToList();
            }
            return collection;
        }
        public List<BonusDetailES> GetFiiiPosProfitDetails(Guid accountId, int type)
        {
            var collection = new List<BonusDetailES>();

            //var sql = "SELECT b.Timestamp,ISNULL(b.CryptoAmount,0) as CryptoAmount , c.MerchantName, c.Id AS MerchantId  FROM (SELECT * FROM InviteRecords WHERE InviterAccountId = @AccountId and Type = @Type) a LEFT JOIN  ProfitDetails b ON a.Id = b.InvitationId JOIN MerchantAccounts c on a.AccountId = c.Id";
            var sql =
                @"SELECT a.Timestamp,ISNULL(b.CryptoAmount,0) as CryptoAmount , c.MerchantName, c.Id AS MerchantId  
FROM InviteRecords a 
LEFT JOIN  ProfitDetails b ON a.Id = b.InvitationId 
LEFT JOIN MerchantAccounts c on a.AccountId = c.Id
WHERE a.InviterAccountId = @AccountId and a.[Type] = @Type";
            using (var con = ReadConnection())
            {
                collection = collection.Concat(con.Query<BonusDetailES>(sql, new { AccountId = accountId, Type = type }).AsList()).ToList();
            }
            return collection;
        }
        public Guid GetInvitorIdBySn(string sn)
        {
            const string sql = "SELECT InviterAccountId FROM [dbo].[InviteRecords] WHERE SN=@SN";
            using (var con = WriteConnection())
            {
                return con.Query<Guid>(sql, new { SN = sn }).FirstOrDefault();
            }
        }
        public List<InviteRankES> GetRankDetailsLittle(int type, int count)
        {
            var sql =
                "SELECT TOP(@Count)  b.Cellphone,b.Id AS AccountId,0 AS CryptoAmount,b.PhoneCode FROM dbo.InviteRecords a LEFT JOIN dbo.UserAccounts b ON a.InviterAccountId = b.Id LEFT JOIN dbo.ProfitDetails c ON a.Id = c.InvitationId WHERE a.Type = @Type AND c.Id IS NULL ORDER BY a.Timestamp ASC";
            using (var con = ReadConnection())
            {
                return con.Query<InviteRankES>(sql, new { Count = count, Type = type }).AsList();
            }
        }

        public List<InviteRankES> GetFiiiPayRankDetails(int count)
        {
            var sql = $"SELECT TOP(@Count) a.AccountId,c.Cellphone,c.PhoneCode,MAX(b.Timestamp) AS Timestamp,SUM(a.CryptoAmount) AS CryptoAmount FROM dbo.ProfitDetails a LEFT JOIN dbo.InviteRecords b ON a.InvitationId = b.Id LEFT JOIN dbo.UserAccounts c ON a.AccountId = c.Id WHERE a.Type=@Type1 or a.Type = @Type2 or a.Type = @Type3 GROUP BY a.AccountId,c.Cellphone,c.PhoneCode ORDER BY CryptoAmount DESC, Timestamp ASC";
            using (var con = ReadConnection())
            {
                return con.Query<InviteRankES>(sql,
                    new
                    {
                        Count = count,
                        Type1 = ProfitType.BeInvited,
                        Type2 = ProfitType.InvitePiiiPay,
                        Type3 = ProfitType.Reward
                    }).AsList();
            }
        }

        public List<InviteRankES> GetFiiiPosRankDetails(int count)
        {
            var sql = $"SELECT TOP(@Count) a.AccountId,c.Cellphone,c.PhoneCode,SUM(a.CryptoAmount) AS CryptoAmount FROM dbo.ProfitDetails a LEFT JOIN dbo.InviteRecords b ON a.InvitationId = b.Id LEFT JOIN dbo.UserAccounts c ON a.AccountId = c.Id WHERE a.Type=@Type GROUP BY a.AccountId,c.Cellphone,c.PhoneCode ORDER BY CryptoAmount DESC";
            using (var con = ReadConnection())
            {
                return con.Query<InviteRankES>(sql, new { Count = count, Type = ProfitType.InvitePiiiPos }).AsList();
            }
        }

        public int GetUserRanking(Guid accountId, int platform)
        {
            var str = platform == 1 ? $"a.Type != {(int)ProfitType.InvitePiiiPos}" : $" a.Type = {(int)ProfitType.InvitePiiiPos}";
            
            var sql = $"SELECT f.RowIndex FROM (SELECT AccountId, ROW_NUMBER() OVER (ORDER BY t.TotalAmount DESC,t.LastTimestamp ASC) AS RowIndex FROM (SELECT a.AccountId, SUM(a.CryptoAmount) AS TotalAmount, MAX(b.Timestamp) as LastTimestamp FROM [dbo].[ProfitDetails] a LEFT JOIN dbo.InviteRecords b ON a.InvitationId = b.Id WHERE {str} GROUP BY a.AccountId) t) f where f.AccountId = @AccountId";
            using (var con = ReadConnection())
            {
                return con.Query<int>(sql, new { AccountId = accountId }).FirstOrDefault();
            } 
        }
        
        public int Insert(InviteRecord record)
        {
            var sql = "INSERT INTO InviteRecords (AccountId, InviterCode, Type, InviterAccountId, Timestamp, SN) VALUES (@AccountId, @InviterCode, @Type, @InviterAccountId, @Timestamp, @SN);SELECT SCOPE_IDENTITY()";
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(sql, record);
            }
        }

        public void UpdateAccountInfo(Guid invitorId,Guid beInviteAccountId,string invitionCode,string sn)
        {
            const string sql = "UPDATE [dbo].[InviteRecords] SET [AccountId]=@AccountId,[InviterCode]=@InviterCode,[InviterAccountId]=@InviterAccountId WHERE SN=@SN";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { AccountId = beInviteAccountId, InviterCode = invitionCode, InviterAccountId = invitorId, SN = sn });
            }
        }

        public InviteRecord GetDetailByAccountId(Guid id)
        {
            var sql = "SELECT * FROM InviteRecords WHERE AccountId = @AccountId";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<InviteRecord>(sql, new { AccountId = id });
            }
        }

        public async Task<InviteRecord> GetDetailByAccountIdAsync(Guid id, InviteType inviteType)
        {
            var sql = "SELECT * FROM InviteRecords WHERE AccountId = @AccountId AND [Type]=@InviteType";
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryFirstOrDefaultAsync<InviteRecord>(sql, new { AccountId = id, InviteType = (byte)inviteType });
            }
        }

        public InviteRecord GetBySN(string sn)
        {
            var sql = "SELECT * FROM InviteRecords WHERE SN = @SN";
            using (var con = ReadConnection())
            {
                return con.Query<InviteRecord>(sql, new { SN = sn }).FirstOrDefault();
            }
        }

        public InviteRecord GetById(long id)
        {
            var sql = "SELECT * FROM InviteRecords WHERE Id = @Id";
            using (var con = ReadConnection())
            {
                return con.Query<InviteRecord>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        public List<InviteRecord> GetFiiiPosRecordsByInvitorId(Guid accountId, InviteType inviteType)
        {
            var sql = "SELECT * FROM dbo.InviteRecords WHERE InviterAccountId=@InviterAccountId AND [Type]=@SystemPlatformType";
            using (var con = ReadConnection())
            {
                return con.Query<InviteRecord>(sql, new { InviterAccountId = accountId, SystemPlatformType = inviteType }).AsList();
            }
        }


    }
}
