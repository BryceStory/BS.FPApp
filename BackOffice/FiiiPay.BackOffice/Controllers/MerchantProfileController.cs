using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using FiiiPay.Framework.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Entities.Enums;
using FiiiPay.BackOffice.ViewModels;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("MerchantProfileMenu")]
    public class MerchantProfileController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = GetCountrySelectList();

            List<VerifyRecordViewModel> list = new VerifyRecordBLL().GetVerifyCount(UserName, "SaveMerchantProfileVerifyL2");
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
        public JsonResult LoadData(string keyword,string sn, int? countryId, int? status, GridPager pager)
        {
            List<MerchantProfile> profileList;
            List<MerchantProfileAccountViewModel> merchantPos;
            string countryName;


            if (string.IsNullOrEmpty(sn))
            {
                if (!countryId.HasValue)
                    return Json(new List<MerchantProfile>().ToGridJson(pager));

                profileList = new MerchantProfileBLL().GetMerchantProfilePageListL2(keyword, countryId.Value, status, ref pager);
                if (profileList == null || profileList.Count <= 0)
                    return Json(new List<MerchantProfile>().ToGridJson(pager));

                var merchantIds = profileList.Select(t => t.MerchantId).ToArray();
                countryName = FoundationDB.CountryDb.GetById(countryId)?.Name;
                merchantPos = FiiiPayDB.DB.Queryable<MerchantAccounts, POSs>((u, v) => new object[] { JoinType.Left, u.POSId == v.Id })
                .Where((u, v) => SqlFunc.ContainsArray<Guid>(merchantIds, u.Id))
                .Select((u, v) => new MerchantProfileAccountViewModel { MerchantId = u.Id, Cellphone = u.Cellphone, MerchantName = u.MerchantName, SN = v.Sn, Username = u.Username }).ToList();
            }
            else
            {
                var pos = FiiiPayDB.POSDb.GetSingle(t => t.Sn == sn);
                if (pos == null)
                    return Json(new List<MerchantProfile>().ToGridJson(pager));
                var merchantAccount = FiiiPayDB.MerchantAccountDb.GetSingle(t => t.POSId == pos.Id);
                if (merchantAccount == null)
                    return Json(new List<MerchantProfile>().ToGridJson(pager));
                var profile = new MerchantProfileBLL().GetMerchantProfile(merchantAccount.Id);
                if (profile == null)
                    return Json(new List<MerchantProfile>().ToGridJson(pager));
                
                profileList = new List<MerchantProfile> { profile };

                profileList = profileList.Where(t =>
                    (keyword == null ? true : t.Cellphone.Contains(keyword))
                    && (countryId.HasValue ? t.Country == countryId.Value : true)
                    && (status.HasValue ? (byte)t.L1VerifyStatus == status.Value : true)
                    ).ToList();

                countryName = FoundationDB.CountryDb.GetById(merchantAccount.CountryId)?.Name;

                merchantPos = new List<MerchantProfileAccountViewModel>
                {
                    new MerchantProfileAccountViewModel
                    {
                        MerchantId = merchantAccount.Id,
                        MerchantName = merchantAccount.MerchantName,
                        Cellphone = merchantAccount.Cellphone,
                        Username = merchantAccount.Username,
                        SN = pos.Sn
                    }
                };
            }

            var data = profileList.ToGridJson(pager, r =>
                    new
                    {
                        id = r.MerchantId,
                        cell = new
                        {
                            MerchantId = r.MerchantId,
                            Cellphone = r.Cellphone,
                            Username = merchantPos.Where(t => t.MerchantId == r.MerchantId).Select(t => t.Username).FirstOrDefault(),
                            SN = merchantPos.Where(t => t.MerchantId == r.MerchantId).Select(t => t.SN).FirstOrDefault(),
                            MerchantName = merchantPos.Where(t => t.MerchantId == r.MerchantId).Select(t => t.MerchantName).FirstOrDefault(),
                            CountryName = countryName,
                            VerifyStatus = r.L2VerifyStatus.ToString(),
                            SubmissionDate = r.L2SubmissionDate.HasValue ? r.L2SubmissionDate.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : "",
                            Remark = r.L2Remark,
                            Options = (int)r.L2VerifyStatus
                        }
                    });
            return Json(data);
        }

        public ActionResult Detail(Guid id)
        {
            var account = FiiiPayDB.MerchantAccountDb.GetById(id);
            ViewBag.AccountName = account.Username;
            var pro = new MerchantProfileBLL().GetMerchantProfile(id);
            return View(pro);
        }

        public ActionResult Verify(Guid id)
        {
            var account = FiiiPayDB.MerchantAccountDb.GetById(id);
            ViewBag.AccountName = account.Username;
            var pro = new MerchantProfileBLL().GetMerchantProfile(id);
            ViewBag.VerifyStatusList = GetVerifyStatusList();
            return View(pro);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("MerchantProfileApprove")]
        public JsonResult SaveVerify(MerchantProfile profile)
        {
            var sr = new MerchantProfileBLL().SaveMerchantProfileVerifyL2(UserId, UserName, profile);
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