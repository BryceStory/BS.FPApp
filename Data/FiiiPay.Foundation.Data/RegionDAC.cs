using Dapper;
using FiiiPay.Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Foundation.Data
{
    public class RegionDAC : BaseFoundationDataAccess
    {
        public Regions GetById(long id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Regions>("SELECT * FROM Regions WHERE Id=@Id", new { Id = id });
            }
        }

        public async Task<Regions> GetByIdAsync(long id)
        {
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryFirstOrDefaultAsync<Regions>("SELECT * FROM Regions WHERE Id=@Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<Regions>> GetStateListAsync(int countryId)
        {
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryAsync<Regions>("SELECT * FROM Regions Where RegionLevel=@RegionLevel AND CountryId=@CountryId", new { RegionLevel = (byte)RegionLevel.State, CountryId = countryId });
            }
        }

        public async Task<IEnumerable<Regions>> GetListAsync(long parentId, RegionLevel currentLevel)
        {
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryAsync<Regions>("SELECT * FROM Regions Where RegionLevel=@RegionLevel AND ParentId=@ParentId", new { RegionLevel = (byte)currentLevel, ParentId = parentId });
            }
        }
    }
}
