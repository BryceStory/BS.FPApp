using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;

namespace FiiiPay.BackOffice.BLL
{
    public class InvestorAccountBLL : BaseBLL
    {
        public SaveResult Create(InvestorAccounts investor, int userId, string userName)
        {
            if (CheckNameExt(investor.Username))
            {
                return new SaveResult(false, "This username already exists!");
            }

            int id = FiiiPayDB.InvestorAccountDb.InsertReturnIdentity(investor);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Investor " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);


            return new SaveResult(true);
        }

        public SaveResult Update(InvestorAccounts investor, int userId, string userName)
        {
            InvestorAccounts oldInvestor = FiiiPayDB.InvestorAccountDb.GetById(investor.Id);
            oldInvestor.CountryId = investor.CountryId;
            oldInvestor.InvestorName = investor.InvestorName;
            oldInvestor.Cellphone = investor.Cellphone;
            oldInvestor.Status = investor.Status;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Investor " + investor.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.InvestorAccountDb.Update(oldInvestor));
        }

        public SaveResult UpdatePassword(InvestorAccounts investor, int userId, string userName)
        {
            InvestorAccounts oldInvestor = FiiiPayDB.InvestorAccountDb.GetById(investor.Id);
            oldInvestor.Password = investor.Password;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".UpdatePassword";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Investor Password " + investor.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.InvestorAccountDb.Update(oldInvestor));
        }
        public SaveResult UpdatePIN(InvestorAccounts investor, int userId, string userName)
        {
            InvestorAccounts oldInvestor = FiiiPayDB.InvestorAccountDb.GetById(investor.Id);
            oldInvestor.PIN = investor.PIN;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".UpdatePIN";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Investor PIN " + investor.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.InvestorAccountDb.Update(oldInvestor));
        }

        public bool CheckNameExt(string userName)
        {
            InvestorAccounts account = FiiiPayDB.DB.Queryable<InvestorAccounts>().Where(t => t.Username == userName).First();
            return account != null;
        }
    }
}