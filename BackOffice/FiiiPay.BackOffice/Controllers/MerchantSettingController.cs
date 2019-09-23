using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("MerchantSettingMenu")]
    public class MerchantSettingController : BaseController
    {
        // GET: MasterSettings
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            List<MasterSettings> withdrawalList = FoundationDB.DB.Queryable<MasterSettings>().Where(r => r.Group == "MerchantWithdrawal").ToList();
            ViewBag.WithdrawalList = withdrawalList;
            return View();
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("MerchantSettingUpdate")]
        public ActionResult Save(List<string> olist)
        {
            MasterSettingBLL msb = new MasterSettingBLL();
            var result = msb.Update(olist, UserId, UserName, "MerchantWithdrawal");
            return Json(result.toJson());
        }
    }
}