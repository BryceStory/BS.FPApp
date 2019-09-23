using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class StoreTypeBLL : BaseBLL
    {
        public SaveResult Create(StoreTypes storeType, int userId, string userName)
        {
            var storeTypeId = FiiiPayDB.StoreTypeDb.InsertReturnIdentity(storeType);
            bool saveSuccess = storeTypeId > 0;
            if (saveSuccess)
            {
                ActionLog actionLog = new ActionLog();
                actionLog.IPAddress = GetClientIPAddress();
                actionLog.AccountId = userId;
                actionLog.CreateTime = DateTime.UtcNow;
                actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".Create";
                actionLog.Username = userName;
                actionLog.LogContent = "Create StoreType " + storeTypeId;
                new ActionLogBLL().Create(actionLog);
            }

            return new SaveResult(saveSuccess);
        }

        public SaveResult Update(StoreTypes storeType, int userId, string userName)
        {
            bool saveSuccess = FiiiPayDB.StoreTypeDb.Update(storeType);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update StoreType " + storeType.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(saveSuccess);
        }

        public SaveResult DeleteById(int id, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(ArticleBLL).FullName + ".DeleteById";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete StoreType " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.StoreTypeDb.DeleteById(id));
        }
    }
}