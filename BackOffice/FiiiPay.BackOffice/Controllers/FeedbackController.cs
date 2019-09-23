using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("FeedbackMenu")]
    public class FeedbackController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public ActionResult LoadData(GridPager pager, string type, bool? hasProcessor)
        {
            FeedbacksBLL fb = new FeedbacksBLL();
            var cotunryList = FoundationDB.CountryDb.GetList();
            var data = fb.GetFeedbacksList(type, hasProcessor, ref pager);
            var obj = data.ToGridJson(pager,
                r => new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id,
                        Date = r.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        Type = r.Type,
                        AccountId = r.AccountName,
                        Country = cotunryList.Where(t => t.Id == r.CountryId).Select(t => t.Name).FirstOrDefault(),
                        Context = r.Context,
                        HasProcessor = r.HasProcessor
                    }
                });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var fb = FiiiPayDB.FeedbacksDb.GetById(id);
            return PartialView(fb);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("FeedbackUpdate")]
        public ActionResult Save(int id, bool selectVal)
        {
            FeedbacksBLL fbb = new FeedbacksBLL();
            var fb = FiiiPayDB.FeedbacksDb.GetById(id);
            fb.HasProcessor = selectVal;
            return Json(fbb.Update(fb, UserId, UserName).toJson());
        }

        [BOAccess("FeedbackDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            FeedbacksBLL fbb = new FeedbacksBLL();
            return Json(fbb.DeleteById(id, UserId, UserName).toJson());
        }

        [BOAccess("FeedbackDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult BatchDelete(string ids)
        {
            FeedbacksBLL fbb = new FeedbacksBLL();
            return Json(fbb.BatchDelete(ids, UserId, UserName).toJson());
        }
    }
}