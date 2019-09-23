using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FiiiPay.Entities;

namespace FiiiPay.Data
{
    public class UserDeviceDAC : BaseDataAccess
    {
        public List<UserDevice> GetUserDeviceByAccountId(Guid accountId)
        {
            var sql = "SELECT * FROM [UserDevices] WHERE UserAccountId = @AccountId";
            using (var con = ReadConnection())
            {
                return con.Query<UserDevice>(sql, new {AccountId = accountId}).ToList();
            }
        }

        public void Insert(UserDevice device)
        {
            var sql =
                "INSERT INTO [UserDevices] ([UserAccountId], [DeviceNumber], [IP], [LastActiveTime], [Name], [Address]) VALUES (@UserAccountId, @DeviceNumber, @IP, @LastActiveTime, @Name, @Address)";
            using (var con = WriteConnection())
            {
                con.Execute(sql, device);
            }
        }

        public void Delete(long id)
        {
            var sql = "DELETE [UserDevices] WHERE Id = @Id";
            using (var con = WriteConnection())
            {
                con.Execute(sql, new {Id = id});
            }
        }

        public void Update(UserDevice device)
        {
            var sql = "UPDATE [UserDevices] SET [LastActiveTime] = @LastActiveTime, [Name] = @Name, [IP] = @IP, [Address] = @Address WHERE DeviceNumber = @DeviceNumber and UserAccountId=@UserAccountId";
            using (var con = WriteConnection())
            {
                con.Execute(sql, device);
            }
        }
    }
}
