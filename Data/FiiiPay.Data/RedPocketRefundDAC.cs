using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class RedPocketRefundDAC : BaseDataAccess
    {
        public long Insert(RedPocketRefund redPocketRefund)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>(@"INSERT INTO [RedPocketRefunds]([Id],[Amount],[Timestamp],[OrderNo]) VALUES(@Id,@Amount,@Timestamp,@OrderNo); SELECT SCOPE_IDENTITY()", redPocketRefund);
            }
        }

        public RedPocketRefund GetById(long id)
        {
            using (var con = ReadConnection())
            {
                string SQL = "SELECT * FROM RedPocketRefunds WHERE Id = @Id";
                return con.QueryFirstOrDefault<RedPocketRefund>(SQL, new {Id = id});
            }
        }
    }
}
