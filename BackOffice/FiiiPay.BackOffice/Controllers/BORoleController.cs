using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("BORoleMenu")]
    public class BORoleController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        [HttpPost]
        public JsonResult LoadData(string roleName, GridPager pager)
        {
            Expression<Func<AccountRole, bool>> filter = t => true;
            if (!string.IsNullOrEmpty(roleName))
                filter = filter.And(t => t.Name.Contains(roleName));
            var obj = BoDB.AccountRoleDb.GetList(filter)
                .ToGridJson(ref pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description
                        }
                    });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        
        public PartialViewResult Create()
        {
            return PartialView("Edit");
        }
        
        public PartialViewResult Edit(int id)
        {
            AccountRole role = BoDB.AccountRoleDb.GetById(id);
            return PartialView(role);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(AccountRole role)
        {
            if (role.Id == 0)
            {
                var sr = SaveCreate(role);
                return Json(sr.toJson());
            }
            var srEdit = SaveEdit(role);
            return Json(srEdit.toJson());
        }


        [BOAccess("BORoleCreate")]
        private SaveResult SaveCreate(AccountRole role)
        {
            role.CreateBy = UserId;
            return new BORoleBLL().SaveCreate(role, UserId, UserName);
        }


        [BOAccess("BORoleUpdate")]
        private SaveResult SaveEdit(AccountRole role)
        {
            role.ModifyBy = UserId;
            return new BORoleBLL().SaveEdit(role, UserId, UserName);
        }

        [BOAccess("BORoleDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var sr = new BORoleBLL().SaveDelete(id, UserId, UserName);
            return Json(sr.toJson());
        }

        public ActionResult RolePermission(int roleId)
        {
            Dictionary<Module, List<Module>> moduleDic = new Dictionary<Module, List<Module>>();
            Dictionary<Module, List<ModulePermission>> modulePerms = new Dictionary<Module, List<ModulePermission>>();
            List<RoleAuthority> authList = BoDB.RoleAuthorityDb.GetList(t => t.RoleId == roleId);
            AccountRole role = BoDB.AccountRoleDb.GetById(roleId);

            List<Module> allModuleList = BoDB.ModuleDb.GetList();
            List<ModulePermission> allModulePermList = BoDB.ModulePermissionDb.GetList();

            if (allModuleList != null)
            {
                var moduleParent = allModuleList.FindAll(t => !t.ParentId.HasValue);
                if (moduleParent != null && moduleParent.Count > 0)
                {
                    foreach (var p in moduleParent)
                    {
                        var moduleChildren = allModuleList.FindAll(t => t.ParentId == p.Id);
                        if (moduleChildren != null && moduleChildren.Count > 0)
                        {
                            foreach (var c in moduleChildren)
                            {
                                var mpList = allModulePermList.FindAll(t => t.ModuleId == c.Id);
                                if (mpList != null && mpList.Count > 0)
                                {
                                    modulePerms.Add(c, mpList);
                                }
                            }
                            moduleDic.Add(p, moduleChildren);
                        }
                    }
                }
            }

            ViewBag.ModuleDic = moduleDic;
            ViewBag.ModulePerms = modulePerms;
            ViewBag.AuthList = authList;
            return View(role);
        }

        [BOAccess("BORoleAuth")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult SavePermission(List<RoleAuthority> roleAuthList)
        {
            var result = new BORoleBLL().SavePermission(UserId, roleAuthList, UserId, UserName);
            return Json(result.toJson());
        }
    }
}