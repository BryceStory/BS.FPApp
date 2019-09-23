using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    /// <summary>
    /// 国家下属区域管理
    /// </summary>
    [BOAccess("RegionManageMenu")]
    public class RegionManageController : BaseController
    {
        private RegionMagangeBLL _bll = new RegionMagangeBLL();

        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = ConvertDicToSelect(GetCountryList(), true, allOptionTxt: "全部");
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(GridPager pager, int? countryId)
        {
            var list = _bll.GetRegionPager(countryId, ref pager);
            var countryList = FoundationDB.CountryDb.GetList();
            var obj = list.ToGridJson(pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            r.Id,
                            CountryName = countryList.FirstOrDefault(t => t.Id == r.CountryId)?.Name_CN,
                            r.Name,
                            r.NameCN,
                            r.Code,
                            r.Sort
                        }
                    });
            return Json(obj);
        }

        [HttpPost]
        public JsonResult LoadChildrenData(GridPager pager, long parentId, byte parentLevel)
        {
            var list = _bll.GetChildrenRegionPager(parentId,parentLevel, ref pager);
            var obj = list.ToGridJson(pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            r.Id,
                            r.Name,
                            r.NameCN,
                            r.Code,
                            r.Sort
                        }
                    });
            return Json(obj);
        }

        public ActionResult Add()
        {
            ViewBag.CountryList = ConvertDicToSelect(GetCountryList(), true, allOptionTxt: "请选择");
            return View();
        }

        [HttpPost]
        [BOAccess("RegionManageCreate")]
        public ActionResult Add(Regions r)
        {
            var sr = _bll.SaveAdd(r);
            ViewBag.CountryName = FoundationDB.CountryDb.GetById(sr.Data.CountryId).Name_CN;
            return View("Edit", sr.Data);
        }

        public ActionResult Edit(long id)
        {
            var entity = FoundationDB.DB.Queryable<Regions>().First(t => t.Id == id);
            string parentName = "";
            if (entity.RegionLevel == RegionLevel.State)
                parentName = FoundationDB.CountryDb.GetById(entity.CountryId).Name_CN;
            else
                parentName = FoundationDB.DB.Queryable<Regions>().First(t => t.Id == entity.ParentId).NameCN;
            ViewBag.ParentName = parentName;
            return View(entity);
        }

        [HttpPost]
        [BOAccess("RegionManageUpdate")]
        public ActionResult Edit(Regions r)
        {
            var sr = _bll.SaveEdit(r);
            string parentName = "";
            if(sr.Data.RegionLevel == RegionLevel.State)
                parentName = FoundationDB.CountryDb.GetById(sr.Data.CountryId).Name_CN;
            else
                parentName = FoundationDB.DB.Queryable<Regions>().First(t => t.Id == sr.Data.ParentId).NameCN;
            ViewBag.ParentName = parentName;
            return View(sr.Data);
        }

        public ActionResult Detail(long id)
        {
            return View();
        }

        public PartialViewResult AddChildRegion(long parentId)
        {
            var parent = FoundationDB.DB.Queryable<Regions>().First(t => t.Id == parentId);
            EditChildRegionModel model = new EditChildRegionModel
            {
                ParantId_Edit = parentId,
                ParentName = parent.NameCN ?? parent.Name
            };
            return PartialView(model);
        }

        [HttpPost]
        [BOAccess("RegionManageCreate")]
        public JsonResult SaveAddChildRegion(EditChildRegionModel model)
        {
            Regions r = new Regions
            {
                ParentId = model.ParantId_Edit,
                Code = model.Code_Edit,
                Name = model.Name_Edit,
                NameCN = model.NameCN_Edit,
                Sort = model.Sort_Edit
            };
            var sr = _bll.SaveChildAdd(r);
            return Json(sr.toJson());
        }
        
        private Dictionary<int, string> GetCountryList()
        {
            return FoundationDB.CountryDb.GetList().ToDictionary(u => u.Id, v => v.Name_CN);
        }
    }
}