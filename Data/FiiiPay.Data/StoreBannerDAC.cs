using Dapper;
using FiiiPay.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class StoreBannerDAC : BaseDataAccess
    {
        public async Task<List<StoreBanner>> GetAliveStoreBannerAsync(int countryId)
        {
            using (var con = await ReadConnectionAsync())
            {
                var querylist = await con.QueryAsync<StoreBanner>("SELECT * FROM [dbo].[StoreBanners] WHERE [Status]=@Status AND ([CountryId]=@CountryId OR [CountryId]=0) ORDER BY [Sort],[StartTime]"
                    , new { Status = (byte)BannerStatus.Active, CountryId = countryId });
                return querylist.ToList();
            }
        }
    }
}
