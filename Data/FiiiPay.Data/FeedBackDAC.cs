using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class FeedBackDAC : BaseDataAccess
    {
        public void Insert(Feedback feedback)
        {
            const string sql = @"INSERT INTO [dbo].[Feedbacks]([Type],[AccountId],[Context],[HasProcessor],[Timestamp])
     VALUES (@Type,@AccountId,@Context,@HasProcessor,@Timestamp)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, feedback);
            }
        }
    }
}