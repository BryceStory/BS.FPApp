using System;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Foundation.Entities;

namespace FiiiPay.BackOffice.BLL
{
    public class AppVersionBLL : BaseBLL
    {
        public SaveResult Update(AppVersion model, int userId, string userName)
        {
            var oldModel = FoundationDB.AppVersionDb.GetById(model.Id);
            oldModel.Description = model.Description;
            oldModel.ForceToUpdate = model.ForceToUpdate;
            oldModel.Url = model.Url;
            oldModel.Version = model.Version;

            var updateResult = FoundationDB.AppVersionDb.Update(oldModel);
            if (updateResult)
            {
                int keyDb = 4;
                string key = $"Foundation:Version:{model.App}:{model.Platform}";
                Framework.Cache.RedisHelper.KeyDelete(keyDb, key);

                // Create ActionLog
                ActionLog actionLog = new ActionLog();
                actionLog.IPAddress = GetClientIPAddress();
                actionLog.AccountId = userId;
                actionLog.CreateTime = DateTime.UtcNow;
                actionLog.ModuleCode = typeof(AppVersionBLL).FullName + ".Update";
                actionLog.Username = userName;
                actionLog.LogContent = "Update AppVersion " + model.Id;
                ActionLogBLL ab = new ActionLogBLL();
                ab.Create(actionLog);
            }

            return new SaveResult(updateResult);
        }
    }
}