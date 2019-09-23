using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using FiiiPay.Framework.Enums;
using System;
using System.Linq;
using FiiiPay.Framework;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework.Cache;

namespace FiiiPay.BackOffice.BLL
{
    public class UserManageBLL : BaseBLL
    {

        public SaveResult SetUser(UserAccount model, int userId, string userName)
        {
            var user = FiiiPayDB.UserAccountDb.GetById(model.Id);
            user.Status = model.Status;
            user.IsAllowExpense = model.IsAllowExpense;
            user.IsAllowWithdrawal = model.IsAllowWithdrawal;
            user.IsAllowTransfer = model.IsAllowTransfer;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserManageBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Set User Status,Id:{0}", model.Id);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.UserAccountDb.Update(user));
        }

        public SaveResult UpdatePassword(UserAccounts account, int userId, string userName, string Type)
        {
            UserAccounts oldAccount = FiiiPayDB.UserAccountDb.GetById(account.Id);
            if (Type.Equals("Password"))
            {
                oldAccount.Password = account.Password;

                var securityVerify = new SecurityVerification(SystemPlatform.FiiiPay);
                securityVerify.DeleteErrorCount(SecurityMethod.Password, account.Id.ToString());
            }
            else if (Type.Equals("PIN"))
            {
                oldAccount.Pin = account.Pin;

                var securityVerify = new SecurityVerification(SystemPlatform.FiiiPay);
                securityVerify.DeleteErrorCount(SecurityMethod.Pin, account.Id.ToString());
            }

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserManageBLL).FullName + ".UpdatePassword";
            actionLog.Username = userName;
            actionLog.LogContent = "Update User Password " + account.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.UserAccountDb.Update(oldAccount));
        }

        public SaveResult GoogleUnbind(Guid id, int userId, string userName)
        {
            var oldAccount = FiiiPayDB.UserAccountDb.GetById(id);
            oldAccount.AuthSecretKey = "";
            oldAccount.ValidationFlag = FiiiPay.BackOffice.Common.ValidationFlagComponent.ReduceValidationFlag(oldAccount.ValidationFlag, ValidationFlag.GooogleAuthenticator);
            FiiiPayDB.UserAccountDb.Update(oldAccount);

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserManageBLL).FullName + ".GoogleUnbind";
            actionLog.Username = userName;
            actionLog.LogContent = "Unbind " + id;
            new ActionLogBLL().Create(actionLog);


            return new SaveResult(true, "Save Success");
        }

        public SaveResult Reward(Guid id, int userId, string userName)
        {
            var settingCollection = new MasterSettingDAC().SelectByGroup("InviteReward");
            var inviteMoney = decimal.Parse(settingCollection.Where(item => item.Name == "Invite_Reward_Amount").Select(item => item.Value).FirstOrDefault());

            var uwComponent = new UserWalletBLL();
            var uwsDAC = new UserWalletStatementDAC();
            var uwDAC = new UserWalletDAC();

            var cryptoId = new CryptocurrencyDAC().GetByCode("FIII").Id;
            var inviteWallet = uwComponent.GetUserWallet(id, cryptoId);
            if (inviteWallet == null)
                inviteWallet = uwComponent.GenerateWallet(id, cryptoId);

            var adoResult = FiiiPayDB.DB.Ado.UseTran(() =>
            {
                //解冻奖励
                uwDAC.Increase(inviteWallet.Id, inviteMoney);
                //插入奖励流水
                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = inviteWallet.Id,
                    Action = UserWalletStatementAction.Invite,
                    Amount = inviteMoney,
                    Balance = inviteWallet.Balance + inviteMoney,
                    Timestamp = DateTime.UtcNow,
                    Remark = "BO Reward"
                });
            });
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserManageBLL).FullName + ".Reward";
            actionLog.Username = userName;
            actionLog.LogContent = "Reward " + id + " Fiii:" + inviteMoney;
            new ActionLogBLL().Create(actionLog);


            return new SaveResult(true, "Save Success");
        }
        public SaveResult DeleteErrorCount(Guid id, string type, int userId, string userName)
        {
            var securityVerify = new SecurityVerification(SystemPlatform.FiiiPay);
            if (type.Equals("PIN"))
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new PinVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
            if (type.Equals("SMS"))
            {
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new CellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new RegisterCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new LoginCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new ForgetPasswordCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdatePinCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new ResetPinCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdatePasswordCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdateCellphoneOldVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new UpdateCellphoneNewVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new MandatoryCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new BindAccountCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new ModifyCellphoneVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));
            }
            if (type.Equals("GoogleAuth"))
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new GoogleVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));

            if (type.Equals("Identity"))
                RedisHelper.KeyDelete(Constant.REDIS_SMS_DBINDEX, new MandatoryVerifier().GetErrorCountKey(SystemPlatform.FiiiPay, id.ToString()));


            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserManageBLL).FullName + ".DeleteErrorCount";
            actionLog.Username = userName;
            actionLog.LogContent = "DeleteErrorCount " + id + ":" + type;
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(true, "Save Success");
        }
    }
}