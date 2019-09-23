using Dapper;
using FiiiPay.Entities;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class FiiiPayMerchantProfileDAC : BaseDataAccess
    {
        public async Task InsertAsync(FiiipayMerchantProfile profile)
        {
            const string sql = @"INSERT INTO [dbo].[FiiipayMerchantProfiles]([MerchantInfoId],[LicenseNo],[BusinessLicenseImage])
                                      VALUES (@MerchantInfoId,@LicenseNo,@BusinessLicenseImage)";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, profile);
            }
        }
    }
}
