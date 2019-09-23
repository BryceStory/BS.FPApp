using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class VerifyRecordDAC : BaseDataAccess
    {
        public long Insert(VerifyRecord model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>("INSERT INTO VerifyRecords(AccountId, Type, Body, CreateTime) VALUES(@AccountId, @Type, @Body , @CreateTime); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public VerifyRecord GetById(long id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<VerifyRecord>("SELECT * FROM VerifyRecords WHERE Id=@Id", new { Id = id });
            }
        }
    }
}
