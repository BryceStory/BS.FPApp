using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class AdvertisingsBLL : BaseBLL
    {
        public SaveResult Update(Advertisings adv, int userId, string userName)
        {
            Advertisings oldAdv = FiiiPayDB.AdvertisingDb.GetById(adv.Id);
            if (adv.PictureEn != Guid.Empty)
                oldAdv.PictureEn = adv.PictureEn;
            if (adv.PictureZh != Guid.Empty)
                oldAdv.PictureZh = adv.PictureZh;
            oldAdv.LinkType = adv.LinkType;
            oldAdv.Link = adv.Link;
            oldAdv.Title = adv.Title;
            oldAdv.Status = adv.Status;
            oldAdv.StartDate = adv.StartDate;
            oldAdv.EndDate = adv.EndDate;
            oldAdv.Version = oldAdv.Version + 1;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Advertisings " + adv.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.AdvertisingDb.Update(oldAdv));
        }

        public SaveResult<int> Create(Advertisings adv, int userId, string userName)
        {
            adv.Version = 1;
            adv.CreateTime = DateTime.Now;
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Advertisings " + adv.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            return new SaveResult<int>(true, FiiiPayDB.AdvertisingDb.InsertReturnIdentity(adv));
        }

        public SaveResult DeleteById(int id, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(FeedbacksBLL).FullName + ".Delete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Feedbacks " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.AdvertisingDb.DeleteById(id));
        }

    }
}