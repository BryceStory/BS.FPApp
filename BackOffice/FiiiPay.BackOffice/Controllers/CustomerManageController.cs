using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("CustomerManageMenu")]
    public class CustomerManageController : BaseController
    {
        // GET: MasterSettings
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            List<MasterSettings> list = FoundationDB.DB.Queryable<MasterSettings>().Where(r => r.Group == "CustomerService").ToList();
            return View(list??new List<MasterSettings>());
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("CustomerManageUpdate")]
        public ActionResult Save(string[] Wechat,string Facebook)
        {
            var result = new CustomerServiceBLL().Update(Wechat, Facebook, UserId, UserName);
            return Json(result.toJson());
        }
    }
}