using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;

namespace FiiiPay.BackOffice.BLL
{
    public class FeedbacksBLL : BaseBLL
    {
        public List<FeedbackViewModel> GetFeedbacksList(string type, bool? hasProcessor, ref GridPager pager)
        {
            string sql = " SELECT t1.*,(CASE when t1.Type = 'FiiiPOS' then t3.Username else t2.Cellphone end) as AccountName," +
                "(CASE when t1.Type = 'FiiiPOS' then t3.CountryId else t2.CountryId end) as CountryId from Feedbacks t1 ";
            sql += " LEFT JOIN UserAccounts t2 on t1.AccountId = t2.id ";
            sql += " LEFT JOIN MerchantAccounts t3 on t1.AccountId = t3.Id where 1=1 ";

            var paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(type))
            {
                sql += " and t1.Type=@Type ";
                paramList.Add(new SqlSugar.SugarParameter("@Type", type));
            }
            if (hasProcessor.HasValue)
            {
                sql += "and t1.HasProcessor=@HasProcessor";
                paramList.Add(new SqlSugar.SugarParameter("@HasProcessor", hasProcessor));
            }

            var data = QueryPager.Query<FeedbackViewModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            //var data = FiiiPayDB.DB.Ado.SqlQuery<Feedbacks>(sql, new { Type = type, HasProcessor = hasProcessor }).ToList();
            return data;
        }

        public SaveResult Update(Feedbacks fb, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(FeedbacksBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Feedbacks " + fb.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.FeedbacksDb.Update(fb));
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

            return new SaveResult(FiiiPayDB.FeedbacksDb.DeleteById(id));
        }

        public SaveResult BatchDelete(string ids, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(FeedbacksBLL).FullName + ".BatchDelete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Feedbacks " + ids;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.FeedbacksDb.DeleteByIds(ids.Split(',')));
        }

    }
}