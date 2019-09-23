using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("AdvertisingMenu")]
    public class AdvertisingController : BaseController
    {
        // GET: Advertising
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public JsonResult LoadData(GridPager pager)
        {
            List<Advertisings> AccountList = FiiiPayDB.DB.Queryable<Advertisings>().ToList();
            var obj = AccountList.ToGridJson(ref pager, r =>
                new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id.ToString(),
                        Title = r.Title,
                        Link = r.Link,
                        LinkType = r.LinkType.ToString(),
                        StartDate = r.StartDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        EndDate = r.EndDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        Status = r.Status ? "Enable" : "Disable",
                        CreateTime = r.CreateTime.ToString()
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            Advertisings adv = new Advertisings();
            adv.Id = -1;
            adv.Status = true;
            if (Id > 0)
            {
                adv = FiiiPayDB.AdvertisingDb.GetById(Id);
            }
            adv.StartDate = adv.StartDate.ToLocalTime();
            adv.EndDate = adv.EndDate.ToLocalTime();
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "True" });
            statusList.Add(new SelectListItem() { Text = "Disable", Value = "False" });
            ViewBag.StatusList = statusList;

            var linkTypeList = new List<SelectListItem>();
            linkTypeList.AddRange(EnumHelper.EnumToList<LinkType>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            ViewBag.LinkTypeList = linkTypeList;
            return View(adv);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(Advertisings adv)
        {
            adv.StartDate = adv.StartDate.ToUniversalTime();
            adv.EndDate = adv.EndDate.ToUniversalTime();
            SaveResult result = new SaveResult();
            var linkTypeList = new List<SelectListItem>();
            linkTypeList.AddRange(EnumHelper.EnumToList<LinkType>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            ViewBag.LinkTypeList = linkTypeList;

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "True" });
            statusList.Add(new SelectListItem() { Text = "Disable", Value = "False" });
            ViewBag.StatusList = statusList;

            if (string.IsNullOrWhiteSpace(adv.Title) || string.IsNullOrWhiteSpace(adv.Link))
            {
                return View("Edit", adv);
            }
            //上传图片
            HttpPostedFileBase EnFile = Request.Files["PictureEn"];
            if (EnFile.ContentLength != 0)
            {
                adv.PictureEn = new Guid(new Utils.FileUploader().UpImageToCDN(EnFile));
            }
            HttpPostedFileBase ZhFile = Request.Files["PictureZh"];
            if (ZhFile.ContentLength != 0)
            {
                adv.PictureZh = new Guid(new Utils.FileUploader().UpImageToCDN(ZhFile));
            }
            if (adv.Id > 0)//编辑
            {
                SaveEdit(adv);
                Advertisings oldAdv = FiiiPayDB.AdvertisingDb.GetById(adv.Id);

                return View("Edit", oldAdv);
            }
            else//新增
            {
                if (ZhFile.ContentLength == 0)
                {
                    return View("Edit", adv);
                }
                if (EnFile.ContentLength == 0)
                {
                    return View("Edit", adv);
                }
                int newId = SaveCreate(adv).Data;
                Advertisings newAdv = FiiiPayDB.AdvertisingDb.GetById(newId);
                return View("Edit", newAdv);
            }

        }

        [BOAccess("AdvertisingCreate")]
        private SaveResult<int> SaveCreate(Advertisings adv)
        {
            AdvertisingsBLL ab = new AdvertisingsBLL();
            return ab.Create(adv, UserId, UserName);
        }

        [BOAccess("AdvertisingUpdate")]
        private SaveResult SaveEdit(Advertisings adv)
        {
            AdvertisingsBLL ab = new AdvertisingsBLL();
            return ab.Update(adv, UserId, UserName);
        }

        [BOAccess("AdvertisingDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            AdvertisingsBLL ab = new AdvertisingsBLL();
            return Json(ab.DeleteById(id, UserId, UserName).toJson());
        }
    }
}