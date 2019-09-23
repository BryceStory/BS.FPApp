using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Framework;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Framework.Component;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("AccountMenu")]
    public class AccountController : BaseController
    {

        // GET: Account
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public JsonResult LoadData(GridPager pager, string userName)
        {
            List<AccountViewModel> AccountList = BoDB.DB.Queryable<AccountViewModel, AccountRole>((account, role) => new object[] {
                JoinType.Left,account.RoleId == role.Id}).WhereIF(!string.IsNullOrEmpty(userName), (account, role) => account.Username.Contains(userName)).Select((account, role) =>
                new AccountViewModel { Id = account.Id, Username = account.Username, Rolename = role.Name, Cellphone = account.Cellphone, Email = account.Email }).ToList();
            var obj = AccountList.ToGridJson(ref pager, r =>
                new
                {
                    id = r.Id,
                    cell = new string[]{
                        r.Id.ToString(),
                        r.Username,
                        r.Rolename,
                        r.Cellphone,
                        r.Email
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int AccountId)
        {
            AccountBLL ab = new AccountBLL();
            List<AccountRole> list = BoDB.AccountRoleDb.GetList();
            List<SelectListItem> oList = new List<SelectListItem>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    oList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            ViewBag.RoleList = oList;
            AccountViewModel account = new AccountViewModel();
            account.Id = -1;
            if (AccountId > 0)
            {
                account = ab.GetViewAccountById(AccountId);
            }
            return PartialView(account);
        }

        public ActionResult Detail(int AccountId)
        {
            AccountBLL ab = new AccountBLL();
            //if (!(AccountId == UserId || PagePermission.View))
            //    throw new Exception("You have no premission to view this page");
            AccountViewModel account = ab.GetViewAccountById(AccountId);
            ViewBag.IsSelft = UserId == AccountId;
            return View(account);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(Account oAccount)
        {
            if (oAccount.Id > 0)//编辑
            {
                return Json(SaveEdit(oAccount).toJson());
            }
            else//新增
            {
                return Json(SaveCreate(oAccount).toJson());
            }
        }

        [BOAccess("AccountCreate")]
        private SaveResult SaveCreate(Account oAccount)
        {
            AccountBLL ab = new AccountBLL();
            oAccount.CreateTime = DateTime.UtcNow;
            oAccount.CreateBy = UserId;
            oAccount.Password = PasswordHasher.HashPassword(oAccount.Password);
            return ab.Create(oAccount, UserId, UserName);
        }

        [BOAccess("AccountUpdate")]
        private SaveResult SaveEdit(Account oAccount)
        {
            AccountBLL ab = new AccountBLL();
            oAccount.ModifyBy = UserId;
            return ab.Update(oAccount, UserId, UserName);
        }

        public ActionResult ResetPassword(int AccountId)
        {
            //if (!(AccountId == UserId || PagePermission.View))
            //    throw new Exception("You have no premission to view this page");
            Account account = BoDB.AccountDb.GetById(AccountId);
            ViewBag.UserId = UserId;
            return PartialView(account);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("AccountUpdate")]
        public ActionResult SavePassword(int AccountId, string oldPassword, string newPassword)
        {
            //if (!(AccountId == UserId || PagePermission.View))
            //    throw new Exception("You have no premission to view this page");
            AccountBLL ab = new AccountBLL();
            Account account = BoDB.AccountDb.GetById(AccountId);
            if (AccountId == UserId && !PasswordHasher.VerifyHashedPassword(account.Password, oldPassword))
            {
                return Json(new SaveResult(false, "Password Error"));
            }
            else
            {
                account.Password = PasswordHasher.HashPassword(newPassword);
                return Json(ab.UpdatePassword(account, UserId, UserName).toJson());
            }
        }
        [BOAccess("AccountDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int accountId)
        {
            AccountBLL ab = new AccountBLL();
            return Json(ab.DeleteById(accountId, UserId, UserName).toJson());
        }

        public PartialViewResult ChangePassword()
        {
            Account account = new Account() { Id = UserId, Username = UserName };
            return PartialView(account);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult SaveChangePassword(int AccountId, string oldPassword, string newPassword)
        {
            if (AccountId != UserId)
                throw new Exception("You have no premission to view this page");
            AccountBLL ab = new AccountBLL();
            Account account = BoDB.AccountDb.GetById(AccountId);
            if (AccountId == UserId && !PasswordHasher.VerifyHashedPassword(account.Password, oldPassword))
            {
                return Json(new SaveResult(false, "Password Error"));
            }
            else
            {
                account.Password = PasswordHasher.HashPassword(newPassword);
                return Json(ab.UpdatePassword(account, UserId, UserName).toJson());
            }
        }
    }
}