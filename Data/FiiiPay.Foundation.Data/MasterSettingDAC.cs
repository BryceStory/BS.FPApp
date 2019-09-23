using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.Foundation.Data
{
    public class MasterSettingDAC : BaseFoundationDataAccess
    {
        public List<MasterSetting> SelectByGroup(string group)
        {
            const string SQL_STATEMENT =
                "SELECT [Id], [Group], [Name], [Type], [Value] " +
                "FROM dbo.MasterSettings " +
                "WHERE [Group]=@Group ";
            using (var con = ReadConnection())
            {
                return con.Query<MasterSetting>(SQL_STATEMENT, new { Group = group }).ToList();
            }
        }

        public MasterSetting Single(string group, string name)
        {
            const string SQL_STATEMENT =
                "SELECT TOP 1 [Id], [Group], [Name], [Type], [Value] " +
                "FROM dbo.MasterSettings  " +
                "WHERE [Group]=@Group AND [Name]=@Name ";
            using (var con = ReadConnection())
            {
                return con.QueryFirstOrDefault<MasterSetting>(SQL_STATEMENT, new { Group = group, Name = name });
            }
        }
    }
}