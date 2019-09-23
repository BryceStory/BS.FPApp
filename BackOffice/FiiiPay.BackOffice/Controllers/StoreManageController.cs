using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("StoreManageMenu")]
    public class StoreManageController : BaseController
    {
        // GET: StoreManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = GetCountrySelectList();

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "All" });
            statusList.AddRange(EnumHelper.EnumToList<VerifyStatus>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            ViewBag.StatusList = statusList;
            return View();
        }
        public JsonResult LoadData(GridPager pager, string merchantName, string userName, string cellPhone, int? countryId, int? status)
        {
            var countryName = FoundationDB.CountryDb.GetById(countryId)?.Name;
            var statusList = GetMerchantStatusList();
            var verifyStatusList = GetMerchantVerfiyStatusList();
            var list = new StoreManageBLL().GetFiiiPosMerchantPager(merchantName, userName, cellPhone, countryId, status, ref pager);

            var obj = list.ToGridJson(pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            Id = r.Id.ToString(),
                            Username = r.Username,
                            Cellphone = r.Cellphone,
                            MerchantName = r.MerchantName,
                            CountryName = countryName,
                            Status = statusList[(int)r.Status],
                            VerifyStatus = verifyStatusList[r.VerifyStatus]
                        }
                    });
            return Json(obj);
        }

        private Dictionary<int, string> GetMerchantStatusList()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)Status.Enabled, "正常");
            dic.Add((int)Status.Stop, "禁用");
            return dic;
        }

        private Dictionary<int, string> GetMerchantVerfiyStatusList()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)VerifyStatus.Uncertified, "未提交");
            dic.Add((int)VerifyStatus.UnderApproval, "审核中");
            dic.Add((int)VerifyStatus.Certified, "审核通过");
            dic.Add((int)VerifyStatus.Disapproval, "驳回");
            return dic;
        }

        public ActionResult StoreSet(Guid id)
        {
            var model = FiiiPayDB.MerchantInformationDb.GetById(id);

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
            statusList.Add(new SelectListItem() { Text = "Stop", Value = "0" });
            ViewBag.StatusList = statusList;

            return PartialView(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult SaveSet(MerchantInformations oModel)
        {
            StoreManageBLL upb = new StoreManageBLL();
            return Json(upb.SetUser(oModel, UserId, UserName).toJson());
        }
        public ActionResult Detail(Guid id)
        {
            var model = FiiiPayDB.MerchantInformationDb.GetById(id);
            var figureList = FiiiPayDB.DB.Queryable<MerchantOwnersFigures>().Where(t => t.MerchantInformationId == id).ToList();
            var recommendList = FiiiPayDB.DB.Queryable<MerchantRecommends>().Where(t => t.MerchantInformationId == id).ToList();
            var storeTypeList = FiiiPayDB.DB.Queryable<MerchantCategorys, StoreTypes>((m, s) => new object[] { JoinType.Inner, m.Category == s.Id })
                .Where((m, s) => (m.MerchantInformationId == id)).Select((m, s) => s.Name_EN).ToList();

            List<string> weekList = new List<string>();
            foreach (var item in EnumHelper.EnumToList<Week>())
            {
                if (CheckWeekOpen((byte)model.Week, (Week)item.EnumValue))
                    weekList.Add(item.EnumName);
            }

            ViewBag.StoreTypeStr = string.Join(",", storeTypeList);
            ViewBag.WeekStr = string.Join(",", weekList);
            ViewBag.FigureList = figureList;
            ViewBag.RecommendList = recommendList;

            return View(model);
        }

        public ActionResult Verify(Guid id)
        {
            var model = FiiiPayDB.MerchantInformationDb.GetById(id);
            var figureList = FiiiPayDB.DB.Queryable<MerchantOwnersFigures>().Where(t => t.MerchantInformationId == id).ToList();
            var recommendList = FiiiPayDB.DB.Queryable<MerchantRecommends>().Where(t => t.MerchantInformationId == id).ToList();
            var storeTypeList = FiiiPayDB.DB.Queryable<MerchantCategorys, StoreTypes>((m, s) => new object[] { JoinType.Inner, m.Category == s.Id })
                .Where((m, s) => (m.MerchantInformationId == id)).Select((m, s) => s.Name_EN).ToList();

            List<string> weekList = new List<string>();
            foreach (var item in EnumHelper.EnumToList<Week>())
            {
                if (CheckWeekOpen((byte)model.Week, (Week)item.EnumValue))
                    weekList.Add(item.EnumName);
            }

            ViewBag.StoreTypeStr = string.Join(",", storeTypeList);
            ViewBag.WeekStr = string.Join(",", weekList);
            ViewBag.FigureList = figureList;
            ViewBag.RecommendList = recommendList;
            ViewBag.VerifyStatusList = GetVerifyStatusList();

            return View(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("StoreApprove")]
        public JsonResult SaveVerify(MerchantInformations info)
        {
            var sr = new StoreManageBLL().SaveProfileVerify(info, UserId, UserName);
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

        public static bool CheckWeekOpen(byte? value, Week flag)
        {
            if (!value.HasValue || value.Value == 0)
                return false;
            Week validate = (Week)Enum.Parse(typeof(Week), value.Value.ToString());
            return (validate & flag) != 0;
        }
    }
}