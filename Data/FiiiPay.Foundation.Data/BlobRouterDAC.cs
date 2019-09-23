using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class BlobRouterDAC : BaseFoundationDataAccess
    {
        public BlobRouter GetRouter(int countryId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<BlobRouter>("SELECT * FROM BlobRouters WHERE CountryId=@CountryId", new { CountryId = countryId });
            }
        }
    }
}