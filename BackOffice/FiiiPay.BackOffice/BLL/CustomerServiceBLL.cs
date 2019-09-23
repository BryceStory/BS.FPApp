using System;
using System.Collections.Generic;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;

namespace FiiiPay.BackOffice.BLL
{
    public class CustomerServiceBLL : BaseBLL
    {
        public SaveResult Update(string[] wechats, string facebook, int userId, string userName)
        {
            List<MasterSettings> list = new List<MasterSettings>();

            for (int i = 0; i < wechats.Length; i++)
            {
                list.Add(new MasterSettings
                {
                    Group = "CustomerService",
                    Name = "WX",
                    Type = "string",
                    Value = wechats[i]
                });
            }
            list.Add(new MasterSettings
            {
                Group = "CustomerService",
                Name = "FB",
                Type = "string",
                Value = facebook
            });

            var sr = FoundationDB.DB.Ado.UseTran(() =>
            {
                FoundationDB.MasterSettingDb.Delete(t => t.Group == "CustomerService");
                FoundationDB.MasterSettingDb.InsertRange(list);
            });

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CustomerServiceBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Update MasterSetting of CustomerService");
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(sr.IsSuccess, sr.ErrorMessage);
        }
    }
}