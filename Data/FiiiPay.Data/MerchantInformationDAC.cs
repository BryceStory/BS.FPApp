using Dapper;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace FiiiPay.Data
{
    public class MerchantInformationDAC : BaseDataAccess
    {
        public List<MerchantBriefInformation> QueryNearbyMerchantList(decimal lat, decimal lng, int countryId, int pageSize, int pageIndex, string keyWord = "", int category = -1)
        {
            dynamic model = new ExpandoObject();

            model.CountryId = countryId;

            var catagorys = "LEFT JOIN[MerchantCategorys] c ON a.Id = c.MerchantInformationId";

            string sql = $"SELECT * FROM (SELECT a.MerchantName,a.Id,a.IsAllowExpense,a.FileId,a.ThumbnailId, a.Tags, a.Lat, a.Lng, (geography::Point({lat},{lng},4326).STDistance(geography::Point(ISNULL(a.Lat,0), ISNULL(a.Lng, 0), 4326))) as Distance, a.Address, a.Id as MerchantInformationId,a.AccountType, a.MerchantAccountId FROM [MerchantInformations] a {(category == -1 ? string.Empty : catagorys)} WHERE a.Status = 1 AND a.VerifyStatus = 1 AND a.IsPublic=1 AND a.CountryId = @CountryId ";

            if (!string.IsNullOrEmpty(keyWord))
            {
                sql += "AND (a.Tags LIKE '%'+@KeyWord+'%' OR a.MerchantName LIKE '%'+@KeyWord+'%') ";
                model.KeyWord = keyWord;
            }

            if (category != -1)
            {
                sql += "AND c.Category = @Category ";
                model.Category = category;
            }

            sql += $") d ORDER BY Distance OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using (var con = ReadConnection())
            {
                return con.Query<MerchantBriefInformation>(sql, (object)model).ToList();
            }
        }

        public List<MerchantBriefInformation> QueryMerchantListByCountryId(int countryId, int pageIndex, int pageSize, string keyWord = "", int category = -1)
        {
            dynamic model = new ExpandoObject();

            model.CountryId = countryId;

            var catagorys = "LEFT JOIN[MerchantCategorys] c ON a.Id = c.MerchantInformationId";

            string sql = $"SELECT * FROM (SELECT a.MerchantName,a.Id,a.IsAllowExpense,a.FileId,a.ThumbnailId, a.Tags, a.Lat, a.Lng,  a.Address, a.Id as MerchantInformationId,a.AccountType, a.MerchantAccountId FROM [MerchantInformations] a {(category == -1 ? string.Empty : catagorys)} WHERE a.Status = 1 AND a.VerifyStatus = 1 AND IsPublic=1 AND a.CountryId = @CountryId ";

            if (!string.IsNullOrEmpty(keyWord))
            {
                sql += "AND (a.Tags LIKE '%'+@KeyWord+'%' OR a.MerchantName LIKE '%'+@KeyWord+'%') ";
                model.KeyWord = keyWord;
            }

            if (category != -1)
            {
                sql += "AND c.Category = @Category ";
                model.Category = category;
            }

            sql += $") d ORDER BY Lat OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            using (var con = ReadConnection())
            {
                return con.Query<MerchantBriefInformation>(sql, (object)model).ToList();
            }
        }

        public List<MerchantBriefInformation> QueryNearbyMerchantList(decimal leftTopX, decimal leftTopY, decimal rightDownX, decimal rightDownY)
        {
            string sql;
            if (rightDownX < leftTopX)
            {
                sql = $"SELECT TOP 20 a.MerchantName,a.Id,a.IsAllowExpense,a.FileId,a.ThumbnailId, a.Tags, a.Lat, a.Lng, 0 as Distance, a.Address, a.Id as MerchantInformationId,a.AccountType, a.MerchantAccountId FROM [MerchantInformations] a WHERE a.Status = 1 AND a.VerifyStatus = 1 AND IsPublic=1 AND a.Lat <= @LeftTopY and a.Lat >= @RightDownY and ((a.Lng >= -180 and a.Lng <= @RightDownX) OR (a.lng >= @LeftTopX AND a.lng <= 180))";
            }
            else
            {
                sql = $"SELECT TOP 20 a.MerchantName,a.Id,a.IsAllowExpense,a.FileId,a.ThumbnailId, a.Tags, a.Lat, a.Lng, 0 as Distance, a.Address, a.Id as MerchantInformationId,a.AccountType, a.MerchantAccountId FROM [MerchantInformations] a WHERE a.Status = 1 AND a.VerifyStatus = 1 AND IsPublic=1 AND a.Lat <= @LeftTopY and a.Lat >= @RightDownY and a.Lng <= @RightDownX and a.Lng >= @LeftTopX";
            }

            using (var con = ReadConnection())
            {
                return con.Query<MerchantBriefInformation>(sql, new { LeftTopX = leftTopX, LeftTopY = leftTopY, RightDownX = rightDownX, RightDownY = rightDownY }).ToList();
            }
        }

        public List<MerchantBriefInformation> QueryMerchantListByCountryId(decimal lat, decimal lng, decimal distance)
        {
            string sql =
                $"SELECT TOP 20 * FROM (SELECT a.MerchantName,a.Id,a.IsAllowExpense,a.FileId,a.ThumbnailId, a.Tags, a.Lat, a.Lng, (geography::Point({lat},{lng},4326).STDistance(geography::Point(ISNULL(a.Lat,0), ISNULL(a.Lng, 0), 4326))) as Distance, a.Address, a.Id as MerchantInformationId,a.AccountType, a.MerchantAccountId FROM [MerchantInformations] a WHERE a.Status = 1 AND a.VerifyStatus = 1 AND IsPublic=1) c WHERE c.Distance < @Distance ORDER BY c.Distance";
            using (var con = ReadConnection())
            {
                return con.Query<MerchantBriefInformation>(sql, new { Distance = distance }).ToList();
            }
        }

        public MerchantInformation GetActiveMerchantDetailById(Guid id)
        {
            const string sql =
                "SELECT * FROM [MerchantInformations] a where a.Status = 1 AND a.VerifyStatus = 1 AND IsPublic=1 AND a.Id = @Id";
            using (var con = ReadConnection())
            {
                return con.Query<MerchantInformation>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        public MerchantInformation GetMerchantDetailById(Guid id)
        {
            const string sql =
                "SELECT * FROM [MerchantInformations] a where a.Id = @Id";
            using (var con = ReadConnection())
            {
                return con.Query<MerchantInformation>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        public void Insert(MerchantInformation information)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantInformations]([Id],[MerchantName],[Week],[Tags],[Introduce],[CountryId],[StateId],[CityId],[PhoneCode],[Address],[Lng],[Lat],[Status],[VerifyStatus],[MerchantAccountId],[Phone],[Remark],[VerifyDate],[IsPublic],[FileId],[ThumbnailId],[AccountType],[Markup],[FeeRate],[IsAllowExpense],[StartTime],[EndTime],[WeekTxt],[ApplicantName],[CreateTime],[LastModifyBy],[LastModifyTime],[FromType],[UseFiiiDeduct])
                                      VALUES (@Id,@MerchantName,@Week,@Tags,@Introduce,@CountryId,@StateId,@CityId,@PhoneCode,@Address,@Lng,@Lat,@Status,@VerifyStatus,@MerchantAccountId,@Phone,@Remark,@VerifyDate,@IsPublic,@FileId,@ThumbnailId,@AccountType,@Markup,@FeeRate,@IsAllowExpense,@StartTime,@EndTime,@WeekTxt,@ApplicantName,@CreateTime,@LastModifyBy,@LastModifyTime,@FromType,@UseFiiiDeduct)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, information);
            }
        }

        public async Task InsertAsync(MerchantInformation information)
        {
            const string sql = @"INSERT INTO [dbo].[MerchantInformations]([Id],[MerchantName],[Week],[Tags],[Introduce],[CountryId],[StateId],[CityId],[PhoneCode],[Address],[Lng],[Lat],[Status],[VerifyStatus],[MerchantAccountId],[Phone],[Remark],[VerifyDate],[IsPublic],[FileId],[ThumbnailId],[AccountType],[Markup],[FeeRate],[IsAllowExpense],[StartTime],[EndTime],[WeekTxt],[ApplicantName],[CreateTime],[LastModifyBy],[LastModifyTime],[FromType],[UseFiiiDeduct])
                                      VALUES (@Id,@MerchantName,@Week,@Tags,@Introduce,@CountryId,@StateId,@CityId,@PhoneCode,@Address,@Lng,@Lat,@Status,@VerifyStatus,@MerchantAccountId,@Phone,@Remark,@VerifyDate,@IsPublic,@FileId,@ThumbnailId,@AccountType,@Markup,@FeeRate,@IsAllowExpense,@StartTime,@EndTime,@WeekTxt,@ApplicantName,@CreateTime,@LastModifyBy,@LastModifyTime,@FromType,@UseFiiiDeduct)";
            using (var con = await WriteConnectionAsync())
            {
                await con.ExecuteAsync(sql, information);
            }
        }

        /// <summary>
        /// 修改商家所有信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="information"></param>
        public void Update(Guid id, MerchantInformation information)
        {
            const string sql = @"UPDATE MerchantInformations
                                SET MerchantName = @MerchantName,
                                    Week = @Week,
                                    Tags = @Tags,
                                    Introduce = @Introduce,
                                    Address = @Address, 
                                    Lng = @Lng, 
                                    Lat = @Lat,
                                    Phone = @Phone,
                                    VerifyStatus = @VerifyStatus
                                WHERE Id=@Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    information.MerchantName,
                    information.Week,
                    information.Tags,
                    information.Introduce,
                    information.Address,
                    information.Lng,
                    information.Lat,
                    Id = id,
                    information.Phone,
                    information.VerifyStatus
                });
            }
        }

        /// <summary>
        /// 修改商家信息不需要审核
        /// </summary>
        /// <param name="id"></param>
        /// <param name="information"></param>
        public void UpdatePartialInformation(Guid id, MerchantInformation information)
        {
            const string sql = @"UPDATE MerchantInformations
                                SET Week = @Week,
                                    Address = @Address, 
                                    Lng = @Lng, 
                                    Lat = @Lat,
                                    Phone = @Phone
                                WHERE Id=@Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new
                {
                    information.Week,
                    information.Address,
                    information.Lng,
                    information.Lat,
                    information.Phone,
                    information.Id
                });
            }
        }

        public MerchantInformation GetById(Guid merchantInfoId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantInformation>("SELECT * FROM [MerchantInformations] WHERE [Id] = @Id", new { Id = merchantInfoId });
            }
        }

        public MerchantInformation GetByMerchantAccountId(Guid merchantAccountId)
        {
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MerchantInformation>("SELECT * FROM [MerchantInformations] WHERE [MerchantAccountId] = @MerchantAccountId", new { MerchantAccountId = merchantAccountId });
            }
        }

        /// <summary>
        /// 商家门店按钮是否停用
        /// </summary>
        /// <param name="id"></param>
        public void UpdateStatus(Guid id)
        {
            const string sql = @"UPDATE MerchantInformations
                                 SET IsPublic=1-IsPublic WHERE MerchantAccountId=@Id";
            using (var conn = WriteConnection())
            {
                conn.Execute(sql, new { Id = id });
            }
        }

        public string CauseFailure(Guid merchantaccountid)
        {

            const string sql = @"SELECT Remark FROM MerchantInformations WHERE VerifyStatus = @VerifyStatus AND MerchantAccountId = @MerchantAccountId";

            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<string>(sql, new { VerifyStatus = Entities.Enums.VerifyStatus.Disapproval, MerchantAccountId = merchantaccountid });
            }
        }
    }
}
