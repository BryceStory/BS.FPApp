using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class FileBLL : BaseBLL
    {
        public SaveResult DeleteById(string id, int userId, string userName)
        {
            BlobBLL bb = new BlobBLL();
            bb.DeleteImage(id);
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(FeedbacksBLL).FullName + ".Delete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete File " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            var result = FoundationDB.FileDb.DeleteById(id);
            return new SaveResult(result);
        }
    }
}