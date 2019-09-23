using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class FiiiFinanceRouterDAC : BaseFoundationDataAccess
    {
        public FiiiFinanceRouter GetRouter(int countryId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<FiiiFinanceRouter>("SELECT * FROM FiiiFinanceRouters WHERE CountryId=@CountryId", new { CountryId = countryId });
            }
        }

        public List<FiiiFinanceRouter> GetAll()
        {
            using (var con = ReadConnection())
            {
                return con.Query<FiiiFinanceRouter>("SELECT * FROM FiiiFinanceRouters").ToList();
            }
        }
    }
}