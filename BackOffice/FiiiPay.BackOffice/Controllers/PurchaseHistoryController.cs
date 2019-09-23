using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("PurchaseHistoryMenu")]
    public class PurchaseHistoryController : BaseController
    {
        // GET: PurchaseHistory
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public ActionResult LoadData(GridPager pager, string orderNo, string possn)
        {
            OrderBLL fb = new OrderBLL();
            var data = fb.GetOrderViewList(orderNo, possn, ref pager);
            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            var obj = data.ToGridJson(pager,
                r => new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id,
                        OrderNo = r.OrderNo,
                        MerchantName = r.MerchantName,
                        PostSN = r.PostSN,
                        Cellphone = r.Cellphone,
                        FiatAmount = r.FiatAmount,
                        Markup = (r.Markup * 100).ToString() + "%",
                        ActualFiatAmount = r.ActualFiatAmount,
                        CryptoName = coinList.Where(t => t.Id == r.CryptoId).Select(t => t.Name).FirstOrDefault(),
                        ExchangeRate = r.ExchangeRate,
                        CryptoAmount = r.CryptoAmount,
                        TransactionFee = r.TransactionFee.ToString(),
                        ActualCryptoAmount = r.ActualCryptoAmount,
                        Status = r.Status.ToString(),
                        Timestamp = r.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                    }
                });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("CanRefund")]
        public JsonResult Refund(string orderNo)
        {
            OrderBLL ob = new OrderBLL();
            return Json(ob.Refund(orderNo).toJson());
        }
    }
}