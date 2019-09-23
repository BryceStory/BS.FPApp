using Dapper;
using FiiiPay.Foundation.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.Foundation.Data
{
    public class CurrenciesDAC : BaseFoundationDataAccess
    {
        public List<Currencies> GetAll()
        {
            const string SQL_STATEMENT = "SELECT * FROM [dbo].[Currencies] order by ID";

            using (var conn = ReadConnection())
            {
                return conn.Query<Currencies>(SQL_STATEMENT).ToList();
            }
        }

        public Currencies GetByID(short id)
        {
            const string SQL_STATEMENT = "SELECT * FROM [dbo].[Currencies] where ID=@ID order by ID";

            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<Currencies>(SQL_STATEMENT, new { ID = id });
            }
        }

        public Currencies GetByCode(string code)
        {
            const string SQL_STATEMENT = "SELECT * FROM [dbo].[Currencies] where Code=@Code";

            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<Currencies>(SQL_STATEMENT, new { Code = code });
            }
        }
    }
}
