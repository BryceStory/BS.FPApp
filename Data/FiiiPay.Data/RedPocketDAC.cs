using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;

namespace FiiiPay.Data
{
    public class RedPocketDAC : BaseDataAccess
    {
        public long Insert(RedPocket model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(@"INSERT INTO RedPockets(AccountId,CryptoCode,Amount,Balance,Count,RemainCount,Message,Timestamp,PassCode,Status,ExpirationDate,CryptoId,OrderNo,FiatAmount) VALUES(@AccountId,@CryptoCode,@Amount,@Balance,@Count,@RemainCount,@Message,@Timestamp,@PassCode,@Status,@ExpirationDate,@CryptoId,@OrderNo,@FiatAmount); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public RedPocket GetById(long id)
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT * FROM RedPockets WHERE Id = @Id";
                return con.QueryFirstOrDefault<RedPocket>(SQL, new { Id = id });
            }
        }

        public RedPocket GetByPassCode(string passcode)
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT * FROM RedPockets WHERE PassCode = @PassCode";
                return con.QueryFirstOrDefault<RedPocket>(SQL, new { PassCode = passcode });
            }
        }

        public bool UpdateRemain(long id, decimal balance)
        {
            string strSql = @"UPDATE [dbo].[RedPockets]
                             SET [RemainCount] = [RemainCount] - 1,[Balance] = [Balance] - @DBalance
                             WHERE Id=@Id";

            using (var con = WriteConnection())
            {
                return con.Execute(strSql, new { DBalance = balance, Id = id }) > 0;
            }
        }

        public bool UpdateStatus(long id, RedPocketStatus status)
        {
            string strSql = @"UPDATE [dbo].[RedPockets]
                             SET Status = @Status
                             WHERE Id=@Id";

            using (var con = WriteConnection())
            {
                return con.Execute(strSql, new { Status = status, Id = id }) > 0;
            }
        }
        
        public int PushCount(Guid accountId)
        {
            using (var con = ReadConnection())
            {
                const string SQL = "SELECT Count(*) as PushCount FROM RedPockets WHERE AccountId = @AccountId";
                return con.Query<int>(SQL, new
                {
                    AccountId = accountId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<RedPocketListES> GetList(Guid accountId, int pageIndex, int pageSize)
        {
            using (var con = ReadConnection())
            {
                var SQL = @"SELECT rp.*,rpr.Amount as RefundAmount FROM RedPockets AS rp
                                LEFT JOIN RedPocketRefunds AS rpr ON rp.Id = rpr.Id
                                WHERE rp.AccountId = @AccountId
                                ORDER BY rp.[Timestamp] DESC
                                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

                return con.Query<RedPocketListES>(SQL, new
                {
                    AccountId = accountId,
                    Offset = pageIndex * pageSize,
                    Limit = pageSize
                });
            }
        }

        public decimal AccountSum(Guid accountId)
        {
            using (var con = ReadConnection())
            {
                const string SQL = "SELECT ISNULL(Sum(FiatAmount),0) as FiatAmount FROM RedPockets WHERE AccountId = @AccountId";
                return con.Query<decimal>(SQL, new
                {
                    AccountId = accountId
                }).FirstOrDefault();
            }
        }
    }
}
