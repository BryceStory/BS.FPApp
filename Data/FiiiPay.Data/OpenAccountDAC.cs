using FiiiPay.Entities;
using System;
using Dapper;

namespace FiiiPay.Data
{
    public class OpenAccountDAC : BaseDataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fiiiType">0FiiiPay 1FiiiPos</param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public OpenAccount GetOpenAccount(FiiiType fiiiType, Guid accountId)
        {
            using (var con = ReadConnection())
            {
                return con.QuerySingleOrDefault<OpenAccount> ("SELECT * FROM [dbo].[OpenAccount] WHERE FiiiType=@FiiiType AND AccountId=@AccountId", new { FiiiType = fiiiType, AccountId = accountId });
            }
        }

        public OpenAccount GetOpenAccount(Guid openId)
        {
            using (var con = ReadConnection())
            {
                return con.QuerySingleOrDefault<OpenAccount>("SELECT * FROM [dbo].[OpenAccount] WHERE OpenId=@OpenId", new { OpenId = openId });
            }
        }

        public long Create(OpenAccount account)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(@"INSERT INTO [dbo].[OpenAccount]([CreateTime],[PlatformId],[FiiiType],[OpenId],[SecretKey],[AccountId]) VALUES (@CreateTime,@PlatformId,@FiiiType,@OpenId,@SecretKey,@AccountId); SELECT SCOPE_IDENTITY()", account);
            }
        }
    }
}
