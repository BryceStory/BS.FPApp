using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class PriceInfoAgent : BaseBLL
    {
        public bool UpdateMarketPriceToDB()
        {
            var apiAddress = ConfigurationManager.AppSettings["APIAddress"];
            var url = $"{apiAddress}/Wallet/UpdateMarketPriceToDB";
            var result = RestUtilities.GetJson(url);
            if (result == "\"Success\"")
                return true;
            return false;
        }

        public bool Insert(List<PriceInfos> list)
        {
            return FoundationDB.PriceInfoDb.InsertRange(list.ToArray());
        }
        public SaveResult Create(PriceInfos model, int userId, string userName)
        {
            model.LastUpdateDate = DateTime.UtcNow;
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create PriceInfos " + model.ID;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FoundationDB.PriceInfoDb.Insert(model));
        }

        public SaveResult Update(PriceInfos model, int userId, string userName)
        {
            var oldPrice = FoundationDB.PriceInfoDb.GetById(model.ID);
            oldPrice.Markup = model.Markup;
            oldPrice.Price = model.Price;
            oldPrice.LastUpdateDate = DateTime.UtcNow;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update PriceInfos " + model.ID;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FoundationDB.PriceInfoDb.Update(oldPrice));
        }

        public SaveResult Delete(int Id, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete PriceInfos " + Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FoundationDB.PriceInfoDb.DeleteById(Id));
        }

    }
}