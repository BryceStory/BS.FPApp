using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class RewardDistributeRecordDAC : BaseDataAccess
    {
        public long Insert(RewardDistributeRecords model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(@"INSERT INTO [dbo].[RewardDistributeRecords]([Timestamp],[OriginalReward],[Percentage],[ProfitId],[UserAccountId],[MerchantAccountId],[SN],[ActualReward]) 
    VALUES(@Timestamp,@OriginalReward,@Percentage,@ProfitId,@UserAccountId,@MerchantAccountId,@SN,@ActualReward); SELECT SCOPE_IDENTITY()", model);
            }
        }
    }
}
