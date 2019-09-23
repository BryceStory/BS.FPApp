using System;
using System.Collections.Generic;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class UserLoginLogDAC: BaseDataAccess
    {
        public List<UserLoginLog> GetLastLoginTimeListByIds(List<Guid> guids)
        {
            string where = null;
            if (guids.Count > 0)
            {
                where = string.Format("'{0}'", string.Join("\',\'", guids));
            }
            else
            {
                return null;
            }
            using (var con = ReadConnection())
            {
                string SQL = $"SELECT UserAccountId,max([Timestamp]) [Timestamp] FROM UserLoginLog WHERE UserAccountId in ({where}) GROUP BY UserAccountId";
                return con.Query<UserLoginLog>(SQL).AsList();
            }
        }

        public void Insert(UserLoginLog model)
        {
            using (var con = WriteConnection())
            {
                con.Execute(@"INSERT INTO UserLoginLog(UserAccountId,IP,Timestamp) VALUES(@UserAccountId,@IP,@Timestamp);", model);
            }
        }
    }
}
