using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("UserManageMenu")]
    public class UserManageController : BaseController
    {
        // GET: UserManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = GetCountrySelectList();

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "All" });
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
            statusList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
            ViewBag.StatusList = statusList;
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(string cellphone, int? countryId, int? status, GridPager pager)
        {
            if (!countryId.HasValue)
                return Json(new List<UserAccountStatus>().ToGridJson(pager));
            var profileList = new UserProfileBLL().GetUserAccountStatusList(cellphone, countryId.Value, status, ref pager);
            if (profileList == null || profileList.Count <= 0)
                return Json(new List<UserAccountStatus>().ToGridJson(pager));

            var accountIds = profileList.Select(t => t.UserAccountId.Value).ToList();
            var countryName = FoundationDB.CountryDb.GetById(countryId).Name;

            var userList = FiiiPayDB.UserAccountDb.GetList(t => accountIds.Contains(t.Id));
            var data = profileList.ToGridJson(pager, r =>
                    new
                    {
                        id = r.UserAccountId,
                        cell = new
                        {
                            Id = r.UserAccountId,
                            AccountNo = r.Cellphone,
                            CountryName = countryName,
                            ProfileVerify = r.L1VerifyStatus.ToString(),
                            AddressVerify = r.L2VerifyStatus.ToString(),
                            IsAllowWithdrawal = r.IsAllowWithdrawal,
                            IsAllowAccept = r.IsAllowExpense,
                            Status = (r.Status == 1) ? "Enable" : "Disable",
                            RegisterDate = r.RegistrationDate.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                            LastLoginDate = r.LastLoginTimeStamp.HasValue ? r.LastLoginTimeStamp.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : ""
                        }
                    });
            return Json(data);
        }

        public ActionResult UserSet(Guid id)
        {
            var model = FiiiPayDB.UserAccountDb.GetById(id);
            if (!model.Status.HasValue)
            {
                model.Status = 0;
            }
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
            statusList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
            ViewBag.StatusList = statusList;


            var isList = new List<SelectListItem>();
            isList.Add(new SelectListItem() { Text = "Enable", Value = "true" });
            isList.Add(new SelectListItem() { Text = "Disable", Value = "false" });
            ViewBag.IsList = isList;

            return PartialView(model);
        }

        public ActionResult Detail(Guid id)
        {
            var pro = new UserProfileBLL().GetUserProfileSet(id);
            pro.CountryName = FoundationDB.CountryDb.GetById(pro.CountryId).Name;
            return View(pro);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(UserAccount oModel)
        {
            UserManageBLL upb = new UserManageBLL();
            return Json(upb.SetUser(oModel, UserId, UserName).toJson());
        }

        private List<SelectListItem> GetCountrySelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var countryList = FoundationDB.CountryDb.GetList();
            if (countryList != null && countryList.Count > 0)
            {
                oList.AddRange(countryList.Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString() }));
            }
            return oList;
        }
        private List<SelectListItem> GetVerifyStatusList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = VerifyStatus.UnderApproval.ToString(), Value = ((int)VerifyStatus.UnderApproval).ToString() });
            oList.Add(new SelectListItem() { Text = VerifyStatus.Certified.ToString(), Value = ((int)VerifyStatus.Certified).ToString() });
            oList.Add(new SelectListItem() { Text = VerifyStatus.Disapproval.ToString(), Value = ((int)VerifyStatus.Disapproval).ToString() });
            return oList;
        }

        public ActionResult ResetPassword(string Id, string Type)
        {
            UserAccounts account = FiiiPayDB.UserAccountDb.GetById(Id);
            ViewBag.UserId = UserId;
            ViewBag.Type = Type;

            return PartialView(account);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("ResetUserPassword")]
        public ActionResult SavePassword(string AccountId, string NewPassword, string Type)
        {
            UserManageBLL ab = new UserManageBLL();
            UserAccounts account = FiiiPayDB.UserAccountDb.GetById(AccountId);
            if (Type.Equals("Password"))
            {
                account.Password = PasswordHasher.HashPassword(NewPassword);
            }
            else if (Type.Equals("PIN"))
            {
                account.Pin = PasswordHasher.HashPassword(NewPassword);
            }
            else
            {
                return Json(new SaveResult(false).toJson());
            }
            return Json(ab.UpdatePassword(account, UserId, UserName, Type).toJson());
        }

        public ActionResult UserAccount(Guid id)
        {
            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            var list = FiiiPayDB.DB.Queryable<UserWallets>().Where(t => t.UserAccountId == id).ToList();
            List<WalletsViewModel> walletsList = new List<WalletsViewModel>();
            foreach (var item in list)
            {
                WalletsViewModel model = new WalletsViewModel();
                model.Balance = item.Balance;
                model.CurrencyName = coinList.Where(t => t.Id == item.CryptoId).Select(t => t.Name).FirstOrDefault();
                model.FrozenBalance = item.FrozenBalance;
                walletsList.Add(model);
            }
            ViewBag.WalletsList = walletsList;
            return View();
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Unbind(Guid id)
        {
            UserManageBLL upb = new UserManageBLL();
            return Json(upb.GoogleUnbind(id, UserId, UserName).toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Reward(Guid id)
        {
            UserManageBLL upb = new UserManageBLL();
            return Json(upb.Reward(id, UserId, UserName).toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult DeleteErrorCount(Guid id, string type)
        {
            UserManageBLL upb = new UserManageBLL();
            return Json(upb.DeleteErrorCount(id, type, UserId, UserName).toJson());
        }
    }
}