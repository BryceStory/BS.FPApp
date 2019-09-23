using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiPay.BackOffice.BLL
{
    public class MasterSettingBLL : BaseBLL
    {
        public SaveResult Update(List<string> KeyPairValues, int userId, string userName, string group)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();
            string logContent = "";
            KeyPairValues.ForEach(data =>
            {
                sb.Append(" Update dbo.MasterSettings " +
                    "SET " +
                        "[Value]=@Value" + count.ToString() +
                " WHERE [Name]=@Name" + count.ToString() + " and [Group] = @Group; ");
                count++;
            });

            count = 0;
            int paraCount = 0;
            // Connect to database.
            SugarParameter[] Paras = new SugarParameter[(KeyPairValues.Count * 2) + 1];
            // Set parameter values.
            KeyPairValues.ForEach(data =>
            {
                Paras[count] = new SugarParameter("@Value" + paraCount.ToString(), KeyPairValues[paraCount].Split(',')[1]);
                count++;
                Paras[count] = new SugarParameter("@Name" + paraCount.ToString(), KeyPairValues[paraCount].Split(',')[0]);
                logContent += KeyPairValues[paraCount].Split(',')[1] + ",";
                count++;
                paraCount++;
            });
            Paras[KeyPairValues.Count * 2] = new SugarParameter("@Group", group);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MasterSettingBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Update MasterSetting, Values:{0}", logContent.TrimEnd(','));
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);   

            return new SaveResult(FoundationDB.DB.Ado.ExecuteCommand(sb.ToString(), Paras) > 0);
        }

        public SaveResult UpdateBatch(Dictionary<string,List<string>> list, int userId, string userName)
        {
            if (list == null || list.Count <= 0)
                return new SaveResult(true);
            int count = 0, groupCount = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder logSb = new StringBuilder();
            List<SugarParameter> paramList = new List<SugarParameter>();
            foreach (var item in list)
            {
                if (item.Value == null)
                    continue;
                item.Value.ForEach(data =>
                {
                    var nv = data.Split(',');

                    sb.Append($"Update dbo.MasterSettings SET [Value]=@Value{count} WHERE [Name]=@Name{count} and [Group] = @Group{groupCount}; ");
                    logSb.AppendFormat($"{0}={1},", nv[0], nv[1]);

                    paramList.Add(new SugarParameter("@Name" + count, nv[0]));
                    paramList.Add(new SugarParameter("@Value" + count, nv[1]));

                    count++;
                });
                paramList.Add(new SugarParameter("@Group" + groupCount, item.Key));
                groupCount++;
            }

            var sr = FoundationDB.DB.Ado.UseTran(() =>
            {
                FoundationDB.DB.Ado.ExecuteCommand(sb.ToString(), paramList.ToArray());
            });

            if (sr.IsSuccess)
            {
                // Create ActionLog
                ActionLog actionLog = new ActionLog();
                actionLog.IPAddress = GetClientIPAddress();
                actionLog.AccountId = userId;
                actionLog.CreateTime = DateTime.UtcNow;
                actionLog.ModuleCode = typeof(MasterSettingBLL).FullName + ".UpdateBatch";
                actionLog.Username = userName;
                actionLog.LogContent = string.Format("Update MasterSetting, Values:{0}", logSb.ToString().TrimEnd(','));
                ActionLogBLL ab = new ActionLogBLL();
                ab.Create(actionLog);
            }

            return new SaveResult(sr.IsSuccess, sr.ErrorMessage);
        }
    }
}