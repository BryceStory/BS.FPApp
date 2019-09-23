using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("POSAccountManageMenu")]
    public class POSAccountManageController : BaseController
    {
        // GET: POSAccountManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public ActionResult LoadData(GridPager pager, string userName, string cellPhone, string sn)
        {
            var cotunryList = FoundationDB.CountryDb.GetList();
            POSBLL pb = new POSBLL();
            var data = pb.GetPOSInfoList(userName, cellPhone, sn, ref pager);
            var obj = data.ToGridJson(pager,
                r => new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id,
                        Username = r.Username,
                        Sn = r.Sn,
                        Cellphone = r.Cellphone,
                        MerchantName = r.MerchantName,
                        CountryName = cotunryList.Where(t => t.Id == r.CountryId).Select(t => t.Name).FirstOrDefault()
                    }
                });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSUnBind")]
        public ActionResult Unbind(Guid id)
        {
            MerchantBLL mb = new MerchantBLL();
            return Json(mb.Unbind(id, UserId, UserName).toJson());
        }
        public ActionResult Bind(Guid id)
        {
            var model = FiiiPayDB.MerchantAccountDb.GetById(id);
            return PartialView(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSBind")]
        public ActionResult SaveSN(Guid merchantId, string newSN)
        {
            MerchantBLL mb = new MerchantBLL();
            return Json(mb.Rebind(merchantId, newSN, UserId, UserName).toJson());
        }

        [BOAccess("POSAccountManageChangeCellphone")]
        public PartialViewResult ChangeCellphone(Guid id)
        {
            var model = FiiiPayDB.MerchantAccountDb.GetById(id);
            if (model.POSId.HasValue)
            {
                var pos = FiiiPayDB.POSDb.GetById(model.POSId);
                ViewBag.SNNo = pos.Sn;
            }
            var countryList = FoundationDB.CountryDb.GetList();
            ViewBag.CountryList = countryList.Select(t => new SelectListItem { Text = t.Name_CN ?? t.Name, Value = t.Id.ToString() }).ToList();
            return PartialView(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSAccountManageChangeCellphone")]
        public ActionResult SaveNewCellphone(Guid MerchantId,int NewCountryId,string NewCellphone)
        {
            var sr = new MerchantBLL().ChangeCellphone(this.UserId, this.UserName, MerchantId, NewCountryId, NewCellphone);
            return Json(sr.toJson());
        }
    }
}