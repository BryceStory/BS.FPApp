using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class ProfileRouterDAC : BaseFoundationDataAccess
    {
        public List<ProfileRouter> GetAll()
        {
            using (var con = ReadConnection())
            {
                return con.Query<ProfileRouter>("SELECT * FROM ProfileRouters").ToList();
            }
        }

        public string GetKYCRouter(int countryId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<string>("SELECT ServerAddress FROM ProfileRouters WHERE Country=@Id", new { Id = countryId });
            }
        }

        public ProfileRouter GetRouter(int countryId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<ProfileRouter>("SELECT * FROM ProfileRouters WHERE Country=@Id", new { Id = countryId });
            }
        }
        
        public bool DeleteById(int countryId)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("DELETE FROM ProfileRouters WHERE Country=@Country", new { Country = countryId }) > 0;
            }
        }

        public bool UpdateRouter(ProfileRouter router)
        {
            using (var con = WriteConnection())
            {
                return con.Execute("UPDATE ProfileRouters set ServerAddress=@ServerAddress, ClientKey=@ClientKey,SecretKey=@SecretKey WHERE Country=@Country", router) > 0;
            }
        }
    }
}
