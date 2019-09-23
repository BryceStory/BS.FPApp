using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("UserSettingMenu")]
    public class UserSettingController : BaseController
    {
        // GET: MasterSettings
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            List<MasterSettings> withdrawalList = FoundationDB.DB.Queryable<MasterSettings>().Where(r => r.Group == "UserWithdrawal").ToList();
            ViewBag.WithdrawalList = withdrawalList;
            return View();
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("UserSettingUpdate")]
        public ActionResult Save(List<string> olist)
        {
            MasterSettingBLL msb = new MasterSettingBLL();
            var result = msb.Update(olist, UserId, UserName, "UserWithdrawal");
            return Json(result.toJson());
        }
    }
}