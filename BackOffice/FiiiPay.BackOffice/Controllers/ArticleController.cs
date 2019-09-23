using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("ArticlesMenu")]
    public class ArticleController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public ActionResult LoadData(GridPager pager, string title)
        {
            //var bll = new DbDemoBLL();
            //var sr = bll.Demo();
            var data = FiiiPayDB.DB.Queryable<Articles>().WhereIF(!string.IsNullOrEmpty(title), t => t.Title.Contains(title)).ToList();
            var obj = data.ToGridJson(ref pager, t =>
            new
            {
                id = t.Id,
                cell = new
                {
                    Id = t.Id,
                    Title = t.Title,
                    Descdescription = t.Descdescription,
                    Type = t.AccountType.ToString(),
                    ShouldPop = t.ShouldPop,
                    HasPushed = t.HasPushed.HasValue ? t.HasPushed.Value : false,
                    CreateTime = t.CreateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                }
            });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            Articles article = new Articles();
            article.Id = -1;
            if (Id > 0)
            {
                article = FiiiPayDB.ArticlesDb.GetById(Id);
            }
            List<SelectListItem> typeList = new List<SelectListItem>();
            typeList.Add(new SelectListItem() { Text = "FiiiPos", Value = "0" });
            typeList.Add(new SelectListItem() { Text = "FiiiPay", Value = "1" });
            ViewBag.TypeList = typeList;
            return View(article);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("ArticleUpdate")]
        public ActionResult Save(Articles oarticle)
        {
            ArticleBLL ab = new ArticleBLL();
            SaveResult result = new SaveResult();
            if (oarticle.Id > 0)//编辑
            {
                result = ab.Update(oarticle, UserId, UserName);
            }
            else//新增
            {
                result = ab.Create(oarticle, UserId, UserName);
            }
            return Json(result.toJson());
        }

        [BOAccess("ArticleDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ArticleBLL fbb = new ArticleBLL();
            return Json(fbb.DeleteById(id, UserId, UserName).toJson());
        }


        [BOAccess("ArticleDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult BatchDelete(string ids)
        {
            ArticleBLL fbb = new ArticleBLL();
            return Json(fbb.BatchDelete(ids, UserId, UserName).toJson());
        }
    }
}