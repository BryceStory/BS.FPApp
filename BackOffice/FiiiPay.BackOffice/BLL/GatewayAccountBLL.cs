using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Framework;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class GatewayAccountBLL : BaseBLL
    {
        public SaveResult UpdatePassword(Guid accountId, string newPassord, int userId, string userName)
        {
            GatewayAccount oldAccount = FiiiPayEnterpriseDB.AccountsDb.GetById(accountId);
            oldAccount.Password = PasswordHasher.HashPassword(newPassord);

            RedisHelper.KeyDelete(3, "FiiiEnterpriseWeb:LoginVerification:" + oldAccount.Username);

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".UpdatePIN";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Gateway Acount PIN " + accountId;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayEnterpriseDB.AccountsDb.Update(oldAccount));
        }
        public SaveResult Create(GatewayAccountViewModel model, int userId, string userName)
        {
            GatewayAccount account = new GatewayAccount();
            account.Id = model.Id;
            account.Username = model.Username;
            account.Password = PasswordHasher.HashPassword(model.Password);

            account.PhoneCode = "";
            account.Cellphone = "";
            account.MerchantName = "";
            account.Balance = 0;
            account.Status = GayewayAccountStatus.Active;
            account.CallbackUrl = "";
            account.IsVerifiedEmail = false;
            account.CountryId = 0;
            account.Currency = "";
            account.RegistrationDate = DateTime.UtcNow;
            account.SecretKey = "";
            account.IsAllowPayment = true;

            GatewayProfile profile = new GatewayProfile();
            profile.AccountId = model.Id;
            profile.CompanyName = model.CompanyName;
            profile.Cellphone = "";
            profile.LicenseNo = model.LicenseNo;
            profile.LastName = model.LastName;
            profile.FirstName = model.FirstName;
            profile.IdentityDocNo = model.IdentityDocNo;
            profile.IdentityDocType = model.IdentityDocType;
            profile.IdentityExpiryDate = DateTime.UtcNow;
            profile.Address1 = "";
            profile.City = "";
            profile.State = "";
            profile.Postcode = "";
            profile.Country = 0;
            profile.FrontIdentityImage = model.FrontIdentityImage;
            profile.BackIdentityImage = model.BackIdentityImage;
            profile.HandHoldWithCard = model.HandHoldWithCard;
            profile.BusinessLicenseImage = model.BusinessLicenseImage;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create GatewayAccount " + model.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            FiiiPayEnterpriseDB.AccountsDb.Insert(account);
            FiiiPayEnterpriseDB.ProfilesDb.Insert(profile);
            return new SaveResult(true);
        }

        public SaveResult Update(GatewayAccountViewModel model, int userId, string userName)
        {
            GatewayAccount account = FiiiPayEnterpriseDB.AccountsDb.GetById(model.Id);
            account.Id = model.Id;
            account.Username = model.Username;
            if (!string.IsNullOrWhiteSpace(model.Password))
                account.Password = PasswordHasher.HashPassword(model.Password);

            GatewayProfile profile = FiiiPayEnterpriseDB.ProfilesDb.GetById(model.Id);
            profile.AccountId = model.Id;
            profile.CompanyName = model.CompanyName;
            profile.LicenseNo = model.LicenseNo;
            profile.LastName = model.LastName;
            profile.FirstName = model.FirstName;
            profile.IdentityDocNo = model.IdentityDocNo;
            profile.IdentityDocType = model.IdentityDocType;

            if (model.FrontIdentityImage != Guid.Empty)
                profile.FrontIdentityImage = model.FrontIdentityImage;
            if (model.BackIdentityImage != Guid.Empty)
                profile.BackIdentityImage = model.BackIdentityImage;
            if (model.HandHoldWithCard != Guid.Empty)
                profile.HandHoldWithCard = model.HandHoldWithCard;
            if (model.BusinessLicenseImage != Guid.Empty)
                profile.BusinessLicenseImage = model.BusinessLicenseImage;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update GatewayAccount " + model.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            FiiiPayEnterpriseDB.AccountsDb.Update(account);
            FiiiPayEnterpriseDB.ProfilesDb.Update(profile);
            return new SaveResult(true);
        }

        public SaveResult Deduct(Guid accountId, decimal amount, int userId, string userName)
        {
            GatewayAccount account = FiiiPayEnterpriseDB.AccountsDb.GetById(accountId);
            account.Balance -= amount;

            BalanceStatement bs = new BalanceStatement();
            bs.AccountId = accountId;
            bs.Action = "Deduct";
            bs.Amount = amount;
            bs.Balance = account.Balance;
            bs.Timestamp = DateTime.UtcNow;
            bs.Remark = "BackOffice Deduct";

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(CryptocurrenciesBLL).FullName + ".Deduct";
            actionLog.Username = userName;
            actionLog.LogContent = "Deduct GatewayAccount " + accountId;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            FiiiPayEnterpriseDB.AccountsDb.Update(account);
            FiiiPayEnterpriseDB.BalanceStatementDb.Insert(bs);
            return new SaveResult(true);
        }
    }
}