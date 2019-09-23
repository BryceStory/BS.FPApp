using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class CryptocurrencyDAC : BaseFoundationDataAccess
    {
        public List<Cryptocurrency> GetByIds(int[] ids)
        {
            using (var con = ReadConnection())
            {
                return con.Query<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE Id in (" + string.Join(",", ids) + ")")
                                        .ToList();
            }
        }

        public Cryptocurrency GetById(int id)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE Id = @id",
                                                new { id });
            }
        }

        public List<Cryptocurrency> List()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE [Enable]=1").ToList();
            }
        }

        public List<Cryptocurrency> GetAll()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Cryptocurrency>("SELECT * FROM Cryptocurrencies").ToList();
            }
        }

        public async Task<List<Cryptocurrency>> GetAllAsync()
        {
            using (var con = await ReadConnectionAsync())
            {
                return (await con.QueryAsync<Cryptocurrency>("SELECT * FROM Cryptocurrencies")).ToList();
            }
        }

        public List<Cryptocurrency> GetAllActived()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE [Enable]=1").ToList();
            }
        }
        public List<Cryptocurrency> GetActivedNotWhite()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE [Enable]=1 and IsWhiteLabel=0").ToList();
            }
        }

        public List<Cryptocurrency> GetActivedWhite()
        {
            using (var con = ReadConnection())
            {
                return con.Query<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE [Enable]=1 and IsWhiteLabel=1").ToList();
            }
        }


        public Cryptocurrency GetByCode(string cryptoCode)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<Cryptocurrency>("SELECT * FROM Cryptocurrencies WHERE Code=@Code",
                    new { Code = cryptoCode });
            }
        }
    }
}