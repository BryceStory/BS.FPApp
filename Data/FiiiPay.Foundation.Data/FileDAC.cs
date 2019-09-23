using System;
using System.Collections.Generic;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    /// <summary>
    /// Class FiiiPay.Foundation.Data.FileDAC
    /// </summary>
    /// <seealso cref="FiiiPay.Foundation.Data.BaseFoundationDataAccess" />
    public class FileDAC : BaseFoundationDataAccess
    {
        /// <summary>
        /// Inserts a new row in the File table.
        /// </summary>
        /// <param name="file">A File object.</param>
        /// <returns>An updated File object.</returns>
        public void Create(File file)
        {
            const string SQL_STATEMENT =
                "INSERT INTO dbo.Files ([Id], [GroupName],[Md5], [FileType], [FileName], [MimeType], [FilePath], [Timestamp]) " +
                "VALUES(@Id, @GroupName,@Md5, @FileType, @FileName, @MimeType, @FilePath, @Timestamp);";

            using (var conn = WriteConnection())
            {
                conn.ExecuteScalar(SQL_STATEMENT, file);
            }
        }

        /// <summary>
        /// Conditionally deletes one or more rows in the File table.
        /// </summary>
        /// <param name="id">A id value.</param>
        public void Delete(Guid id)
        {
            const string SQL_STATEMENT = "DELETE dbo.Files " +
                                         "WHERE [Id]=@Id ";
            using (var conn = WriteConnection())
            {
                conn.ExecuteScalar<int>(SQL_STATEMENT, new {Id = id});
            }
        }

        /// <summary>
        /// Returns a row from the File table.
        /// </summary>
        /// <param name="id">A Id value.</param>
        /// <returns>A File object with data populated from the database.</returns>
        public File SelectById(Guid id)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM dbo.Files  " +
                "WHERE [Id]=@Id ";

            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<File>(SQL_STATEMENT, new {Id = id});
            }
        }

        public File SelectByMd5(string md5)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM dbo.Files  " +
                "WHERE [Md5]=@Md5 ";

            using (var conn = ReadConnection())
            {
                return conn.QueryFirstOrDefault<File>(SQL_STATEMENT, new { Md5 = md5 });
            }
        }

        public IEnumerable<File> SelectByGroupName(string groupName)
        {
            const string sql = "SELECT * FROM dbo.Files WHERE GroupName = @GroupName";
            using (var conn = ReadConnection())
            {
                return conn.Query<File>(sql, new { GroupName = groupName });
            }
        }

        /// <summary>
        /// 源图为缩略图
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageFrom"></param>
        /// <returns></returns>
        //public Image GetReducedImage2(int width, int height, Image imageFrom)
        //{
        //    // 源图宽度及高度 
        //    int imageFromWidth = imageFrom.Width;
        //    int imageFromHeight = imageFrom.Height;
        //    try
        //    {
        //        // 生成的缩略图实际宽度及高度.如果指定的高和宽比原图大，则返回原图；否则按照指定高宽生成图片
        //        if (width >= imageFromWidth && height >= imageFromHeight)
        //        {
        //            return imageFrom;
        //        }
        //        else
        //        {
        //            Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(() => { return false; });
        //            //调用Image对象自带的GetThumbnailImage()进行图片缩略
        //            Image reducedImage = imageFrom.GetThumbnailImage(width, height, callb, IntPtr.Zero);
        //            //将图片以指定的格式保存到到指定的位置
        //            reducedImage.Save(@"E:\640x480.png", ImageFormat.Png);
        //            return reducedImage;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //抛出异常
        //        throw new Exception("转换失败，请重试！");
        //    }
        //}
    }
}
