using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("BillerOrderManageMenu")]
    public class BillerOrderController : BaseController
    {
        // GET: BillerOrder
        public ActionResult Index(int id)
        {
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "All" });
            statusList.AddRange(EnumHelper.EnumToList<BillerOrderStatus>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            ViewBag.StatusList = statusList;
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        [HttpPost]
        public JsonResult LoadData(string orderNo, string accountNo, string billerCode, int? status, DateTime? startDate, DateTime? endDate, GridPager pager)
        {
            List<BillerOrderViewModel> orderList = FiiiPayDB.DB.Queryable<BillerOrders, UserAccounts>((orders, account) => new object[] {
                JoinType.Left,orders.AccountId == account.Id})
                .WhereIF(!string.IsNullOrEmpty(orderNo), (orders, account) => orders.OrderNo.Contains(orderNo))
                .WhereIF(!string.IsNullOrEmpty(accountNo), (orders, account) => account.Cellphone.Contains(accountNo))
                .WhereIF(!string.IsNullOrEmpty(billerCode), (orders, account) => orders.BillerCode.Contains(billerCode))
                .WhereIF(status.HasValue, (orders, account) => (int)orders.Status == status.Value)
                .WhereIF(startDate.HasValue, (orders, account) => orders.Timestamp > startDate.Value.AddDays(-1))
                .WhereIF(endDate.HasValue, (orders, account) => orders.Timestamp < endDate.Value.AddDays(1))
                .Select<BillerOrderViewModel>((orders, account) => new BillerOrderViewModel
                {
                    Id = orders.Id,
                    AccountNo = account.Cellphone,
                    OrderNo = orders.OrderNo,
                    CountryId = orders.CountryId,
                    BillerCode = orders.BillerCode,
                    ReferenceNumber = orders.ReferenceNumber,
                    FiatAmount = orders.FiatAmount,
                    FiatCurrency = orders.FiatCurrency,
                    CryptoId = orders.CryptoId,
                    CryptoAmount = orders.CryptoAmount,
                    ExchangeRate = orders.ExchangeRate,
                    Status = orders.Status,
                    Timestamp = orders.Timestamp,
                    Tag = orders.Tag
                })
                .ToList();
            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            var cotunryList = FoundationDB.CountryDb.GetList();
            var data = orderList.ToGridJson(ref pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            OrderNo = r.OrderNo,
                            AccountNo = r.AccountNo,
                            CountryName = cotunryList.Where(t => t.Id == r.CountryId).Select(t => t.Name).FirstOrDefault(),
                            BillerCode = r.BillerCode,
                            ReferenceNumber = r.ReferenceNumber,
                            Amount = r.FiatAmount.ToString() + r.FiatCurrency,
                            CryptoName = coinList.Where(t => t.Id == r.CryptoId).Select(t => t.Name).FirstOrDefault(),
                            ExchangeRate = r.ExchangeRate,
                            CryptoAmount = r.CryptoAmount,
                            Status = r.Status.ToString(),
                            Timestamp = r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    });
            return Json(data);
        }

        public ActionResult Edit(Guid Id)
        {
            var order = FiiiPayDB.BillerOrderDb.GetById(Id);
            var statusList = new List<SelectListItem>();
            if (order.Status == BillerOrderStatus.Pending)
            {
                statusList.AddRange(EnumHelper.EnumToList<BillerOrderStatus>().Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            }
            else
            {
                statusList.AddRange(EnumHelper.EnumToList<BillerOrderStatus>().Where(t => t.EnumValue.Equals((int)order.Status)).Select(t => new SelectListItem() { Text = t.EnumName, Value = t.EnumValue.ToString() }));
            }
            ViewBag.StatusList = statusList;
            return PartialView(order);
        }

        [BOAccess("BillerOrderUpdate")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(BillerOrders orders)
        {
            BillerOrderBLL ab = new BillerOrderBLL();
            return Json(ab.Update(orders, UserId, UserName).toJson());
        }
    }
}