using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;

namespace FiiiPay.BackOffice.BLL
{
    public class InvestorWalletStatementBLL : BaseBLL
    {
        public SaveResult Create(InvestorWalletStatements statement, InvestorAccounts investor, int userId, string userName)
        {
            statement.Id = Guid.NewGuid();
            FiiiPayDB.InvestorAccountDb.Update(investor);
            int id = FiiiPayDB.InvestorWalletStatementDb.InsertReturnIdentity(statement);
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create InvestorWalletStatement " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            return new SaveResult(true);
        }
    }
}