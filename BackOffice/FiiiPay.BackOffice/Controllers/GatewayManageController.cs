using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Framework.Component;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("GatewayAccountManageMenu")]
    public class GatewayManageController : BaseController
    {
        // GET: GatewayManage
        public ActionResult Index(int id)
        {
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "All" });
            statusList.AddRange(EnumHelper.EnumToList<GayewayAccountStatus>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            ViewBag.StatusList = statusList;
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public ActionResult LoadData(GridPager pager, string username, GayewayAccountStatus? status)
        {
            var data = FiiiPayEnterpriseDB.DB.Queryable<GatewayAccount>().
                WhereIF(!string.IsNullOrWhiteSpace(username), t => t.Username.Contains(username)).
                WhereIF(status.HasValue, t => t.Status == status).ToList();
            var obj = data.ToGridJson(ref pager,
                r => new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id.ToString(),
                        Username = r.Username,
                        MerchantName = r.MerchantName,
                        Email = r.Email,
                        Balance = r.Balance,
                        Status = r.Status.ToString(),
                        RegistrationDate = r.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadDeduct(GridPager pager, string username, DateTime? startDate, DateTime? endDate)
        {
            var data = FiiiPayEnterpriseDB.DB.Queryable<GatewayAccount, BalanceStatement>((account, balance) => new object[] {
                JoinType.Left,account.Id == balance.AccountId}).
                WhereIF(!string.IsNullOrWhiteSpace(username), (account, balance) => account.Username.Contains(username)).
                WhereIF(startDate.HasValue, (account, balance) => balance.Timestamp > startDate.Value.AddDays(-1)).
                WhereIF(endDate.HasValue, (account, balance) => balance.Timestamp < endDate.Value.AddDays(1)).
                Select<GatewayDeductViewModel>((account, balance) => new GatewayDeductViewModel
                {
                    Username = account.Username,
                    Amount = balance.Amount,
                    Timestamp = balance.Timestamp
                }).ToList();
            var obj = data.ToGridJson(ref pager,
                r => new
                {
                    cell = new
                    {
                        Username = r.Username,
                        Amount = r.Amount,
                        Timestamp = r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(Guid Id)
        {
            var identityDocTypeList = new List<SelectListItem>();
            identityDocTypeList.Add(new SelectListItem() { Text = "IdentityCard", Value = "1" });
            identityDocTypeList.Add(new SelectListItem() { Text = "Passport", Value = "2" });
            ViewBag.IdentityDocTypeList = identityDocTypeList;
            GatewayAccountViewModel model = new GatewayAccountViewModel();
            if (Id != Guid.Empty)
            {
                model = GetViewModel(Id);
            }
            return View(model);
        }

        public ActionResult Detail(Guid Id)
        {
            GatewayAccountViewModel model = GetViewModel(Id);
            return View(model);
        }

        private GatewayAccountViewModel GetViewModel(Guid Id)
        {
            var model = FiiiPayEnterpriseDB.DB.Queryable<GatewayAccount, GatewayProfile>((account, profile) => new object[] {
                JoinType.Left,account.Id == profile.AccountId}).Where((account, profile) => account.Id == Id).
                Select<GatewayAccountViewModel>((account, profile) => new GatewayAccountViewModel
                {
                    Id = account.Id,
                    Username = account.Username,
                    Password = "",
                    Email = account.Email,
                    CompanyName = profile.CompanyName,
                    LicenseNo = profile.LicenseNo,
                    BusinessLicenseImage = profile.BusinessLicenseImage,
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    IdentityDocNo = profile.IdentityDocNo,
                    IdentityDocType = profile.IdentityDocType,
                    FrontIdentityImage = profile.FrontIdentityImage,
                    BackIdentityImage = profile.BackIdentityImage,
                    HandHoldWithCard = profile.HandHoldWithCard
                }).First();

            return model;
        }

        public ActionResult ResetPassword(Guid Id)
        {
            var model = FiiiPayEnterpriseDB.AccountsDb.GetById(Id);
            return PartialView(model);
        }

        public ActionResult Deduct(Guid Id)
        {
            var model = FiiiPayEnterpriseDB.AccountsDb.GetById(Id);
            return PartialView(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("ResetGatewayAccountPassword")]
        public ActionResult SavePassword(Guid AccountId, string NewPassword)
        {
            GatewayAccountBLL ab = new GatewayAccountBLL();
            return Json(ab.UpdatePassword(AccountId, NewPassword, UserId, UserName).toJson());
        }
        public ActionResult Save(GatewayAccountViewModel model)
        {
            //上传图片
            HttpPostedFileBase BusinessLicenseFile = Request.Files["LicenseUrl"];
            if (BusinessLicenseFile.ContentLength != 0)
            {
                model.BusinessLicenseImage = new Guid(new Utils.FileUploader().UpImageToCDN(BusinessLicenseFile));
            }
            HttpPostedFileBase FrontIdentityFile = Request.Files["FrontIdentityUrl"];
            if (FrontIdentityFile.ContentLength != 0)
            {
                model.FrontIdentityImage = new Guid(new Utils.FileUploader().UpImageToCDN(FrontIdentityFile));
            }
            HttpPostedFileBase BackIdentityFile = Request.Files["BackIdentityUrl"];
            if (BackIdentityFile.ContentLength != 0)
            {
                model.BackIdentityImage = new Guid(new Utils.FileUploader().UpImageToCDN(BackIdentityFile));
            }
            HttpPostedFileBase HandHoldWithCardFile = Request.Files["HandHoldWithCardUrl"];
            if (HandHoldWithCardFile.ContentLength != 0)
            {
                model.HandHoldWithCard = new Guid(new Utils.FileUploader().UpImageToCDN(HandHoldWithCardFile));
            }
            SaveResult result = new SaveResult();

            var identityDocTypeList = new List<SelectListItem>();
            identityDocTypeList.Add(new SelectListItem() { Text = "IdentityCard", Value = "1" });
            identityDocTypeList.Add(new SelectListItem() { Text = "Passport", Value = "2" });
            ViewBag.IdentityDocTypeList = identityDocTypeList;

            if (model.Id != Guid.Empty)//编辑
            {
                SaveEdit(model);
                var oldModel = GetViewModel(model.Id);
                return View("Edit", oldModel);
            }
            else//新增
            {
                model.Id = Guid.NewGuid();
                SaveCreate(model);
                var newModel = GetViewModel(model.Id);
                return View("Edit", newModel);
            }
        }

        [BOAccess("GatewayAccountCreate")]
        private SaveResult SaveCreate(GatewayAccountViewModel model)
        {
            GatewayAccountBLL ab = new GatewayAccountBLL();
            var result = ab.Create(model, UserId, UserName);
            return result;
        }

        [BOAccess("GatewayAccountUpdate")]
        private SaveResult SaveEdit(GatewayAccountViewModel model)
        {
            GatewayAccountBLL ab = new GatewayAccountBLL();
            return ab.Update(model, UserId, UserName);
        }

        [BOAccess("GatewayAccountDeduct")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult SaveDeduct(Guid accountId, decimal amount)
        {
            GatewayAccountBLL ab = new GatewayAccountBLL();
            return Json(ab.Deduct(accountId, amount, UserId, UserName).toJson());
        }

    }
}