using Dapper;
using FiiiPay.Entities;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class AdvertisingDAC : BaseDataAccess
    {
        public async Task<Advertising> GetActiveSingle()
        {
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryFirstOrDefaultAsync<Advertising>("SELECT top 1 * FROM Advertisings WHERE Status=@Status and  GETUTCDATE() between StartDate and EndDate order by CreateTime desc", new { Status = true });
            }
        }
    }
}
