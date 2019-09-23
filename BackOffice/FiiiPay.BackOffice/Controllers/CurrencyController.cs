using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Foundation.Business;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("CurrencyMenu")]
    public class CurrencyController : BaseController
    {
        // GET: Currency
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        public JsonResult LoadData(GridPager pager, string code)
        {
            List<Currency> AccountList = FoundationDB.DB.Queryable<Currency>().WhereIF(!string.IsNullOrEmpty(code), t => t.Code.Contains(code)).ToList();
            var obj = AccountList.ToGridJson(ref pager, r =>
                new
                {
                    id = r.ID,
                    cell = new
                    {
                        Id = r.ID.ToString(),
                        Name_CN = r.Name_CN,
                        Name = r.Name,
                        Code = r.Code
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            Currency cur = new Currency();
            cur.ID = -1;
            List<PriceInfoViewModel> priceList = new List<PriceInfoViewModel>();
            if (Id > 0)
            {
                cur = FoundationDB.CurrencyDb.GetById(Id);
                priceList = FoundationDB.DB.Queryable<Cryptocurrencies, PriceInfos>((c, pi) => new object[] { JoinType.Left, c.Id == pi.CryptoID }).Where((c, pi) => pi.CurrencyID == Id).Select((c, pi) =>
              new PriceInfoViewModel { CryptoName = c.Name, CryptoCode = c.Code, Price = pi.Price }).ToList();
            }
            else
            {
                foreach (Cryptocurrencies item in FoundationDB.CryptocurrencyDb.GetList())
                {
                    PriceInfoViewModel pv = new PriceInfoViewModel();
                    pv.CryptoName = item.Name;
                    pv.CryptoCode = item.Code;
                    priceList.Add(pv);
                }
            }
            ViewBag.PriceList = priceList;
            return View(cur);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(Currency cur)
        {
            SaveResult result = new SaveResult();
            if (cur.ID > 0)//编辑
            {
                SaveEdit(cur);
                Currency oldCur = FoundationDB.CurrencyDb.GetById(cur.ID);
                List<PriceInfoViewModel> priceList = new List<PriceInfoViewModel>();
                priceList = FoundationDB.DB.Queryable<Cryptocurrencies, PriceInfos>((c, pi) => new object[] { JoinType.Left, c.Id == pi.CryptoID }).Where((c, pi) => pi.CurrencyID == cur.ID).Select((c, pi) =>
             new PriceInfoViewModel { CryptoName = c.Name, CryptoCode = c.Code, Price = pi.Price }).ToList();
                ViewBag.PriceList = priceList;

                return View("Edit", oldCur);

            }
            else//新增
            {
                int newId = SaveCreate(cur).Data;
                Currency newCur = FoundationDB.CurrencyDb.GetById(newId);

                List<PriceInfoViewModel> priceList = new List<PriceInfoViewModel>();
                priceList = FoundationDB.DB.Queryable<Cryptocurrencies, PriceInfos>((c, pi) => new object[] { JoinType.Left, c.Id == pi.CryptoID }).Where((c, pi) => pi.CurrencyID == newId).Select((c, pi) =>
             new PriceInfoViewModel { CryptoName = c.Name, CryptoCode = c.Code, Price = pi.Price }).ToList();
                ViewBag.PriceList = priceList;

                return View("Edit", newCur);
            }

        }

        [BOAccess("CurrencyCreate")]
        private SaveResult<int> SaveCreate(Currency cur)
        {
            List<PriceInfos> list = new List<PriceInfos>();
            if (cur.IsFixedPrice)
            {
                foreach (Currency item in FoundationDB.CurrencyDb.GetList())
                {
                    PriceInfos pi = new PriceInfos();
                    pi.CryptoID = cur.ID;
                    pi.CurrencyID = item.ID;
                    pi.LastUpdateDate = DateTime.Now;
                    pi.Price = Convert.ToDecimal(Request.Form[item.Code]);
                    list.Add(pi);
                }
            }
            CurrencyBLL cb = new CurrencyBLL();
            return cb.Create(cur, list, UserId, UserName);
        }

        [BOAccess("CurrencyUpdate")]
        private SaveResult SaveEdit(Currency cur)
        {
            List<PriceInfos> list = new List<PriceInfos>();
            if (cur.IsFixedPrice)
            {
                foreach (Cryptocurrencies item in FoundationDB.CryptocurrencyDb.GetList())
                {
                    PriceInfos pi = new PriceInfos();
                    pi.CryptoID = item.Id;
                    pi.CurrencyID = cur.ID;
                    pi.LastUpdateDate = DateTime.Now;
                    pi.Price = Convert.ToDecimal(Request.Form[item.Code]);
                    list.Add(pi);
                }
            }

            CurrencyBLL cb = new CurrencyBLL();
            return cb.Update(cur, list, UserId, UserName);
        }
    }
}