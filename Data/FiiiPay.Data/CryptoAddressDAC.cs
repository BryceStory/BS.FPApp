using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class CryptoAddressDAC : BaseDataAccess
    {
        public List<CryptoAddress> GetByAccountId(Guid accountId)
        {
            const string sql = @"SELECT * FROM [dbo].[CryptoAddresses] WHERE AccountId=@AccountId";
            using (var con = ReadConnection())
            {
                return con.Query<CryptoAddress>(sql, new { AccountId = accountId }).ToList();
            }
        }

        public List<CryptoAddress> GetByCryptoId(Guid accountId, int cryptoId)
        {
            const string sql = @"SELECT * FROM [dbo].[CryptoAddresses] WHERE AccountId=@AccountId AND CryptoId=@CryptoId";
            using (var con = ReadConnection())
            {
                return con.Query<CryptoAddress>(sql, new { AccountId = accountId, CryptoId = cryptoId }).ToList();
            }
        }

        public long Insert(CryptoAddress entities)
        {
            const string sql = @"INSERT INTO [dbo].[CryptoAddresses]([AccountId],[AccountType],[CryptoId],[Alias],[Address],[Tag])
                                      VALUES (@AccountId,@AccountType,@CryptoId,@Alias,@Address,@Tag); SELECT SCOPE_IDENTITY()";
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(sql, entities);
            }
        }

        public void Update(CryptoAddress enities)
        {
            const string sql = @"UPDATE [dbo].[CryptoAddresses]
                                    SET [Alias] = @Alias,
                                        [Address] = @Address
                                  WHERE [Id]=@Id";

            using (var con = WriteConnection())
            {
                con.Execute(sql, enities);
            }
        }

        public void Delete(long id)
        {
            const string sql = @"DELETE FROM [dbo].[CryptoAddresses] WHERE Id=@Id"; 
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { Id = id });
            }
        }

        public CryptoAddress GetById(long id)
        {
            const string sql = @"SELECT * FROM [dbo].[CryptoAddresses] WHERE Id=@Id";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<CryptoAddress>(sql, new { Id = id });
            }
        }
    }
}