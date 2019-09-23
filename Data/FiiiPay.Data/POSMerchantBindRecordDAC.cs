using Dapper;
using FiiiPay.Entities;
using System;
using System.Linq;

namespace FiiiPay.Data
{
    public class POSMerchantBindRecordDAC : BaseDataAccess
    {
        public int Insert(POSMerchantBindRecord record)
        {
            var sql = "INSERT INTO [POSMerchantBindRecords] ([POSId],[SN],[MerchantId],[MerchantUsername],[BindTime],[BindStatus]) VALUES (@POSId,@SN,@MerchantId,@MerchantUsername,@BindTime,@BindStatus);SELECT SCOPE_IDENTITY()";
            using (var con = ReadConnection())
            {
                return con.ExecuteScalar<int>(sql, record);
            }
        }

        public POSMerchantBindRecord GetById(long id)
        {
            var sql = "SELECT * FROM [POSMerchantBindRecords] WHERE Id = @Id";
            using (var con = ReadConnection())
            {
                return con.Query<POSMerchantBindRecord>(sql, new {Id = id}).FirstOrDefault();
            }
        }

        public POSMerchantBindRecord GetByMerchantId(Guid id)
        {
            var sql = "SELECT * FROM [POSMerchantBindRecords] WHERE MerchantId = @Id";
            using (var con = ReadConnection())
            {
                return con.Query<POSMerchantBindRecord>(sql, new { Id = id }).FirstOrDefault();
            }
        }
        public POSMerchantBindRecord GetBySn(string sn)
        {
            var sql = "SELECT * FROM [POSMerchantBindRecords] WHERE SN = @SN";
            using (var con = ReadConnection())
            {
                return con.Query<POSMerchantBindRecord>(sql, new { SN = sn }).FirstOrDefault();
            }
        }

        public void UnbindRecord(Guid merchantId,long posId)
        {
            const string sql = "UPDATE [POSMerchantBindRecords] SET [BindStatus]=@BindStatus,[UnbindTime]=@UnbindTime WHERE MerchantId=@MerchantId AND POSId=@POSId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { BindStatus = 0, UnbindTime = DateTime.UtcNow, MerchantId = merchantId, POSId = posId });
            }
        }
    }
}
