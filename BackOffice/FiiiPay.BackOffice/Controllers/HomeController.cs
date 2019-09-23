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
    [BOAccess]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            Dictionary<Module, List<Module>> moduleDic = new Dictionary<Module, List<Module>>();
            List<Module> modules = BoDB.ModuleDb.GetList();
            var permissionList = PerimissionList.Where(t => t.IsDefault).Select(t => t.ModuleId).ToList(); ;
            if (modules!=null)
            {
                var moduleParent = modules.Where(t => !t.ParentId.HasValue).OrderBy(t=>t.Sort).ToList();
                if (moduleParent != null && moduleParent.Count>0)
                {
                    foreach (var p in moduleParent)
                    {
                        var moduleChildren = modules.FindAll(t => t.ParentId == p.Id);
                        if(moduleChildren!=null && moduleChildren.Count > 0)
                        {
                            if (IsAdmin)
                                moduleDic.Add(p, moduleChildren);
                            else
                            {
                                var mlist = moduleChildren.FindAll(t => permissionList.Contains(t.Id));
                                if(mlist!=null&&mlist.Count>0)
                                    moduleDic.Add(p, mlist);
                            }
                        }
                    }
                }
            }
            ViewBag.RoleName = UserName;
            return View(moduleDic);
        }
        public ActionResult HomePage()
        {
            return View();
        }
    }
}