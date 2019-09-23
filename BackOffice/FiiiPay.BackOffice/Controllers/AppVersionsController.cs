using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.Framework.Cache;

namespace FiiiPay.BackOffice.Controllers
{
    public class AppVersionsController : BaseController
    {
        // GET: AppVersions
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public ActionResult LoadData(GridPager pager, string type, bool? hasProcessor)
        {
            var data = FoundationDB.AppVersionDb.GetList();
            var obj = data.ToGridJson(pager, r =>
                new
                {
                    id = r.Id,
                    cell = new string[]{
                        r.Id.ToString(),
                        r.Platform.ToString(),
                        r.Version,
                        r.ForceToUpdate.ToString(),
                        r.Description,
                        r.Url,
                        r.App.ToString()
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var isList = new List<SelectListItem>();
            isList.Add(new SelectListItem() { Text = "True", Value = "true" });
            isList.Add(new SelectListItem() { Text = "False", Value = "false" });
            ViewBag.IsList = isList;

            var fb = FoundationDB.AppVersionDb.GetById(id);
            return PartialView(fb);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("AppVersionUpdate")]
        public ActionResult Save(AppVersions model)
        {
            AppVersionBLL fbb = new AppVersionBLL();
            return Json(fbb.Update(model, UserId, UserName).toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("AppVersionUpdate")]
        public ActionResult EditAPIStatus(string type)
        {
            switch (type)
            {
                case "OpenFiiiPay":
                    RedisHelper.StringSet("FiiiPay:API:IsMaintain", "False");
                    break;
                case "CloseFiiiPay":
                    RedisHelper.StringSet("FiiiPay:API:IsMaintain", "True");
                    break;
                case "OpenFiiiPos":
                    RedisHelper.StringSet("FiiiPos:API:IsMaintain", "False");
                    break;
                case "CloseFiiiPos":
                    RedisHelper.StringSet("FiiiPos:API:IsMaintain", "True");
                    break;
                default:
                    break;
            }
            return Json(new SaveResult(true).toJson());
        }
    }
}