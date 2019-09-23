using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("SalespersonMenu")]
    public class SalespersonController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(string salesCode,string salesName,string mobile, GridPager pager)
        {
            List<SalespersonViewModel> data = new SalespersonBLL().GetSalesPersonPager(salesCode, salesName, mobile, ref pager);
            var obj = data.ToGridJson(pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            Id = r.Id,
                            SaleCode = r.SaleCode,
                            SaleName = r.SaleName,
                            Gender = (r.Gender == 0?"Male":"Female"),
                            Position = r.Position,
                            Mobile = r.Mobile,
                            AgentCount = r.AgentCount,
                            CreateTime = r.CreateTime.ToString("yyyy-MM-dd")
                        }
                    });
            return Json(obj);
        }

        public PartialViewResult Create()
        {
            return PartialView("Edit", new Salesperson());
        }
        public PartialViewResult Edit(int id)
        {
            Salesperson sales = BoDB.SalespersonDb.GetById(id);
            return PartialView(sales);
        }
        [HttpPost, AjaxAntiForgeryToken]
        public JsonResult Save(Salesperson sales)
        {
            SaveResult sr;
            if (sales.Id > 0)
            {
                sales.ModifyBy = UserId;
                sr = SaveEdit(sales);
            }
            else
            {
                sales.CreateBy = UserId;
                sr = SaveCreate(sales);
            }
            return Json(sr.toJson());
        }
        [BOAccess("SalespersonCreate")]
        private SaveResult SaveCreate(Salesperson sales)
        {
            return new SalespersonBLL().SaveCreate(sales, UserId, UserName);
        }
        [BOAccess("SalespersonUpdate")]
        private SaveResult SaveEdit(Salesperson sales)
        {
            return new SalespersonBLL().SaveEdit(sales, UserId, UserName);
        }
        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("SalespersonDelete")]
        public JsonResult Delete(int id)
        {
            var sr = new SalespersonBLL().Delete(id, UserId, UserName);
            return Json(sr.toJson());
        }
    }
}