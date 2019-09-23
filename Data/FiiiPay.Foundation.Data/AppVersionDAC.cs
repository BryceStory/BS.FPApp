using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class AppVersionDAC : BaseFoundationDataAccess
    {
        public AppVersion GetLatestByPlatform(byte platform, byte app)
        {
            const string SQL_STATEMENT =
                "SELECT top 1 * " +
                "FROM dbo.AppVersions where Platform=@platform and [App]=@app order by Id desc";

            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<AppVersion>(SQL_STATEMENT, new { platform, app });
            }
        }
    }
}
