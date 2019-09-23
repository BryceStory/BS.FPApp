using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using SqlSugar;
using System;

namespace FiiiPay.BackOffice.BLL
{
    public class AccountBLL : BaseBLL
    {
        public AccountViewModel GetViewAccountById(int accountId)
        {
            AccountViewModel account = BoDB.DB.Queryable<AccountViewModel, AccountRole>((ac, role) => new object[] {
                JoinType.Left,ac.RoleId == role.Id}).Select((ac, role) =>
                new AccountViewModel { Id = ac.Id, Username = ac.Username,RoleId = role.Id, Rolename = role.Name, Cellphone = ac.Cellphone, Email = ac.Email })
                .Where(ac => ac.Id == accountId).First();
            return account;
        }

        public SaveResult Create(Account account, int userId, string userName)
        {
            if (CheckNameExt(account.Username))
            {
                return new SaveResult(false, "This username already exists!");
            }

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Account " + account.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(BoDB.AccountDb.Insert(account));
        }

        public SaveResult Update(Account account, int userId, string userName)
        {
            Account oldAccount = BoDB.AccountDb.GetById(account.Id);
            oldAccount.RoleId = account.RoleId;
            oldAccount.Cellphone = account.Cellphone;
            oldAccount.Email = account.Email;
            oldAccount.ModifyBy = account.ModifyBy;
            oldAccount.ModifyTime = DateTime.UtcNow;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Account " + account.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(BoDB.AccountDb.Update(oldAccount));
        }

        public SaveResult UpdatePassword(Account account, int userId, string userName)
        {
            Account oldAccount = BoDB.AccountDb.GetById(account.Id);
            oldAccount.Password = account.Password;
            oldAccount.ModifyBy = account.ModifyBy;
            oldAccount.ModifyTime = DateTime.UtcNow;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Password " + account.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(BoDB.AccountDb.Update(oldAccount));
        }

        public SaveResult DeleteById(int id, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".DeleteById";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Account " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(BoDB.AccountDb.DeleteById(id));
        }

        public SaveResult BatchDelete(string ids, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".BatchDelete";
            actionLog.Username = userName;
            actionLog.LogContent = "BatchDelete Account " + ids;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(BoDB.AccountDb.DeleteByIds(ids.Split(',')));
        }

        public bool CheckNameExt(string userName)
        {
            Account account = BoDB.DB.Queryable<Account>().Where(t => t.Username == userName).First();
            return account != null;
        }
    }
}