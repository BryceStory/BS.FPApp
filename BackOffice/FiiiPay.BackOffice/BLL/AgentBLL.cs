using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;

namespace FiiiPay.BackOffice.BLL
{
    public class AgentBLL : BaseBLL
    {
        public List<AgentViewModel> GetAgentPersonPager(string companyName, int? countryId, int? stateId, int? cityId, ref GridPager pager)
        {
            string sql = "SELECT t1.*,t2.SaleCode,t2.SaleName FROM [dbo].[Agent] t1";
            sql += " LEFT JOIN [dbo].[Salesperson] t2 ON t1.SaleId=t2.Id";
            sql += " WHERE 1=1";
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(companyName))
            {
                sql += " AND t1.CompanyName LIKE @CompanyName";
                paramList.Add(new SqlSugar.SugarParameter("@CompanyName", "%" + companyName + "%"));
            }
            if (countryId.HasValue&&countryId.Value>0)
            {
                sql += " AND t1.CountryId=@CountryId";
                paramList.Add(new SqlSugar.SugarParameter("@CountryId", countryId.Value));
            }
            if (stateId.HasValue && stateId.Value > 0)
            {
                sql += " AND t1.StateId=@StateId";
                paramList.Add(new SqlSugar.SugarParameter("@StateId", stateId.Value));
            }
            if (cityId.HasValue && cityId.Value > 0)
            {
                sql += " AND t1.CityId=@CityId";
                paramList.Add(new SqlSugar.SugarParameter("@CityId", cityId.Value));
            }
            var data = QueryPager.Query<AgentViewModel>(BoDB.DB, sql, ref pager, paramList);
            return data;
        }

        public SaveResult SaveCreate(Agent agent, int userId, string userName)
        {
            agent.CreateTime = DateTime.UtcNow;
            agent.AgentCode = GenerateSalesCode();
            BoDB.AgentDb.Insert(agent);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AgentBLL).FullName + ".SaveCreate";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Agent " + agent.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult SaveEdit(Agent agent, int userId, string userName)
        {
            var oldAgent = BoDB.AgentDb.GetById(agent.Id);
            oldAgent.ModifyBy = agent.ModifyBy;
            oldAgent.ModifyTime = DateTime.UtcNow;
            oldAgent.CompanyName = agent.CompanyName;
            oldAgent.ContactName = agent.ContactName;
            oldAgent.ContactWay = agent.ContactWay;
            oldAgent.SaleId = agent.SaleId;
            oldAgent.CountryId = agent.CountryId;
            oldAgent.StateId = agent.StateId;
            oldAgent.CityId = agent.CityId;
            BoDB.AgentDb.Update(oldAgent);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AgentBLL).FullName + ".SaveEdit";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Agent " + agent.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        public SaveResult Delete(int agentId, int userId, string userName)
        {
            BoDB.AgentDb.DeleteById(agentId);
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AgentBLL).FullName + ".Delete";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Agent " + agentId;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true);
        }

        private string GenerateSalesCode()
        {
            var setting = BoDB.DB.Queryable<Setting>().First();
            string prevStr = "P";
            if (setting == null)
            {
                setting = new Setting()
                {
                    AgentIndex = 1
                };
                BoDB.SettingDb.Insert(setting);
                return prevStr + "0001";
            }
            var agentIndex = setting.AgentIndex + 1;
            setting.AgentIndex = agentIndex;
            BoDB.SettingDb.Update(setting);
            return prevStr + agentIndex.ToString().PadLeft(4, '0');
        }
    }
}