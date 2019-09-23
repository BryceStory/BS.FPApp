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
    [BOAccess("ModuleMenu")]
    public class ModuleController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        [HttpPost]
        public JsonResult LoadData(GridPager pager, string expandIds)
        {
            List<int> expandList = new List<int>();
            if (!string.IsNullOrEmpty(expandIds))
            {
                expandList = expandIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => int.Parse(t)).ToList();
            }
            var moduleList = BoDB.ModuleDb.GetList().ToList();
            List<Module> sortList = new List<Module>();
            var modulegroup = (from d in moduleList
                               group d by d.ParentId into m
                               select m).ToDictionary(t => t.Key.HasValue ? t.Key.Value : 0, t => t.ToList());

            var parentList = moduleList.Where(t => !t.ParentId.HasValue).OrderBy(t => t.Sort).ToList();
            foreach (var pitem in parentList)
            {
                sortList.Add(pitem);
                if (!modulegroup.ContainsKey(pitem.Id))
                    continue;
                if (modulegroup[pitem.Id] == null)
                    continue;
                sortList.AddRange(modulegroup[pitem.Id].OrderBy(t => t.Sort));
            }
            
            var obj = sortList.ToGridJson(ref pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            Id = r.Id,
                            Code = r.Code,
                            Icon = r.Icon,
                            PageName = r.Name,
                            PageUrl = r.PathAddress,
                            Sort = r.Sort,
                            parent = r.ParentId.HasValue ? r.ParentId.ToString() : "",
                            loaded = true,
                            expanded = expandList.Contains(r.Id),
                            isLeaf = r.ParentId.HasValue,
                            level = r.ParentId.HasValue ? 1 : 0
                        }
                    },false);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        
        public PartialViewResult Create()
        {
            var parentList = BoDB.ModuleDb.GetList(t => !SqlFunc.HasValue(t.ParentId));
            ViewBag.ParentList = GetParentModuleList(parentList);
            return PartialView();
        }

        public PartialViewResult Edit(int Id)
        {
            var module = BoDB.ModuleDb.GetById(Id);
            var parentList = BoDB.ModuleDb.GetList(t => !SqlFunc.HasValue(t.ParentId));
            ViewBag.ParentList = GetParentModuleList(parentList);
            return PartialView("Create", module);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public JsonResult Save(Module module)
        {
            SaveResult sr;
            if (module.Id > 0)
            {
                module.ModifyBy = UserId;
                sr = SaveEdit(module);
            }
            else
            {
                module.CreateBy = UserId;
                sr = SaveCreate(module);
            }
            return Json(sr.toJson());
        }

        [BOAccess("ModuleCreate")]
        public SaveResult SaveCreate(Module module)
        {
            var mbll = new ModuleBLL();
            module.CreateBy = UserId;
            var sr = mbll.SaveCreate(module, UserId, UserName);
            return sr;
        }
        
        [BOAccess("ModuleUpdate")]
        public SaveResult SaveEdit(Module module)
        {
            var mbll = new ModuleBLL();
            module.ModifyBy = UserId;
            var sr = mbll.SaveEdit(module, UserId, UserName);
            return sr;
        }

        [BOAccess("ModuleDelete")]
        [HttpPost,AjaxAntiForgeryToken]
        public ActionResult Delete(int Id)
        {
            var sr = new ModuleBLL().SaveDelete(Id, UserId, UserName);

            return Json(sr.toJson());
        }

        private List<SelectListItem> GetParentModuleList(List<Module> oList)
        {
            List<SelectListItem> parentModuleList = new List<SelectListItem>();
            parentModuleList.Add(new SelectListItem() { Text = "无", Value = "" });
            if (oList == null || oList.Count <= 0)
                return parentModuleList;
            foreach (var item in oList)
            {
                parentModuleList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            return parentModuleList;
        }

        [BOAccess("ModulePermission")]
        public ActionResult Perimssion(int moduleId)
        {
            GeneralPermission gp = new GeneralPermission()
            {
                Create = CheckPerimission("ModulePermissionCreate"),
                Update = CheckPerimission("ModulePermissionUpdate"),
                Delete = CheckPerimission("ModulePermissionDelete"),
            };
            var module = BoDB.ModuleDb.GetById(moduleId);
            List<ModulePermission> permList = BoDB.ModulePermissionDb.GetList(t => t.ModuleId == moduleId);
            ViewBag.ModuleInfo = module;
            ViewBag.GeneralPermission = gp;
            return View(permList);
        }

        public PartialViewResult PerimissCreate(int moduleId)
        {
            ModulePermission permission = new ModulePermission() { ModuleId = moduleId };
            return PartialView(permission);
        }
        
        public PartialViewResult PerimissEdit(int Id)
        {
            ModulePermission permission = BoDB.ModulePermissionDb.GetById(Id);
            return PartialView("PerimissCreate", permission);
        }

        [BOAccess("ModulePermissionCreate")]
        [HttpPost, AjaxAntiForgeryToken]
        public JsonResult SaveAddPermission(ModulePermission permission)
        {
            permission.CreateBy = UserId;
            var sr = new ModuleBLL().SaveAddPermission(permission, UserId, UserName);
            return Json(sr.toJson());
        }

        [BOAccess("ModulePermissionUpdate")]
        [HttpPost, AjaxAntiForgeryToken]
        public JsonResult SaveEditPermission(ModulePermission permission)
        {
            permission.ModifyBy = UserId;
            var sr = new ModuleBLL().SaveEditPermission(permission, UserId, UserName);
            return Json(sr.toJson());
        }

        [BOAccess("ModulePermissionDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public JsonResult SaveDeletePermission(int id)
        {
            var sr = new ModuleBLL().SaveDeletePermission(id, UserId, UserName);
            return Json(sr.toJson());
        }
    }
}