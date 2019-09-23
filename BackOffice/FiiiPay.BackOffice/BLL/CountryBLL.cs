using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Foundation.Business;

namespace FiiiPay.BackOffice.BLL
{
    public class CountryBLL : BaseBLL
    {
        public SaveResult Update(CountryViewModel country, int userId, string userName)
        {
            CountryAgent agent = new CountryAgent();
            if (!agent.CheckProfileRouters(country.ProfileServerAddress, country.ProfileClientKey, country.ProfileSecretKey))
            {
                return new SaveResult(false, "ProfilesRouters connection failed");
            }
            var oldCountry = FoundationDB.CountryDb.GetById(country.Id);

            var result = BoDB.DB.Ado.UseTran(() =>
            {
                ProfileRouters pr = new ProfileRouters()
                {
                    Country = country.Id,
                    ServerAddress = country.ProfileServerAddress,
                    ClientKey = country.ProfileClientKey,
                    SecretKey = country.ProfileSecretKey
                };
                FoundationDB.ProfileRouterDb.Update(pr);

                oldCountry.Name = country.Name;
                oldCountry.Name_CN = country.Name_CN;
                oldCountry.Status = country.Status;
                oldCountry.PhoneCode = country.PhoneCode;
                oldCountry.PinYin = country.PinYin;
                //oldCountry.IsHot = country.IsHot;
                //oldCountry.CustomerService = country.CustomerService;
                oldCountry.FiatCurrency = country.FiatCurrency;
                oldCountry.Code = country.Code;
                oldCountry.IsSupportStore = country.IsSupportStore;
                //oldCountry.FiatCurrencySymbol = country.FiatCurrencySymbol;
                oldCountry.NationalFlagURL = country.NationalFlagURL;
                FoundationDB.CountryDb.Update(oldCountry);

            });

            

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(BORoleBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update Country " + country.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            

            return new SaveResult(true);
        }

        public SaveResult CheckRouters(string type, string ServerAddress, string ClientKey, string SecretKey)
        {
            CountryAgent agent = new CountryAgent();
            if (type.Equals("Profile"))
            {
                return new SaveResult(agent.CheckProfileRouters(ServerAddress, ClientKey, SecretKey));
            }
            else
            {
                return new SaveResult(false);
            }
        }


        public SaveResult<int> Create(CountryViewModel country, int userId, string userName)
        {
            CountryAgent agent = new CountryAgent();
            if (!agent.CheckProfileRouters(country.ProfileServerAddress, country.ProfileClientKey, country.ProfileSecretKey))
            {
                return new SaveResult<int>(false, "ProfilesRouters connection failed");
            }
            var model = new Countries();
            model.Name = country.Name;
            model.Name_CN = country.Name_CN;
            model.PhoneCode = country.PhoneCode;
            model.PinYin = country.PinYin;
            model.Status = country.Status;
            //model.IsHot = country.IsHot;
            model.IsSupportStore = country.IsSupportStore;
            //model.CustomerService = country.CustomerService;
            model.FiatCurrency = country.FiatCurrency;
            model.Code = country.Code;
            //model.FiatCurrencySymbol = country.FiatCurrencySymbol;
            model.NationalFlagURL = country.NationalFlagURL;
            int countryId = FoundationDB.CountryDb.InsertReturnIdentity(model);

            var tran = BoDB.DB.Ado.UseTran(() =>
            {
                ProfileRouters pr = new ProfileRouters()
                {
                    Country = countryId,
                    ServerAddress = country.ProfileServerAddress,
                    ClientKey = country.ProfileClientKey,
                    SecretKey = country.ProfileSecretKey
                };
                FoundationDB.ProfileRouterDb.Insert(pr);
            });
            

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(BORoleBLL).FullName + ".Create";
            actionLog.Username = userName;
            actionLog.LogContent = "Create Country " + country.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            var result = new SaveResult<int>();
            result.Data = countryId;
            return result;
        }

        public SaveResult DeleteById(int id, int userId, string userName)
        {
            var result = FoundationDB.CountryDb.DeleteById(id);
            

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".DeleteById";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete Country " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            
            return new SaveResult(result);
        }

        
    }
}