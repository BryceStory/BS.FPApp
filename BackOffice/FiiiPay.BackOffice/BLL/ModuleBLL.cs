using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;

namespace FiiiPay.BackOffice.BLL
{
    public class ModuleBLL : BaseBLL
    {
        public SaveResult SaveCreate(Module module, int userId, string userName)
        {
            var sameCodeEnity = BoDB.ModuleDb.IsAny(t => t.Code == module.Code);
            if (sameCodeEnity)
                return new SaveResult() { Result = false, Message = string.Format("Code {0} is exist", module.Code) };

            var result = BoDB.DB.Ado.UseTran(() =>
            {
                module.CreateTime = DateTime.UtcNow;
                int moduleId = BoDB.ModuleDb.InsertReturnIdentity(module);

                ModulePermission mp = new ModulePermission()
                {
                    CreateBy = module.CreateBy,
                    CreateTime = DateTime.UtcNow,
                    ModuleId = moduleId,
                    Code = module.Code + "Menu",
                    Description = "Menu Entrance",
                    IsDefault = true
                };
                BoDB.ModulePermissionDb.Insert(mp);
            });
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ModuleBLL).FullName + ".SaveCreate";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Create Module,ID :{0},Name:{1}", module.Id, module.Name);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(result);
        }
        public SaveResult SaveEdit(Module module, int userId, string userName)
        {
            var oldModule = BoDB.ModuleDb.GetById(module.Id);
            bool codeChanged = oldModule.Code != module.Code;

            if (codeChanged)
            {
                var sameCodeEnity = BoDB.ModuleDb.IsAny(t => t.Code == module.Code);
                if (sameCodeEnity)
                    return new SaveResult() { Result = false, Message = string.Format("Code {0} is exist", module.Code) };
            }


            oldModule.ModifyBy = module.ModifyBy;
            oldModule.ModifyTime = DateTime.UtcNow;
            oldModule.Code = module.Code;
            oldModule.Icon = module.Icon;
            oldModule.Name = module.Name;
            oldModule.PathAddress = module.PathAddress;
            oldModule.ParentId = module.ParentId;
            oldModule.Sort = module.Sort;

            BoDB.ModuleDb.Update(oldModule);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ModuleBLL).FullName + ".SaveEdit";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Module " + module.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            //以下推荐方法报错
            //BoDB.DB.Updateable<Module>().UpdateColumns(t => new
            //{
            //    ModifyBy = module.ModifyBy,
            //    ModifyTime = DateTime.UtcNow,
            //    Code = module.Code,
            //    Icon = module.Icon,
            //    Name = module.Name,
            //    PathAddress = module.PathAddress,
            //    ParentId = module.ParentId,
            //    Sort = module.Sort
            //}).Where(t => t.Id == module.Id).ExecuteCommand();

            return new SaveResult(true, "Save Success");
        }

        public SaveResult SaveDelete(int Id, int userId, string userName)
        {
            var hasChild = BoDB.ModuleDb.IsAny(t => t.ParentId == Id);
            if (hasChild)
                return new SaveResult() { Result = false, Message = "This module had one or more children modules" };

            var result = BoDB.DB.Ado.UseTran(() =>
            {
                BoDB.RoleAuthorityDb.Delete(t => t.ModuleId == Id);
                BoDB.ModulePermissionDb.Delete(t => t.ModuleId == Id);
                BoDB.ModuleDb.DeleteById(Id);
            });

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ModuleBLL).FullName + ".SaveDelete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Module " + Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(result);
        }

        public SaveResult SaveAddPermission(ModulePermission permission, int userId, string userName)
        {
            var module = BoDB.ModuleDb.GetById(permission.ModuleId);
            if (!module.ParentId.HasValue)
                return new SaveResult(false, "Can't add permission for root module");

            var hasSameCode = BoDB.ModulePermissionDb.IsAny(t => t.Code == permission.Code);
            if (hasSameCode)
                return new SaveResult(false, string.Format("The code {0} is exist", permission.Code));

            permission.CreateTime = DateTime.UtcNow;
            permission.IsDefault = false;
            BoDB.ModulePermissionDb.Insert(permission);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ModuleBLL).FullName + ".SaveAddPermission";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Create ModulePermission,ID:{0},Code:{1} ", permission.Id, permission.Code);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult();
        }

        public SaveResult SaveEditPermission(ModulePermission permission, int userId, string userName)
        {
            var oldEnt = BoDB.ModulePermissionDb.GetById(permission.Id);
            if (oldEnt.IsDefault)
            {
                return new SaveResult(false, "This permission can't be updated");
            }
            bool codeChanged = oldEnt.Code != permission.Code;
            if (codeChanged)
            {
                var hasSameCode = BoDB.ModulePermissionDb.IsAny(t => t.Code == permission.Code);
                if (hasSameCode)
                {
                    return new SaveResult(false, string.Format("The code {0} is exist", permission.Code));
                }
            }

            oldEnt.Code = permission.Code;
            oldEnt.Description = permission.Description;
            oldEnt.Remark = permission.Remark;
            oldEnt.ModifyBy = permission.ModifyBy;
            oldEnt.ModifyTime = DateTime.UtcNow;

            BoDB.ModulePermissionDb.Update(oldEnt);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ModuleBLL).FullName + ".SaveEditPermission";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Update ModulePermission,ID:{0},Code:{1} ", permission.Id, permission.Code);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult SaveDeletePermission(int id, int userId, string userName)
        {
            var result = BoDB.DB.Ado.UseTran(() =>
            {
                BoDB.RoleAuthorityDb.Delete(t => t.PermissionId == id);
                BoDB.ModulePermissionDb.DeleteById(id);
            });

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ModuleBLL).FullName + ".SaveDeletePermission";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Delete ModulePermission,ID:{0}", id);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(result);
        }
    }
}