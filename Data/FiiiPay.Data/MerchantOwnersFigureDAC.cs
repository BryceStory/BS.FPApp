using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class MerchantOwnersFigureDAC : BaseDataAccess
    {
        public List<MerchantOwnersFigure> GetOwnersFiguresById(Guid id)
        {
            const string sql = "SELECT * FROM [MerchantOwnersFigures] WHERE MerchantInformationId = @Id";

            using (var con = ReadConnection())
            {
                return con.Query<MerchantOwnersFigure>(sql, new { Id = id }).ToList();
            }
        }

        public void Insert(MerchantOwnersFigure ownersfigure)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantOwnersFigures] ([MerchantInformationId], [FileId],[Sort],[ThumbnailId])
                                      VALUES (@MerchantInformationId, @FileId,@Sort,@ThumbnailId);";
            using (var con = WriteConnection())
            {
                con.Execute(sql, ownersfigure);
            }
        }
        public async Task InsertAsync(MerchantOwnersFigure ownersfigure)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantOwnersFigures] ([MerchantInformationId], [FileId],[Sort],[ThumbnailId])
                                      VALUES (@MerchantInformationId, @FileId,@Sort,@ThumbnailId);";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, ownersfigure);
            }
        }
        /// <summary>
        /// 删除商家主图
        /// </summary>
        /// <param name="MerchantInformationId"></param>
        public void Delete(Guid MerchantInformationId)
        {
            const string sql = @"DELETE FROM MerchantOwnersFigures WHERE MerchantInformationId=@MerchantInformationId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { MerchantInformationId = MerchantInformationId });
            }
        }
    }
}
