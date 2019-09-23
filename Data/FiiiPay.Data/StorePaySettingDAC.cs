using Dapper;
using FiiiPay.Entities;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class StorePaySettingDAC : BaseDataAccess
    {
        public async Task<StorePaySetting> GetByCountryIdAsync(int countryId)
        {
            const string sql = "SELECT * FROM [dbo].[StorePaySettings] WHERE CountryId=@CountryId";
            using (var con = await ReadConnectionAsync())
            {
                return await con.QuerySingleOrDefaultAsync<StorePaySetting>(sql, new { CountryId = countryId });
            }
        }
    }
}
