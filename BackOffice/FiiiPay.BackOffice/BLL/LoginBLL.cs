using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Framework.Component;

namespace FiiiPay.BackOffice.BLL
{
    public class LoginBLL:BaseBLL
    {
        public bool CheckUser(string userName, string passcode, out Account account, ref string mes)
        {
            try
            {
                account = BoDB.DB.Queryable<Account>().Where(t => t.Username == userName).First();
                if (account == null)
                {
                    mes = "Username or password is wrong";
                    return false;
                }
                bool checkResult = PasswordHasher.VerifyHashedPassword(account.Password, passcode);
                if (!checkResult)
                    mes = "Username or password is wrong";
                return checkResult;
            }
            catch (Exception ex)
            {
                account = null;
                mes = "Login Failed";
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(LoginBLL));
                log.Error(ex);
                return false;
            }
        }

        public List<UserPermission> GetUserPermissionByRoleId(int roleId)
        {
            string sql = "SELECT t2.Code AS PerimCode,t3.Id AS ModuleId,t3.Code AS ModuleCode,t2.IsDefault,t1.Value FROM dbo.RoleAuthority t1";
            sql += " LEFT JOIN dbo.ModulePermission t2 ON t1.PermissionId=t2.Id";
            sql += " LEFT JOIN dbo.Module t3 ON t1.ModuleId=t3.Id";
            sql += " WHERE t1.RoleId=@RoleId and t1.Value>0";
            var data = BoDB.DB.Ado.SqlQuery<UserPermission>(sql,new { RoleId= roleId }).ToList();
            return data;
        }

        public List<UserPermission> GetAllPermission(int roleId)
        {
            string sql = "SELECT t1.Code AS PerimCode,t2.Id AS ModuleId,t2.Code AS ModuleCode,t1.IsDefault,1 AS [Value] FROM dbo.ModulePermission t1";
            sql += " INNER JOIN dbo.Module t2 ON t1.ModuleId=t2.Id";
            var data = BoDB.DB.Ado.SqlQuery<UserPermission>(sql).ToList();
            return data;
        }
    }
}