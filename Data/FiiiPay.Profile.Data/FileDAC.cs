using System;
using System.Collections.Generic;
using Dapper;
using FiiiPay.Profile.Entities;

namespace FiiiPay.Profile.Data
{
    public class FileDAC : BaseDataAccess
    {
        /// <summary>
        /// Inserts a new row in the File table.
        /// </summary>
        /// <param name="file">A File object.</param>
        /// <returns>An updated File object.</returns>
        public void Create(File file)
        {
            const string SQL_STATEMENT =
                "INSERT INTO dbo.Files ([Id], [AccountId], [FileType], [FileName], [MimeType], [FilePath], [Timestamp]) " +
                "VALUES(@Id, @AccountId, @FileType, @FileName, @MimeType, @FilePath, @Timestamp);";

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

        /// <summary>
        /// Conditionally retrieves one or more rows from the File table.
        /// </summary>
        /// <param name="accountId">A accountId value.</param>
        /// <returns>A collection of File objects.</returns>		
        public IEnumerable<File> SelectByAccountId(long accountId)
        {
            const string SQL_STATEMENT =
                "SELECT * " +
                "FROM dbo.Files " +
                "WHERE [AccountId]=@AccountId ";

            using (var conn = ReadConnection())
            {
                return conn.Query<File>(SQL_STATEMENT, new {AccountId = accountId});
            }
        }
    }
}
