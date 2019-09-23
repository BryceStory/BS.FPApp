using System.Collections.Generic;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class StateDAC : BaseDataAccess
    {
        public int Insert(State model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<int>("INSERT INTO States(CountryId, Name) VALUES(@CountryId, @Name); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public void Delete(State model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM States WHERE @Id=@Id", model);
            }
        }

        public void DeleteById(int id)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM States WHERE @Id=@Id", new { Id = id });
            }
        }

        public void Update(State model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE States set Name=@Name, CountryId=@CountryId WHERE Id=@Id", model);
            }
        }

        public void UpdateNameById(int id, string name)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE States set Name=@Name WHERE Id=@Id", new { Id = id, Name = name });
            }
        }

        public State GetById(int id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<State>("SELECT * FROM States WHERE Id=@Id", new { Id = id });
            }
        }

        public IEnumerable<State> GetList()
        {
            using (var con = ReadConnection())
            {
                return con.Query<State>("SELECT * FROM States");
            }
        }
    }
}
