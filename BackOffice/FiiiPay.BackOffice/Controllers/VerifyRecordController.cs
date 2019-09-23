using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("VerifyRecordMenu")]
    public class VerifyRecordController : BaseController
    {
        // GET: VerifyRecord
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            var typeList = new List<SelectListItem>();

            typeList.Add(new SelectListItem() { Text = "All", Value = "" });
            typeList.Add(new SelectListItem() { Text = "UserProfile", Value = "SaveProfileVerify" });
            typeList.Add(new SelectListItem() { Text = "UserResidence", Value = "SaveResidenceVerify" });
            typeList.Add(new SelectListItem() { Text = "MerchantProfile", Value = "SaveMerchantProfileVerifyL1" });
            typeList.Add(new SelectListItem() { Text = "MerchantBusinessLicense", Value = "SaveMerchantProfileVerifyL2" });
            ViewBag.TypeList = typeList;

            return View();
        }
        public JsonResult LoadData(GridPager pager, string verifyAccount, string type, DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

            List<VerifyRecordViewModel> AccountList = new VerifyRecordBLL().GetVerifyCount(verifyAccount, type, startDate, endDate, ref pager);
            var obj = AccountList.ToGridJson(pager, r =>
                new
                {
                    cell = new
                    {
                        VerifyCount = r.VerifyCount.ToString(),
                        VerifyAccount = r.VerifyAccount
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}