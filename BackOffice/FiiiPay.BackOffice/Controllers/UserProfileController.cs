using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities;
using FiiiPay.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Entities.Enums;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("UserProfileMenu")]
    public class UserProfileController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = GetCountrySelectList();
            List<VerifyRecordViewModel> list = new VerifyRecordBLL().GetVerifyCount(UserName, "SaveProfileVerify");
            var verifyCount = 0;
            if (list.Count > 0)
            {
                verifyCount = list.First().VerifyCount;
            }
            ViewBag.VerifyCount = verifyCount;
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "All" });
            statusList.AddRange(EnumHelper.EnumToList<VerifyStatus>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString(), Selected = t.EnumValue == (int)VerifyStatus.UnderApproval }));
            ViewBag.StatusList = statusList;

            return View();
        }

        [HttpPost]
        public JsonResult LoadData(string account, int? countryId, int? status, GridPager pager)
        {
            if (!countryId.HasValue)
                return Json(new List<UserProfileViewModel>().ToGridJson(pager));
            var profileList = new UserProfileBLL().GetUserProfilePageList(account, countryId.Value, status, ref pager);
            if (profileList == null || profileList.Count <= 0)
                return Json(new List<UserProfileViewModel>().ToGridJson(pager));

            var accountIds = profileList.Select(t => t.UserAccountId.Value).ToList();
            var countryName = FoundationDB.CountryDb.GetById(countryId).Name;

            var userList = FiiiPayDB.UserAccountDb.GetList(t => accountIds.Contains(t.Id));
            var data = profileList.ToGridJson(pager, r =>
                    new
                    {
                        id = r.UserAccountId,
                        cell = new
                        {
                            UserAccountId = r.UserAccountId,
                            Cellphone = userList.Where(t => t.Id == r.UserAccountId).Select(t => t.Cellphone).FirstOrDefault(),
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            CountryName = countryName,
                            L1VerifyStatus = r.L1VerifyStatus.HasValue ? ((VerifyStatus)r.L1VerifyStatus.Value).ToString() : "",
                            L1SubmissionDate = r.L1SubmissionDate.HasValue ? r.L1SubmissionDate.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : "",
                            Remark = r.L1Remark,
                            Options = r.L1VerifyStatus.HasValue ? (int)r.L1VerifyStatus : 0
                        }
                    });
            return Json(data);
        }

        public ActionResult Detail(Guid id)
        {
            var pro = new UserProfileBLL().GetUserProfile(id);
            var profile = ReflectionHelper.AutoCopy<Entities.UserProfile, UserProfileViewModel>(pro);
            profile.L1VerifyStatusName = profile.L1VerifyStatus.HasValue ? ((VerifyStatus)profile.L1VerifyStatus.Value).ToString() : "";
            profile.IdentityDocTypeName = profile.IdentityDocType.HasValue ? ((IdentityDocType)profile.IdentityDocType.Value).ToString() : "";
            var profileSDK = new UserProfileAgent();
            int count = profileSDK.GetCountByIdentityDocNo(profile.UserAccountId.Value, pro.IdentityDocNo);
            ViewBag.IdentityCount = count;
            return View(profile);
        }

        public ActionResult Verify(Guid id)
        {
            var account = FiiiPayDB.UserAccountDb.GetById(id);
            ViewBag.AccountName = account.PhoneCode + " " + account.Cellphone;

            var pro = new UserProfileBLL().GetUserProfile(id);
            var profile = ReflectionHelper.AutoCopy<Entities.UserProfile, UserProfileViewModel>(pro);
            profile.IdentityDocTypeName = profile.IdentityDocType.HasValue ? ((IdentityDocType)profile.IdentityDocType.Value).ToString() : "";
            ViewBag.VerifyStatusList = GetVerifyStatusList();

            var profileSDK = new UserProfileAgent();
            int count = profileSDK.GetCountByIdentityDocNo(profile.UserAccountId.Value, pro.IdentityDocNo);
            ViewBag.IdentityCount = count;

            return View(profile);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("UserProfileApprove")]
        public JsonResult SaveVerify(UserProfile profile)
        {
            var sr = new UserProfileBLL().SaveProfileVerify(UserId, UserName, profile);
            return Json(sr.toJson());
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
    }
}