using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using FiiiPay.Framework.Component;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("POSAdvanceOrderMenu")]
    public class AdvanceOrderController : BaseController
    {
        AdvanceOrderBLL advanceorder = new AdvanceOrderBLL();
        // GET: AdvanceOrder
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(string OrderName, string Paymentstatus, string Shippingstatus, GridPager pager)
        {
            List<AdvanceOrders> data = advanceorder.GetCodeQuerAll( OrderName, Paymentstatus, Shippingstatus, ref pager);
            var obj = data.ToGridJson(pager, r =>
                  new
                  {
                      id = r.Id,
                      cell = new
                      {
                          Id = r.Id,
                          OrderName=r.OrderName,
                          Phone=r.Phone,
                          Amount=r.Amount,
                          Price=r.Price,
                          Totalpayment=r.Totalpayment,
                          Advance=r.Advance,
                          TransferName=r.TransferName,
                          TransferNumber=r.TransferNumber,
                          Paymentstatus=r.Paymentstatus,
                          Shippingstatus=r.Shippingstatus,
                          Time= r.Time.ToString("yyyy-MM-dd"),
                          Salesperson=r.Salesperson,
                          Remark=r.Remark,
                          Amountpaid=r.Amountpaid
                      }
                  });
            return Json(obj);
        }
        [AllowAnonymous]
        public JsonResult SaveCreate()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();
            try
            {
                // assuming JSON.net/Newtonsoft library from http://json.codeplex.com/
                var info = JsonConvert.DeserializeObject<AdvanceOrders>(json);
                info.Time= DateTime.Now;
                info.Shippingstatus = "未发货"; //发货状态
                if (info.Advance!=null) //付款状态
                    info.Paymentstatus = "已付预付款"; 
                else
                    info.Paymentstatus = "未付款";
                AccountBLL ab = new AccountBLL();
                advanceorder.Create(info);
                return Json(true);
            }

            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public ActionResult Edit(int id)
        {
            var fb = BoDB.AdvanceOrderDb.GetById(id);
            return PartialView(fb);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSAdvanceOrderUpdate")]
        public ActionResult Save(AdvanceOrders advanceOrders)
        {
            var fb = BoDB.AdvanceOrderDb.GetById(advanceOrders.Id);
            return Json(advanceorder.UpdatePassword(advanceOrders, UserId, UserName).toJson());
        }
    }
}