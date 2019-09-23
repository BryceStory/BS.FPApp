using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("PriceInfoMenu")]
    public class PriceInfoController : BaseController
    {
        // GET: PriceInfo
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public JsonResult LoadData(GridPager pager, string cryptoCode, string currencyCode)
        {
            List<PriceInfoViewModel> data = FoundationDB.DB.Queryable<Currency, PriceInfos, Cryptocurrencies>((currency, priceinfo, crypto) =>
            new object[]{
                JoinType.Left, priceinfo.CurrencyID == currency.ID,
                JoinType.Left, priceinfo.CryptoID == crypto.Id})
                .WhereIF(!string.IsNullOrEmpty(cryptoCode), (currency, priceinfo, crypto) => crypto.Code.Contains(cryptoCode))
                .WhereIF(!string.IsNullOrEmpty(currencyCode), (currency, priceinfo, crypto) => currency.Code.Contains(currencyCode))
                .Select<PriceInfoViewModel>((currency, priceinfo, crypto) => new PriceInfoViewModel
                {
                    ID = priceinfo.ID,
                    CryptoCode = crypto.Code,
                    CryptoName = crypto.Name,
                    CurrencyCode = currency.Code,
                    CurrencyName = currency.Name,
                    Price = priceinfo.Price,
                    Markup = priceinfo.Markup
                }).ToList();
            var obj = data.ToGridJson(ref pager, r =>
                new
                {
                    id = r.ID,
                    cell = new
                    {
                        Id = r.ID.ToString(),
                        CryptoCode = r.CryptoCode,
                        CryptoName = r.CryptoName,
                        CurrencyCode = r.CurrencyCode,
                        CurrencyName = r.CurrencyName,
                        Price = r.Price,
                        Markup = (r.Markup * 100) + "%",
                        MarkupPrice = (r.Price * r.Markup).ToString("0.00")
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            ViewBag.CurrencyList = GetCurrencySelectList();
            ViewBag.CryptoList = GetCryptoSelectList();
            PriceInfos priceinfo = new PriceInfos();
            if (Id > 0)
            {
                priceinfo = FoundationDB.PriceInfoDb.GetById(Id);
                priceinfo.Markup = priceinfo.Markup * 100;
            }
            return PartialView(priceinfo);
        }

        private List<SelectListItem> GetCurrencySelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var currencyList = FoundationDB.CurrencyDb.GetList();
            if (currencyList != null && currencyList.Count > 0)
            {
                oList.AddRange(currencyList.Select(t => new SelectListItem() { Text = t.Code, Value = t.ID.ToString() }));
            }
            return oList;
        }
        private List<SelectListItem> GetCryptoSelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            var cryptoList = FoundationDB.CryptocurrencyDb.GetList();
            if (cryptoList != null && cryptoList.Count > 0)
            {
                oList.AddRange(cryptoList.Select(t => new SelectListItem() { Text = t.Code, Value = t.Id.ToString() }));
            }
            return oList;
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(PriceInfos priceinfo)
        {
            if (priceinfo.ID > 0)//编辑
            {
                return Json(SaveEdit(priceinfo).toJson());
            }
            else//新增
            {
                return Json(SaveCreate(priceinfo).toJson());
            }
        }

        [BOAccess("PriceInfoCreate")]
        private SaveResult SaveCreate(PriceInfos priceinfo)
        {
            PriceInfoAgent ab = new PriceInfoAgent();

            return ab.Create(priceinfo, UserId, UserName);
        }

        [BOAccess("PriceInfoUpdate")]
        private SaveResult SaveEdit(PriceInfos priceinfo)
        {
            PriceInfoAgent ab = new PriceInfoAgent();
            return ab.Update(priceinfo, UserId, UserName);
        }

        [BOAccess("PriceInfoDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int Id)
        {
            PriceInfoAgent ab = new PriceInfoAgent();
            return Json(ab.Delete(Id, UserId, UserName).toJson());
        }
    }
}