using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class StoreTypeDAC : BaseDataAccess
    {
        public List<StoreType> GetList()
        {
            var sql = "SELECT * FROM [StoreTypes]";
            using (var con = ReadConnection())
            {
                return con.Query<StoreType>(sql).ToList();
            }
        }
    }
}
