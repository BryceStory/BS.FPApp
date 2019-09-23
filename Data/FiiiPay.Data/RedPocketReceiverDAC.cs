using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.Data
{
    public class RedPocketReceiverDAC : BaseDataAccess
    {
        public long Insert(RedPocketReceiver model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(@"INSERT INTO [RedPocketReceivers]([PocketId],[AccountId],[CryptoCode],[Amount],[Timestamp],[IsBestLuck],[SendAccountId],[OrderNo],[FiatAmount]) VALUES(@PocketId,@AccountId,@CryptoCode,@Amount,@Timestamp,@IsBestLuck,@SendAccountId,@OrderNo,@FiatAmount); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public RedPocketReceiver GetById(long id)
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT * FROM RedPocketReceivers WHERE Id = @Id";
                return con.QueryFirstOrDefault<RedPocketReceiver>(SQL, new { Id = id });
            }
        }

        public RedPocketReceiver GetById(Guid accountId, long id)
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT * FROM RedPocketReceivers WHERE Id = @Id AND AccountId = @AccountId";
                return con.QueryFirstOrDefault<RedPocketReceiver>(SQL, new { Id = id, AccountId = accountId });
            }
        }

        public RedPocketReceiver HasReceive(Guid accountId, long pocketId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<RedPocketReceiver>("SELECT * FROM RedPocketReceivers WHERE AccountId=@AccountId AND PocketId = @PocketId",
                    new { AccountId = accountId, PocketId = pocketId });
            }
        }

        public bool UpdateBestLuck(long pocketId)
        {
            string SQL = "update RedPocketReceivers set IsBestLuck = 1 where PocketId = @PocketId and Amount = (select Max(Amount) from RedPocketReceivers where PocketId = @PocketId)";

            using (var con = ReadConnection())
            {
                return con.Execute(SQL, new { PocketId = pocketId }) > 0;
            }
        }

        //public IEnumerable<RedPocketReceiver> GetList(long pocketId, int pageIndex, int pageSize)
        //{
        //    const string sql = @"SELECT * FROM RedPocketReceivers WHERE PocketId = @PocketId ORDER BY [Timestamp] DESC
        //                         OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

        //    using (var conn = ReadConnection())
        //    {
        //        var list = conn.Query<RedPocketReceiver>(sql, new
        //        {
        //            PocketId = pocketId,
        //            Offset = (pageIndex - 1) * pageSize,
        //            Limit = pageSize
        //        });

        //        return list;
        //    }
        //}

        public int ReceiveCount(Guid accountId)
        {
            using (var con = ReadConnection())
            {
                string SQL = $"SELECT Count(*) as PushCount FROM RedPocketReceivers WHERE AccountId = @AccountId";
                return con.Query<int>(SQL, new
                {
                    AccountId = accountId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<RedPocketReceiverES> GetList(Guid accountId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT rpr.*,ua.Nickname FROM RedPocketReceivers AS rpr
                                    LEFT JOIN UserAccounts AS ua ON rpr.SendAccountId = ua.Id
                                    WHERE rpr.AccountId = @AccountId 
                                    ORDER BY rpr.[Timestamp] DESC
                                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            using (var conn = ReadConnection())
            {
                var list = conn.Query<RedPocketReceiverES>(sql, new
                {
                    AccountId = accountId,
                    Offset = pageIndex * pageSize,
                    Limit = pageSize
                });

                return list;
            }
        }

        public IEnumerable<RedPocketReceiverES> GetList(long pocketId, int pageIndex, int pageSize)
        {
            const string sql = @"SELECT rpr.*,ua.Nickname FROM RedPocketReceivers AS rpr
                                    LEFT JOIN UserAccounts AS ua ON rpr.AccountId = ua.Id
                                    WHERE rpr.PocketId = @PocketId 
                                    ORDER BY rpr.[Timestamp] DESC
                                    OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            using (var conn = ReadConnection())
            {
                var list = conn.Query<RedPocketReceiverES>(sql, new
                {
                    PocketId = pocketId,
                    Offset = pageIndex * pageSize,
                    Limit = pageSize
                });

                return list;
            }
        }

        public decimal AccountSum(Guid accountId)
        {
            using (var con = ReadConnection())
            {
                const string SQL = "SELECT ISNULL(Sum(FiatAmount),0) as FiatAmount FROM RedPocketReceivers WHERE AccountId = @AccountId";
                return con.Query<decimal>(SQL, new
                {
                    AccountId = accountId
                }).FirstOrDefault();
            }
        }
    }
}
