using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    public class TelephoneServiceController : BaseController
    {
        [BOAccess("TelephoneServiceMenu")]
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        [HttpPost]
        public JsonResult LoadData(GridPager pager)
        {
            var obj = FoundationDB.CountryDb.GetList()
               .ToGridJson(ref pager, r =>
                   new
                   {
                       id = r.Id,
                       cell = new
                       {
                           Id = r.Id,
                           Name = r.Name,
                           PhoneCode = r.PhoneCode,
                           CustomerService = ""//r.CustomerService
                       }
                   });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Edit(int id)
        {
            var country = FoundationDB.CountryDb.GetById(id);
            List<Countries> list = FoundationDB.CountryDb.GetList();
            List<SelectListItem> oList = new List<SelectListItem>();
            foreach (var item in list)
            {
                oList.Add(new SelectListItem() { Text = item.PhoneCode + " " + item.Code, Value = item.Id.ToString() });
            }

            ViewBag.List = oList;
            return PartialView(country);
        }
    }
}