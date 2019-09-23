using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Foundation.Business;
using FiiiPay.Foundation.Entities.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("CryptoCurMenu")]
    public class CryptoCurController : BaseController
    {
        // GET: CurManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }

        public JsonResult LoadData(GridPager pager, string code)
        {
            List<Cryptocurrencies> AccountList = FoundationDB.DB.Queryable<Cryptocurrencies>().WhereIF(!string.IsNullOrEmpty(code), t => t.Code.Contains(code)).ToList();
            var obj = AccountList.ToGridJson(ref pager, r =>
                new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id.ToString(),
                        Name = r.Name,
                        Code = r.Code,
                        IconURL = r.IconURL,
                        Withdrawal_Tier = (r.Withdrawal_Tier * 100).ToString() + "%" + (r.Withdrawal_Fee.HasValue ? " + " : "") + r.Withdrawal_Fee
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            Cryptocurrencies cur = new Cryptocurrencies();
            cur.Id = -1;
            List<PriceInfoViewModel> priceList = new List<PriceInfoViewModel>();
            if (Id > 0)
            {
                cur = FoundationDB.CryptocurrencyDb.GetById(Id);
                cur.Withdrawal_Tier = cur.Withdrawal_Tier * 100;
                priceList = FoundationDB.DB.Queryable<Currency, PriceInfos>((c, pi) => new object[] { JoinType.Left, c.ID == pi.CurrencyID }).Where((c, pi) => pi.CryptoID == Id).Select((c, pi) =>
              new PriceInfoViewModel { CurrencyName = c.Name, CurrencyCode = c.Code, Price = pi.Price }).ToList();
                if (priceList.Count == 0)
                {
                    foreach (Currency item in FoundationDB.CurrencyDb.GetList())
                    {
                        PriceInfoViewModel pv = new PriceInfoViewModel();
                        pv.CurrencyName = item.Name;
                        pv.CurrencyCode = item.Code;
                        priceList.Add(pv);
                    }
                }
            }
            else
            {
                foreach (Currency item in FoundationDB.CurrencyDb.GetList())
                {
                    PriceInfoViewModel pv = new PriceInfoViewModel();
                    pv.CurrencyName = item.Name;
                    pv.CurrencyCode = item.Code;
                    priceList.Add(pv);
                }
            }
            Dictionary<string, bool> roleDic = new Dictionary<string, bool>();
            foreach (var item in EnumHelper.EnumToList<CryptoStatus>())
            {
                roleDic.Add(item.EnumName, CheckRoleOpened((byte)cur.Status, (CryptoStatus)item.EnumValue));
            }
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
            statusList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
            ViewBag.StatusList = statusList;
            ViewBag.RoleDic = roleDic;
            ViewBag.PriceList = priceList;
            var boolList = new List<SelectListItem>();
            boolList.Add(new SelectListItem() { Text = "True", Value = "1" });
            boolList.Add(new SelectListItem() { Text = "False", Value = "0" });
            ViewBag.BoolList = boolList;
            return View(cur);
        }

        public static bool CheckRoleOpened(byte? value, CryptoStatus flag)
        {
            if (!value.HasValue || value.Value == 0)
                return false;
            CryptoStatus validate = (CryptoStatus)Enum.Parse(typeof(CryptoStatus), value.Value.ToString());
            return (validate & flag) != 0;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(Cryptocurrencies cur)
        {
            var roleStr = Request.Form["moduleperm"];
            CryptoStatus role = new CryptoStatus();
            if (!string.IsNullOrWhiteSpace(roleStr))
            {
                roleStr.Split(',').ToList().ForEach(a =>
                {
                    role = role | (CryptoStatus)Enum.Parse(typeof(CryptoStatus), a);

                });
            }
            cur.Status = role;
            //上传图片
            HttpPostedFileBase IconFile = Request.Files["Icon"];
            if (IconFile.ContentLength != 0)
            {
                cur.IconURL = new Guid(new Utils.FileUploader().UpImageToCDN(IconFile));
            }
            SaveResult result = new SaveResult();

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
            statusList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
            ViewBag.StatusList = statusList;
            var boolList = new List<SelectListItem>();
            boolList.Add(new SelectListItem() { Text = "True", Value = "1" });
            boolList.Add(new SelectListItem() { Text = "False", Value = "0" });
            ViewBag.BoolList = boolList;
            if (cur.Id > 0)//编辑
            {
                SaveEdit(cur);
                Cryptocurrencies oldCur = FoundationDB.CryptocurrencyDb.GetById(cur.Id);
                Dictionary<string, bool> roleDic = new Dictionary<string, bool>();
                foreach (var item in EnumHelper.EnumToList<CryptoStatus>())
                {
                    roleDic.Add(item.EnumName, CheckRoleOpened((byte)oldCur.Status, (CryptoStatus)item.EnumValue));
                }
                ViewBag.RoleDic = roleDic;

                List<PriceInfoViewModel> priceList = new List<PriceInfoViewModel>();
                priceList = FoundationDB.DB.Queryable<Currency, PriceInfos>((c, pi) => new object[] { JoinType.Left, c.ID == pi.CurrencyID }).Where((c, pi) => pi.CryptoID == cur.Id).Select((c, pi) =>
             new PriceInfoViewModel { CurrencyName = c.Name, CurrencyCode = c.Code, Price = pi.Price }).ToList();
                ViewBag.PriceList = priceList;

                return View("Edit", oldCur);
            }
            else//新增
            {
                int newId = SaveCreate(cur).Data;
                Cryptocurrencies newCur = FoundationDB.CryptocurrencyDb.GetById(newId);
                Dictionary<string, bool> roleDic = new Dictionary<string, bool>();
                foreach (var item in EnumHelper.EnumToList<CryptoStatus>())
                {
                    roleDic.Add(item.EnumName, CheckRoleOpened((byte)newCur.Status, (CryptoStatus)item.EnumValue));
                }
                ViewBag.RoleDic = roleDic;

                List<PriceInfoViewModel> priceList = new List<PriceInfoViewModel>();
                priceList = FoundationDB.DB.Queryable<Currency, PriceInfos>((c, pi) => new object[] { JoinType.Left, c.ID == pi.CurrencyID }).Where((c, pi) => pi.CryptoID == newId).Select((c, pi) =>
             new PriceInfoViewModel { CurrencyName = c.Name, CurrencyCode = c.Code, Price = pi.Price }).ToList();
                ViewBag.PriceList = priceList;

                return View("Edit", newCur);
            }

        }

        [BOAccess("CryptoCurCreate")]
        private SaveResult<int> SaveCreate(Cryptocurrencies cur)
        {
            List<PriceInfos> list = new List<PriceInfos>();
            CryptocurrenciesBLL ab = new CryptocurrenciesBLL();
            PriceInfoAgent pia = new PriceInfoAgent();
            var result = ab.Create(cur, UserId, UserName);
            if (cur.IsFixedPrice)
            {
                foreach (Currency item in FoundationDB.CurrencyDb.GetList())
                {
                    PriceInfos pi = new PriceInfos();
                    pi.CryptoID = result.Data;
                    pi.CurrencyID = item.ID;
                    pi.LastUpdateDate = DateTime.Now;
                    pi.Price = Convert.ToDecimal(Request.Form[item.Code]);
                    list.Add(pi);
                }
                pia.Insert(list);
            }
            return result;
        }

        [BOAccess("CryptoCurUpdate")]
        private SaveResult SaveEdit(Cryptocurrencies cur)
        {
            List<PriceInfos> list = new List<PriceInfos>();
            if (cur.IsFixedPrice)
            {
                foreach (Currency item in FoundationDB.CurrencyDb.GetList())
                {
                    PriceInfos pi = new PriceInfos();
                    pi.CryptoID = cur.Id;
                    pi.CurrencyID = item.ID;
                    pi.LastUpdateDate = DateTime.Now;
                    pi.Price = Convert.ToDecimal(Request.Form[item.Code]);
                    list.Add(pi);
                }
            }

            CryptocurrenciesBLL ab = new CryptocurrenciesBLL();
            return ab.Update(cur, list, UserId, UserName);
        }
    }
}