using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class BillerAddressDAC : BaseDataAccess
    {
        public void Insert(BillerAddress address)
        {
            const string sql =
                @"INSERT INTO [dbo].[BillerAddresses] ([AccountId],[BillerCode],[ReferenceNumber],[Tag],[IconIndex],[Timestamp])
                       VALUES (@AccountId,@BillerCode,@ReferenceNumber,@Tag,@IconIndex,@Timestamp);";
            using (var con = WriteConnection())
            {
                con.Execute(sql, address);
            }
        }
        public List<BillerAddress> GetAllAddresses(Guid accountId)
        {
            const string sql = @"SELECT * FROM [dbo].[BillerAddresses] WHERE AccountId = @AccountId";
            using (var con = ReadConnection())
            {
                return con.Query<BillerAddress>(sql,
                    new { AccountId = accountId}).ToList();
            }
        }
        public List<BillerAddress> GetAddresses(Guid accountId, int pageSize, int pageIndex)
        {
            const string sql = @"SELECT * FROM [dbo].[BillerAddresses] WHERE AccountId = @AccountId ORDER BY [Timestamp] DESC OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var con = ReadConnection())
            {
                return con.Query<BillerAddress>(sql,
                    new {AccountId = accountId, Offset = pageIndex * pageSize, Limit = pageSize}).ToList();
            }
        }
        public List<BillerAddress> GetAddressesByIconIndex(Guid accountId, string iconIndex, int pageSize, int pageIndex)
        {
            const string sql = @"SELECT * FROM [dbo].[BillerAddresses] WHERE AccountId = @AccountId AND IconIndex = @IconIndex ORDER BY [Timestamp] DESC OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            using (var con = ReadConnection())
            {
                return con.Query<BillerAddress>(sql, new { AccountId = accountId, IconIndex = iconIndex, Offset = pageIndex * pageSize, Limit = pageSize }).ToList();
            }
        }


        public void Delete(long id)
        {
            const string sql =
                @"DELETE FROM [dbo].[BillerAddresses] WHERE Id = @Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new {Id = id});
            }
        }

        public void Update(BillerAddress address)
        {
            const string sql =
                @"UPDATE [dbo].[BillerAddresses] SET [BillerCode] = @BillerCode ,[ReferenceNumber] = @ReferenceNumber ,[Tag] = @Tag ,[Timestamp] = @Timestamp ,[IconIndex] = @IconIndex WHERE Id = @Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, address);
            }
        }
    }
}
