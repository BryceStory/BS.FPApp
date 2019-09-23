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
    [BOAccess("StoreTypeMenu")]
    public class StoreTypeController : BaseController
    {
        // GET: StoreType
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public JsonResult LoadData(GridPager pager, string name_cn, string name_en)
        {
            var list = FiiiPayDB.DB.Queryable<StoreTypes>().
                 WhereIF(!string.IsNullOrWhiteSpace(name_cn), t => t.Name_CN.Contains(name_cn)).
                 WhereIF(!string.IsNullOrWhiteSpace(name_en), t => t.Name_EN.Contains(name_en)).ToList();
            var obj = list.ToGridJson(ref pager, r =>
                new
                {
                    id = r.Id,
                    cell = new string[]{
                        r.Id.ToString(),
                        r.Name_CN,
                        r.Name_EN
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var stb = new StoreTypes();
            if (id > 0)
            {
                stb = FiiiPayDB.StoreTypeDb.GetById(id);
            }
            return PartialView(stb);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(StoreTypes storeType)
        {
            if (storeType.Id > 0)//编辑
            {
                return Json(SaveEdit(storeType).toJson());
            }
            else//新增
            {
                return Json(SaveCreate(storeType).toJson());
            }
        }

        [BOAccess("StoreTypeCreate")]
        private SaveResult SaveCreate(StoreTypes storeType)
        {
            StoreTypeBLL stb = new StoreTypeBLL();
            return stb.Create(storeType, UserId, UserName);
        }

        [BOAccess("StoreTypeUpdate")]
        private SaveResult SaveEdit(StoreTypes storeType)
        {
            StoreTypeBLL stb = new StoreTypeBLL();
            return stb.Update(storeType, UserId, UserName);
        }

        [BOAccess("StoreTypeDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            StoreTypeBLL stb = new StoreTypeBLL();
            return Json(stb.DeleteById(id, UserId, UserName).toJson());
        }
    }
}