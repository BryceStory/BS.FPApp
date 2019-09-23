using System.Collections.Generic;
using Dapper;
using FiiiPay.Entities;
using System;
using FiiiPay.Entities.EntitySet;

namespace FiiiPay.Data
{
    public class ArticleDAC : BaseDataAccess
    {
        public int Insert(Article model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<int>("INSERT INTO Articles(Title,Type,Body,CreateTime,UpdateTime,ShouldPop,AccountType) VALUES(@Title,@Type,@Body,@CreateTime,@UpdateTime,@ShouldPop,@AccountType); SELECT SCOPE_IDENTITY()", model);
            }
        }
        //public int BatchUpdate(List<PriceInfo> model)
        //{
        //    using (var con = WriteConnection())
        //    {
        //        return con.ExecuteScalar<int>("INSERT INTO Articles(Title,Type,Body,CreateTime,UpdateTime,ShouldPop,AccountType) VALUES(@Title,@Type,@Body,@CreateTime,@UpdateTime,@ShouldPop,@AccountType); SELECT SCOPE_IDENTITY()", model);
        //    }
        //}
        public void Delete(Article model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM Articles WHERE @Id=@Id", model);
            }
        }

        public void DeleteById(int id)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM Articles WHERE @Id=@Id", new { Id = id });
            }
        }

        public void Update(Article model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE Title=@Title,Type=@Type,Body=@Body,CreateTime=@CreateTime,UpdateTime=@UpdateTime,ShouldPop=@ShouldPop,AccountType=@AccountType WHERE Id=@Id", model);
            }
        }

        public Article GetById(int id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Article>("SELECT * FROM Articles WHERE Id=@Id", new { Id = id });
            }
        }

        public IEnumerable<Article> GetList()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Article>("SELECT * FROM Articles");
            }
        }

        /// <summary>
        /// 分页获取公告列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex">从1开始</param>
        /// <param name="articleAccountType"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public List<SystemMessageES> List(int pageSize, int pageIndex, ArticleAccountType articleAccountType, Guid accountId)
        {
            using (var con = ReadConnection())
            {
                return con.Query<SystemMessageES>($@"SELECT g.* FROM (
SELECT a.Id, 0 as Type, a.Title, a.Body, a.CreateTime, NULL AS Attach, CASE WHEN b.Id IS NULL THEN 0 ELSE 1 END AS [Read] FROM Articles a
LEFT JOIN (
SELECT c.* FROM ReadRecords c WHERE c.AccountId = @AccountId) b
ON b.TargetId = a.Id AND b.Type = 0
WHERE a.AccountType = @articleAccountType
) g
ORDER BY g.[Read], g.CreateTime DESC OFFSET {pageSize * (pageIndex - 1)} ROWS FETCH NEXT {pageSize} ROWS ONLY", new { articleAccountType, AccountId = accountId }).AsList();
            }
        }

        /// <summary>
        /// 分页获取公告列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex">从1开始</param>
        /// <param name="articleAccountType"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public List<SystemMessageES> AllList(ArticleAccountType articleAccountType, Guid accountId)
        {
            using (var con = ReadConnection())
            {
                return con.Query<SystemMessageES>($@"select g.* from (
SELECT a.Id, 0 as Type, a.Title, a.Body, a.CreateTime, NULL AS Attach, CASE WHEN b.Id IS NULL THEN 0 ELSE 1 END AS [Read] FROM Articles a
LEFT JOIN (
SELECT c.* FROM ReadRecords c WHERE c.AccountId = @AccountId) b
ON b.TargetId = a.Id AND b.Type = 0
WHERE a.AccountType = @articleAccountType) g where g.[Read] = 0 ", new { articleAccountType, AccountId = accountId }).AsList();
            }
        }

        public Tuple<SystemMessageES, int> GetFirstTitleAndNotReadCount(ArticleAccountType articleAccountType, Guid accountId)
        {
            using (var con = ReadConnection())
            {
                var model = con.QueryFirstOrDefault<SystemMessageES>($@"SELECT TOP 1 g.* FROM (
SELECT a.Id, 0 as Type, a.Title, a.Body, a.CreateTime, NULL AS Attach, CASE WHEN b.Id IS NULL THEN 0 ELSE 1 END AS [Read] FROM Articles a
LEFT JOIN (
SELECT c.* FROM ReadRecords c WHERE c.AccountId = @AccountId) b
ON b.TargetId = a.Id AND b.Type = 0
WHERE a.AccountType = @articleAccountType
) g
ORDER BY g.[Read], g.CreateTime DESC", new { articleAccountType, AccountId = accountId });
                var count = con.ExecuteScalar<int>(@"SELECT COUNT(*) FROM (
SELECT a.Id, 0 as Type, a.Title, a.Body, a.CreateTime, NULL AS Attach, CASE WHEN b.Id IS NULL THEN 0 ELSE 1 END AS [Read] FROM Articles a
LEFT JOIN (
SELECT c.* FROM ReadRecords c WHERE c.AccountId = @AccountId) b
ON b.TargetId = a.Id AND b.Type = 0
WHERE a.AccountType = @articleAccountType
) g WHERE g.[Read] = 0", new { articleAccountType, AccountId = accountId });
                return new Tuple<SystemMessageES, int>(model, count);
            }
        }

        public int GetUnreadCount(ArticleAccountType articleAccountType, Guid accountId)
        {
            const string sql =
                @"SELECT COUNT(1) FROM (
                      (SELECT * FROM Articles WHERE AccountType=@articleAccountType) a 
                       LEFT JOIN 
                      (SELECT distinct TargetId FROM ReadRecords WHERE AccountId=@accountId) b
                       ON a.Id = b.TargetId
                    ) WHERE b.TargetId is null";

            using (var con = ReadConnection())
            {
                var count = con.ExecuteScalar<int>(sql, new { articleAccountType, AccountId = accountId });
                return count;

            }

        }
    }
}
