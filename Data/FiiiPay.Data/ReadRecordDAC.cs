using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class ReadRecordDAC : BaseDataAccess
    {
        public long Insert(ReadRecord model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<long>("INSERT INTO ReadRecords(Type, AccountId, TargetId) VALUES(@Type, @AccountId, @TargetId); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public bool Exists(ReadRecord model)
        {
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>("SELECT * FROM ReadRecords WHERE Type = @Type AND AccountId = @AccountId AND TargetId = @TargetId", model) > 0;
            }
        }
    }
}
