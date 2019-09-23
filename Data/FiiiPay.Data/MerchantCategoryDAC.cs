using Dapper;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
   public class MerchantCategoryDAC:BaseDataAccess
    {
        public void Insert(MerchantCategory category)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantCategorys] ([MerchantInformationId], [Category])
                                      VALUES (@MerchantInformationId, @Category);";
            using (var con = WriteConnection())
            {
                con.Execute(sql, category);
            }
        }
        public async Task InsertAsync(MerchantCategory category)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantCategorys] ([MerchantInformationId], [Category])
                                      VALUES (@MerchantInformationId, @Category);";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, category);
            }
        }
        public List<MerchantCategory> GetByMerchantInformationId(Guid merchantInformationId)
        {
            using (var con = ReadConnection())
            {
                return con.Query<MerchantCategory>("SELECT * FROM [MerchantCategorys] WHERE [MerchantInformationId] = @MerchantInformationId", new { MerchantInformationId = merchantInformationId }).ToList() ;
            }
        }

        public void Delete(Guid MerchantInformationId)
        {
            const string sql = @"DELETE FROM MerchantCategorys WHERE MerchantInformationId=@MerchantInformationId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { MerchantInformationId = MerchantInformationId});
            }
        }
    }
}
