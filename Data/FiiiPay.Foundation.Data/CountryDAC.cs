using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class CountryDAC : BaseFoundationDataAccess
    {
        public int Insert(Country model)
        {
            using (var con = WriteConnection())
            {
                return con.ExecuteScalar<int>("INSERT INTO Countries(Name, Name_CN, PhoneCode, CustomerService, IsHot, Status) VALUES(@Name, @Name_CN, @PhoneCode, @CustomerService,@IsHot,@Status); SELECT SCOPE_IDENTITY()", model);
            }
        }

        public void Delete(Country model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM Countries WHERE @Id=@Id", model);
            }
        }

        public void DeleteById(int id)
        {
            using (var con = WriteConnection())
            {
                con.Execute("DELETE FROM Countries WHERE @Id=@Id", new { Id = id });
            }
        }

        public void Update(Country model)
        {
            using (var con = WriteConnection())
            {
                con.Execute("UPDATE Countries set Name=@Name, Name_CN=@Name_CN, PhoneCode=@PhoneCode, CustomerService=@CustomerService,IsHot=@IsHot,Status=@Status WHERE Id=@Id", model);
            }
        }

        public Country GetById(int id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Country>("SELECT * FROM Countries WHERE Id=@Id and Status=1", new { Id = id });
            }
        }

        public async Task<Country> GetByCodeAsync(string code)
        {
            using (var con = await ReadConnectionAsync())
            {
                return await con.QueryFirstOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Code=@Code", new { Code = code });
            }
        }

        public IEnumerable<Country> GetList()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Country>("SELECT * FROM Countries where Status=1 ");
            }
        }

        public IEnumerable<Country> GetCustomerList()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Country>("SELECT * FROM Countries where Status=1 and len(CustomerService)>0 ");
            }
        }
    }
}
