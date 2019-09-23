using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class MerchantRecommendDAC : BaseDataAccess
    {
        public List<MerchantRecommend> GetRecommendsById(Guid id)
        {
            const string sql = "SELECT * FROM [MerchantRecommends] WHERE MerchantInformationId = @Id";

            using (var con = ReadConnection())
            {
                return con.Query<MerchantRecommend>(sql, new { Id = id }).ToList();
            }
        }

        public void Insert(MerchantRecommend recommend) 
        {
            const string sql = @"INSERT INTO [dbo].[MerchantRecommends] ([Id], [MerchantInformationId], [RecommendContent], [RecommendPicture],[Sort],[ThumbnailId])
                                      VALUES (@Id, @MerchantInformationId, @RecommendContent, @RecommendPicture,@Sort,@ThumbnailId);";
            using (var con = WriteConnection())
            {
                con.Execute(sql, recommend);
            }
        }
        /// <summary>
        /// 删除商家推荐
        /// </summary>
        /// <param name="MerchantInformationId"></param>
        public void Delete(Guid MerchantInformationId)
        {
            const string sql = @"DELETE FROM MerchantRecommends WHERE MerchantInformationId=@MerchantInformationId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new { MerchantInformationId = MerchantInformationId });
            }
        }
    }
}
