using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.Data
{
    public class ProfitDetailDAC : BaseDataAccess
    {
        public long Insert(ProfitDetail im)
        {
            var sql = "INSERT INTO ProfitDetails (InvitationId, CryptoAmount, AccountId, Type,Status, Timestamp, OrderNo, CryptoId,CryptoCode) VALUES (@InvitationId, @CryptoAmount, @AccountId, @Type,@Status, @Timestamp, @OrderNo, @CryptoId,@CryptoCode);SELECT SCOPE_IDENTITY()";
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(sql, im);
            }
        }
        /// <summary>
        /// 获取邀请人数
        /// </summary>
        /// <returns></returns>
        public int GetInvitedCount(UserAccount account)
        {
            var sql = "SELECT count(1) FROM [dbo].[ProfitDetails] WHERE AccountId = @AccountId AND Type = @Type";
            using (var con = WriteConnection())
            {
                return con.Query<int>(sql, new { AccountId = account.Id, Type = ProfitType.InvitePiiiPay }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取邀请并发放奖励人数
        /// </summary>
        /// <returns></returns>
        public int GetInvitedAndActiveCount(Guid accountId)
        {
            var sql = "SELECT count(1) FROM [dbo].[ProfitDetails] WHERE AccountId = @AccountId AND Type = @Type AND Status = @Status";
            using (var con = WriteConnection())
            {
                return con.Query<int>(sql, new { AccountId = accountId, Type = ProfitType.InvitePiiiPay, Status = InviteStatusType.IssuedActive }).FirstOrDefault();
            }
        }

        public Guid GetAccountId(long id)
        {
            var sql = "SELECT AccountId FROM [dbo].[ProfitDetails] WHERE Id = @Id";
            using (var con = WriteConnection())
            {
                return con.Query<Guid>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        public decimal GetTotalReward(UserAccount account)
        {
            var sql = $"SELECT ISNULL(SUM(CryptoAmount),0) AS Amount FROM ProfitDetails WHERE Type != {(int)ProfitType.InvitePiiiPos}";
            using (var con = WriteConnection())
            {
                return con.Query<decimal>(sql, new { AccountId = account.Id }).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取邀请fiiipos的分红记录
        /// </summary>
        /// <param name="id">invitationid</param>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<ProfitDetail> GetFiiiposBonusRecords(int id, UserAccount account)
        {
            var sql = "SELECT * FROM ProfitDetails WHERE AccountId = @AccountId AND InvitationId = @InvitationId ORDER BY Timestamp DESC";

            using (var con = WriteConnection())
            {
                return con.Query<ProfitDetail>(sql, new { AccountId = account.Id, InvitationId = id }).ToList();
            }
        }

        public ProfitDetail GetBonusDetailById(int id)
        {
            var sql = "SELECT * FROM ProfitDetails WHERE Id = @Id";
            using (var con = WriteConnection())
            {
                return con.Query<SingleBonusDetailES>(sql, new { Id = id }).First();
            }
        }

        public decimal GetUserBonusTotalAmount(UserAccount account, int platform)
        {
            var str = platform == 1 
                ? $"Type != {(int)ProfitType.InvitePiiiPos}" 
                : $"Type = {(int)ProfitType.InvitePiiiPos}";
            var sql = $"SELECT ISNULL(SUM(CryptoAmount),0) FROM ProfitDetails WHERE {str} AND AccountId = @AccountId";
            using (var con = WriteConnection())
            {
                return con.Query<decimal>(sql, new { AccountId = account.Id }).First();
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        public void UpdateStatus(long id, InviteStatusType status)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE ProfitDetails SET Status = @status WHERE Id = @id",
                    new { id, status });
            }
        }
    }
}
