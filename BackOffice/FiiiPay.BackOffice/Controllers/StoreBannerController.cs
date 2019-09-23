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
    [BOAccess("StoreBannerMenu")]
    public class StoreBannerController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public JsonResult LoadData(GridPager pager)
        {
            var list = FiiiPayDB.StoreBannerDb.GetList().OrderByDescending(t => t.Status).ThenBy(t => t.Sort).ThenByDescending(t => t.StartTime).ToList();
            var countryList = FoundationDB.CountryDb.GetList();
            var data = list.ToGridJson(ref pager, t => new
            {
                id = t.Id,
                cell = new {
                    t.Id,
                    t.Title,
                    t.LinkUrl,
                    CountryName = t.CountryId == 0 ? "全部" : countryList.Where(c => c.Id == t.CountryId).Select(c => c.Name_CN).FirstOrDefault(),
                    AliveTime = t.StartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm") + " - " + t.EndTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                    Status = t.Status == Entities.BannerStatus.Active ? "正常" : "关闭",
                    Timestamp = t.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                }
            }, false);
            return Json(data);
        }

        public ActionResult Add()
        {
            ViewBag.BannerStatusList = new List<SelectListItem>
            {
                new SelectListItem{ Text="正常",Value=((byte)Entities.BannerStatus.Active).ToString() },
                new SelectListItem{ Text="关闭",Value=((byte)Entities.BannerStatus.Closed).ToString() }
            };
            ViewBag.OpenByAppList = new List<SelectListItem>
            {
                new SelectListItem{ Text="APP打开",Value="true" },
                new SelectListItem{ Text="H5打开",Value="false" }
            };
            var countryList = FoundationDB.CountryDb.GetList().ToDictionary(k => k.Id, v => v.Name_CN);
            ViewBag.CountryList = ConvertDicToSelect(countryList, true, allOptionTxt: "所有", allOptionValue: "0");
            return View();
        }

        public ActionResult Edit(long id)
        {
            ViewBag.BannerStatusList = new List<SelectListItem>
            {
                new SelectListItem{ Text="正常",Value=((byte)Entities.BannerStatus.Active).ToString() },
                new SelectListItem{ Text="关闭",Value=((byte)Entities.BannerStatus.Closed).ToString() }
            };
            ViewBag.OpenByAppList = new List<SelectListItem>
            {
                new SelectListItem{ Text="APP打开",Value="true" },
                new SelectListItem{ Text="H5打开",Value="false" }
            };
            var countryList = FoundationDB.CountryDb.GetList().ToDictionary(k => k.Id, v => v.Name_CN);
            ViewBag.CountryList = ConvertDicToSelect(countryList, true, allOptionTxt: "所有", allOptionValue: "0");
            var banner = FiiiPayDB.StoreBannerDb.GetById(id);
            banner.StartTime = banner.StartTime.ToLocalTime();
            banner.EndTime = banner.EndTime.ToLocalTime();
            return View(banner);
        }

        public PartialViewResult UploadPicture()
        {
            return PartialView();
        }

        [BOAccess("StoreBannerCreate")]
        public JsonResult SaveAdd(StoreBanners banner)
        {
            banner.StartTime = banner.StartTime.ToUniversalTime();
            banner.EndTime = banner.EndTime.ToUniversalTime();
            var sr = new StoreBannerBLL().SaveAdd(banner);
            return Json(sr.toJson());
        }

        [BOAccess("StoreBannerUpdate")]
        public JsonResult SaveEdit(StoreBanners banner)
        {
            banner.StartTime = banner.StartTime.ToUniversalTime();
            banner.EndTime = banner.EndTime.ToUniversalTime();
            var sr = new StoreBannerBLL().SaveEdit(banner);
            return Json(sr.toJson());
        }

        [BOAccess("StoreBannerUpdate")]
        public JsonResult SetTop(long id)
        {
            var sr = new StoreBannerBLL().SetTop(id);
            return Json(sr.toJson());
        }

        [BOAccess("StoreBannerDelete")]
        public JsonResult Delete(long id)
        {
            var sr = new StoreBannerBLL().Delete(id);
            return Json(sr.toJson());
        }
    }
}