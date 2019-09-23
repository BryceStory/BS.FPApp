using Dapper;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class MerchantSupportCryptoDAC:BaseDataAccess
    {
        public IEnumerable<MerchantSupportCrypto> GetList(Guid merchantInfoId)
        {
            using (var con = ReadConnection())
            {
                return con.Query<MerchantSupportCrypto>("SELECT * FROM [MerchantSupportCryptos] WHERE [MerchantInfoId] = @MerchantInfoId", new { MerchantInfoId = merchantInfoId });
            }
        }
        public void Insert(MerchantSupportCrypto supportCrypto)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantSupportCryptos]([MerchantInfoId],[CryptoId],[CryptoCode])
                                      VALUES (@MerchantInfoId,@CryptoId,@CryptoCode)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, supportCrypto);
            }
        }
        public async Task InsertAsync(MerchantSupportCrypto supportCrypto)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantSupportCryptos]([MerchantInfoId],[CryptoId],[CryptoCode])
                                      VALUES (@MerchantInfoId,@CryptoId,@CryptoCode)";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, supportCrypto);
            }
        }
    }
}
