using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;

namespace FiiiPay.BackOffice.BLL
{
    public class SalespersonBLL : BaseBLL
    {
        public List<SalespersonViewModel> GetSalesPersonPager(string salesCode, string salesName, string mobile, ref GridPager pager)
        {
            string sql = "SELECT *,(SELECT COUNT(1) FROM dbo.Agent t2 WHERE t2.SaleId=t1.Id) AS AgentCount FROM dbo.Salesperson t1 WHERE 1=1";
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(salesCode))
            {
                sql += " AND t1.SaleCode LIKE @SaleCode";
                paramList.Add(new SqlSugar.SugarParameter("@SaleCode", "%" + salesCode + "%"));
            }
            if (!string.IsNullOrEmpty(salesName))
            {
                sql += " AND t1.SaleName LIKE @SaleName";
                paramList.Add(new SqlSugar.SugarParameter("@SaleName", "%" + salesName + "%"));
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                sql += " AND t1.Mobile=@Mobile";
                paramList.Add(new SqlSugar.SugarParameter("@Mobile", mobile));
            }
            var data = QueryPager.Query<SalespersonViewModel>(BoDB.DB, sql, ref pager, paramList);
            return data;
        }

        public SaveResult SaveCreate(Salesperson sales, int userId, string userName)
        {
            sales.CreateTime = DateTime.UtcNow;
            sales.SaleCode = GenerateSalesCode();
            BoDB.SalespersonDb.Insert(sales);
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(SalespersonBLL).FullName + ".SaveCreate";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Create Salesperson,ID:{0},SaleName:{1} ", sales.Id, sales.SaleName);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult SaveEdit(Salesperson sales, int userId, string userName)
        {
            var oldSales = BoDB.SalespersonDb.GetById(sales.Id);
            oldSales.ModifyBy = sales.ModifyBy;
            oldSales.ModifyTime = DateTime.UtcNow;
            oldSales.SaleName = sales.SaleName;
            oldSales.Position = sales.Position;
            oldSales.Gender = sales.Gender;
            oldSales.Mobile = sales.Mobile;
            BoDB.SalespersonDb.Update(oldSales);
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(SalespersonBLL).FullName + ".SaveEdit";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Update Salesperson,ID:{0},SaleName:{1} ", sales.Id, sales.SaleName);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult Delete(int salesId, int userId, string userName)
        {
            BoDB.SalespersonDb.DeleteById(salesId);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(SalespersonBLL).FullName + ".Delete";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Delete Salesperson,ID:{0} ", salesId);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        private string GenerateSalesCode()
        {
            var setting = BoDB.DB.Queryable<Setting>().First();
            string prevStr = "S";
            if (setting == null)
            {
                setting = new Setting()
                {
                    SalespersonIndex = 1
                };
                BoDB.SettingDb.Insert(setting);
                return prevStr + "0001";
            }
            var salesIndex = setting.SalespersonIndex + 1;
            setting.SalespersonIndex = salesIndex;
            BoDB.SettingDb.Update(setting);
            return prevStr + salesIndex.ToString().PadLeft(4, '0');
        }
    }
}