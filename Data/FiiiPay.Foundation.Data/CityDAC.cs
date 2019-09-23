using System.Collections.Generic;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class CityDAC : BaseFoundationDataAccess
    {
        public int Insert(City model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<int>("INSERT INTO Cities(StateId, Name) VALUES(@StateId, @Name); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public void Delete(City model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM Cities WHERE @Id=@Id", model);
            }
        }

        public void DeleteById(int id)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM Cities WHERE @Id=@Id", new { Id = id });
            }
        }

        public void Update(City model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE Cities set Name=@Name, StateId=@StateId WHERE Id=@Id", model);
            }
        }

        public void UpdateNameById(int id, string name)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE Cities set Name=@Name WHERE Id=@Id", new { Id = id, Name = name });
            }
        }

        public City GetById(int id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<City>("SELECT * FROM Cities WHERE Id=@Id", new { Id = id });
            }
        }

        public IEnumerable<City> GetList()
        {
            using (var con = ReadConnection())
            {
                return con.Query<City>("SELECT * FROM Cities");
            }
        }
    }
}
