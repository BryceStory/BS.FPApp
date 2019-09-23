using Dapper;
using FiiiPay.Entities;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class FiiipayMerchantVerifyRecordDAC : BaseDataAccess
    {
        public async Task InsertAsync(FiiipayMerchantVerifyRecord record)
        {
            const string sql = @"INSERT INTO [dbo].[FiiipayMerchantVerifyRecords]([CreateTime],[MerchantInfoId],[LicenseNo],[BusinessLicenseImage],[VerifyStatus],[VerifyTime],[Auditor],[Message])
                                      VALUES (@CreateTime,@MerchantInfoId,@LicenseNo,@BusinessLicenseImage,@VerifyStatus,@VerifyTime,@Auditor,@Message)";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, record);
            }
        }

        public void Insert(FiiipayMerchantVerifyRecord record)
        {
            const string sql = @"INSERT INTO [dbo].[FiiipayMerchantVerifyRecords]([CreateTime],[MerchantInfoId],[LicenseNo],[BusinessLicenseImage],[VerifyStatus],[VerifyTime],[Auditor],[Message])
                                      VALUES (@CreateTime,@MerchantInfoId,@LicenseNo,@BusinessLicenseImage,@VerifyStatus,@VerifyTime,@Auditor,@Message)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, record);
            }
        }

        public async Task<FiiipayMerchantVerifyRecord> GetByIdAsync(long id)
        {
            const string sql = @"SELECT * FROM dbo.FiiipayMerchantVerifyRecords WHERE Id=@RecordId";
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryFirstAsync<FiiipayMerchantVerifyRecord>(sql, new { RecordId = id });
            }
        }
    }
}
