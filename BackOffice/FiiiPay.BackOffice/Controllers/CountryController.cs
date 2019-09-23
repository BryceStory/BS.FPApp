using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.Foundation.Entities;
using SqlSugar;
using System.Configuration;

namespace FiiiPay.BackOffice.Controllers
{
    public class CountryController : BaseController
    {
        [BOAccess("CountryManageMenu")]
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            return View();
        }
        [HttpPost]
        public JsonResult LoadData(GridPager pager)
        {
            var obj = FoundationDB.CountryDb.GetList()
               .ToGridJson(ref pager, r =>
                   new
                   {
                       id = r.Id,
                       cell = new
                       {
                           Id = r.Id,
                           Name = r.Name,
                           Name_CN = r.Name_CN,
                           PinYin = r.PinYin,
                           FiatCurrency = r.FiatCurrency,
                           Code = r.Code,
                           PhoneCode = r.PhoneCode
                           //CustomerService = r.CustomerService
                       }
                   });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int CountryId)
        {
            var model = GetModel(CountryId);

            List<Countries> list = FoundationDB.CountryDb.GetList();
            List<SelectListItem> oList = new List<SelectListItem>();
            foreach (var item in list)
            {
                oList.Add(new SelectListItem() { Text = item.PhoneCode + " " + item.Code, Value = item.Id.ToString() });
            }

            List<SelectListItem> funcList = new List<SelectListItem>();
            funcList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
            funcList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
            ViewBag.FuncList = funcList;

            ViewBag.List = oList;
            return View(model);
        }



        public ActionResult Detail(int CountryId)
        {
            CountryViewModel model = new CountryViewModel();
            model.Id = -1;
            if (CountryId > 0)
            {
                model = FoundationDB.DB.Queryable<Countries, ProfileRouters>
                    ((country, profileRouter) => new object[]
                    {
                        JoinType.Left, country.Id == profileRouter.Country
                    }).Where((country, profileRouter) => country.Id == CountryId)
                    .Select((country, profileRouter) => new CountryViewModel
                    {
                        Id = country.Id,
                        Name = country.Name,
                        Name_CN = country.Name_CN,
                        PhoneCode = country.PhoneCode,
                        //CustomerService = country.CustomerService,
                        PinYin = country.PinYin,
                        FiatCurrency = country.FiatCurrency,
                        Code = country.Code,
                        //IsHot = country.IsHot,
                        IsSupportStore = country.IsSupportStore,
                        Status = country.Status,
                        ProfileServerAddress = profileRouter.ServerAddress,
                        ProfileClientKey = profileRouter.ClientKey,
                        ProfileSecretKey = profileRouter.SecretKey
                    }).First();
            }
            return View(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(CountryViewModel oCountry)
        {
            //if (Request.Form["IsHot"] != null)
            //    oCountry.IsHot = Request.Form["IsHot"].ToString() == "on" ? true : false;
            if (Request.Form["IsSupportStore"] != null)
                oCountry.IsSupportStore = Request.Form["IsSupportStore"].ToString() == "on" ? true : false;
            //上传图片
            HttpPostedFileBase IconFile = Request.Files["NationalFlagURL"];
            if (IconFile.ContentLength != 0)
            {
                oCountry.NationalFlagURL = new Guid(new Utils.FileUploader().UpImageToCDN(IconFile));
            }

            if (oCountry.Id > 0)//编辑
            {
                SaveEdit(oCountry);
                var model = GetModel(oCountry.Id);
                List<Countries> list = FoundationDB.CountryDb.GetList();
                List<SelectListItem> oList = new List<SelectListItem>();
                foreach (var item in list)
                {
                    oList.Add(new SelectListItem() { Text = item.PhoneCode + " " + item.Code, Value = item.Id.ToString() });
                }
                List<SelectListItem> funcList = new List<SelectListItem>();
                funcList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
                funcList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
                ViewBag.FuncList = funcList;

                ViewBag.List = oList;

                return View("Edit", model);
            }
            else//新增
            {
                int newId = SaveCreate(oCountry).Data;
                var model = GetModel(newId);
                List<Countries> list = FoundationDB.CountryDb.GetList();
                List<SelectListItem> oList = new List<SelectListItem>();
                foreach (var item in list)
                {
                    oList.Add(new SelectListItem() { Text = item.PhoneCode + " " + item.Code, Value = item.Id.ToString() });
                }
                List<SelectListItem> funcList = new List<SelectListItem>();
                funcList.Add(new SelectListItem() { Text = "Enable", Value = "1" });
                funcList.Add(new SelectListItem() { Text = "Disable", Value = "0" });
                ViewBag.FuncList = funcList;

                ViewBag.List = oList;

                return View("Edit", model);
            }
        }


        [BOAccess("CountryCreate")]
        private SaveResult<int> SaveCreate(CountryViewModel oCountry)
        {
            CountryBLL cb = new CountryBLL();
            return cb.Create(oCountry, UserId, UserName);
        }

        [BOAccess("CountryUpdate")]
        private SaveResult SaveEdit(CountryViewModel oCountry)
        {
            CountryBLL cb = new CountryBLL();
            return cb.Update(oCountry, UserId, UserName);
        }

        [BOAccess("CountryDelete")]
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Delete(int countryId)
        {
            CountryBLL cb = new CountryBLL();
            return Json(cb.DeleteById(countryId, UserId, UserName).toJson());
        }
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult CheckRouters(string Type, string ServerAddress, string ClientKey, string SecretKey)
        {
            CountryBLL cb = new CountryBLL();
            return Json(cb.CheckRouters(Type, ServerAddress, ClientKey, SecretKey).toJson());
        }

        private CountryViewModel GetModel(int CountryId)
        {
            CountryViewModel model = new CountryViewModel();
            model.Id = -1;
            if (CountryId > 0)
            {
                model = FoundationDB.DB.Queryable<Countries, ProfileRouters>
                    ((country, profileRouter) => new object[]
                    {
                        JoinType.Left, country.Id == profileRouter.Country
                    }).Where((country, profileRouter) => country.Id == CountryId)
                    .Select((country, profileRouter) => new CountryViewModel
                    {
                        Id = country.Id,
                        Name = country.Name,
                        Name_CN = country.Name_CN,
                        PhoneCode = country.PhoneCode,
                        //CustomerService = country.CustomerService,
                        PinYin = country.PinYin,
                        FiatCurrency = country.FiatCurrency,
                        Code = country.Code,
                        //IsHot = country.IsHot,
                        IsSupportStore = country.IsSupportStore,
                        Status = country.Status,
                        //FiatCurrencySymbol = country.FiatCurrencySymbol,
                        NationalFlagURL = country.NationalFlagURL,
                        ProfileServerAddress = profileRouter.ServerAddress,
                        ProfileClientKey = profileRouter.ClientKey,
                        ProfileSecretKey = profileRouter.SecretKey
                    }).First();
            }
            else
            {
                model.ProfileServerAddress = ConfigurationManager.AppSettings["GlobalProfile_URL"];
                model.ProfileClientKey = "FiiiPay";
                model.ProfileSecretKey = ConfigurationManager.AppSettings["GlobalProfile_SecretKey"];
            }
            return model;
        }
    }
}