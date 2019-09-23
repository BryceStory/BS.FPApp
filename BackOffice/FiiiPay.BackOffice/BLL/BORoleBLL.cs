using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.BackOffice.BLL
{
    public class BORoleBLL:BaseBLL
    {
        public SaveResult SaveCreate(AccountRole role, int userId, string userName)
        {
            var hasSameCode = BoDB.AccountRoleDb.IsAny(t => t.Name == role.Name);
            if (hasSameCode)
                return new SaveResult() { Result = false, Message = string.Format("Rolename {0} is exist", role.Name) };
            
            role.CreateTime = DateTime.UtcNow;
            BoDB.AccountRoleDb.Insert(role);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(BORoleBLL).FullName + ".SaveCreate";
            actionLog.Username = userName;
            actionLog.LogContent = "Create BORole " + role.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult SaveEdit(AccountRole role, int userId, string userName)
        {
            AccountRole oldRole = BoDB.AccountRoleDb.GetById(role.Id);
            bool codeChanged = oldRole.Name != role.Name;
            if (codeChanged)
            {
                var hasSameCode = BoDB.AccountRoleDb.IsAny(t => t.Name == role.Name);
                if (hasSameCode)
                    return new SaveResult() { Result = false, Message = string.Format("Rolename {0} is exist", role.Name) };
            }
            
            oldRole.Name = role.Name;
            oldRole.Description = role.Description;
            oldRole.ModifyBy = role.ModifyBy;
            oldRole.ModifyTime = DateTime.UtcNow;

            BoDB.AccountRoleDb.Update(oldRole);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(BORoleBLL).FullName + ".SaveEdit";
            actionLog.Username = userName;
            actionLog.LogContent = "Update BORole " + role.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult SaveDelete(int id, int userId, string userName)
        {
            bool hasAccount = BoDB.AccountDb.IsAny(t => t.RoleId == id);
            if (hasAccount)
                return new SaveResult() { Result = false, Message = "There are one or more accounts which is belong to this role" };

            var result = BoDB.DB.Ado.UseTran(() => {
                BoDB.RoleAuthorityDb.Delete(t => t.RoleId == id);
                BoDB.AccountRoleDb.DeleteById(id);
            });
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(BORoleBLL).FullName + ".SaveDelete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete BORole " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(result);
        }

        public SaveResult SavePermission(int adminId, List<RoleAuthority> roleAuthList, int userId, string userName)
        {
            int roleId = roleAuthList[0].RoleId;
            foreach (var item in roleAuthList)
            {
                item.CreateBy = adminId;
                item.CreateTime = DateTime.UtcNow;
            }

            var result = BoDB.DB.Ado.UseTran(() => {
                BoDB.RoleAuthorityDb.Delete(t => t.RoleId == roleId);
                if(roleAuthList!=null)
                    BoDB.RoleAuthorityDb.InsertRange(roleAuthList.ToArray());
            });
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(BORoleBLL).FullName + ".SavePermission";
            actionLog.Username = userName;
            var permissionCodes = roleAuthList == null ? "" : (roleAuthList.Select(t => t.PermissionId).ToArray().ToString());
            actionLog.LogContent = string.Format("Save RoleAuthority,roleid:{0},permissioncodes:{1}", roleId, permissionCodes);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(result);
        }
    }
}