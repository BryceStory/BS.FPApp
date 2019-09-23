using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("StorePaySettingMenu")]
    public class StorePaySettingController : BaseController
    {
        // GET: StorePaySetting
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public JsonResult LoadData(GridPager pager)
        {
            var list = FiiiPayDB.StorePaySettingDb.GetList();
            var countryList = FoundationDB.CountryDb.GetList();
            var data = list.ToGridJson(ref pager, t =>
            {
                var country = countryList.Find(c => c.Id == t.CountryId);
                return new {
                    id=t.Id,
                    cell = new
                    {
                        Id = t.Id,
                        NameEN = country.Name,
                        NameCN = country.Name_CN,
                        FiatCurrency = t.FiatCurrency,
                        LimitAmount = t.LimitAmount
                    }
                };
            }, true);
            return Json(data);
        }

        public PartialViewResult Add()
        {
            ViewBag.CountryList = FoundationDB.CountryDb.GetList(t => t.IsSupportStore);
            var setting = new StorePaySettings { Id = 0 };
            return PartialView("Edit", setting);
        }

        public PartialViewResult Edit(long id)
        {
            ViewBag.CountryList = FoundationDB.CountryDb.GetList(t => t.IsSupportStore);
            var setting = FiiiPayDB.StorePaySettingDb.GetById(id);
            return PartialView(setting);
        }
        
        public JsonResult Save(StorePaySettings paySetting)
        {
            if (paySetting.Id <= 0)
                return Json(SaveAdd(paySetting).toJson());
            return Json(SaveEdit(paySetting).toJson());
        }
        
        [BOAccess("StorePaySettingCreate")]
        private SaveResult SaveAdd(StorePaySettings paySetting)
        {
            var existSetting = FiiiPayDB.StorePaySettingDb.IsAny(t => t.CountryId == paySetting.CountryId);
            if (existSetting)
                return new SaveResult(false, "已经设置了这个国家的数据");
            var sr = FiiiPayDB.StorePaySettingDb.Insert(paySetting);
            return new SaveResult(sr);
        }

        [BOAccess("StorePaySettingUpdate")]
        private SaveResult SaveEdit(StorePaySettings paySetting)
        {
            var sr = FiiiPayDB.StorePaySettingDb.Update(paySetting);
            return new SaveResult(sr);
        }

        [BOAccess("StorePaySettingDelete")]
        public JsonResult Delete(long id)
        {
            var sr = FiiiPayDB.StorePaySettingDb.DeleteById(id);
            return Json(new SaveResult(sr).toJson());
        }
    }
}