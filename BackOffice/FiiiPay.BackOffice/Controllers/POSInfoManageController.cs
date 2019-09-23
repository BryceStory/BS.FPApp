using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.Component;
using System.IO;
using FiiiPay.Entities;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("POSInfoManageMenu")]
    public class POSInfoManageController : BaseController
    {
        // GET: POSInfoManage
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = GetCountrySelectList();

            return View();
        }
        public ActionResult LoadData(GridPager pager, string merchantName, string cellPhone, string username, string possn, AccountStatus? status, int? countryId)
        {
            POSBLL pb = new POSBLL();
            var data = pb.GetPOSInfoList(merchantName, username, cellPhone, status, possn, countryId, ref pager);
            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            var cotunryList = FoundationDB.CountryDb.GetList();
            var obj = data.ToGridJson(pager,
                r => new
                {
                    id = r.Id,
                    cell = new
                    {
                        Id = r.Id.ToString(),
                        Username = r.Username,
                        Sn = r.Sn,
                        Cellphone = r.Cellphone,
                        MerchantName = r.MerchantName,
                        CountryName = cotunryList.Where(t => t.Id == r.CountryId).Select(t => t.Name).FirstOrDefault(),
                        Email = r.Email,
                        //DefaultCrypto = coinList.Where(x => x.Id == (FiiiPayDB.DB.Queryable<MerchantWallets>().Where(t => t.MerchantAccountId == r.Id).Where(t => t.IsDefault == 1).ToList().Select(t => t.CryptoId).FirstOrDefault())).Select(t => t.Name).FirstOrDefault(),
                        Status = r.Status.ToString(),
                        IsAllowWithdrawal = r.IsAllowWithdrawal.ToString(),
                        IsAllowAcceptPayment = r.IsAllowAcceptPayment.ToString()
                    }
                });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        private List<SelectListItem> GetCountrySelectList()
        {
            List<SelectListItem> oList = new List<SelectListItem>();
            oList.Add(new SelectListItem() { Text = "Select country" });
            var countryList = FoundationDB.CountryDb.GetList();
            if (countryList != null && countryList.Count > 0)
            {
                oList.AddRange(countryList.Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString() }));
            }
            return oList;
        }

        public ActionResult Detail(Guid id)
        {
            MerchantProfileBLL pb = new MerchantProfileBLL();
            var model = pb.GetMerchantProfileSet(id);
            model.CountryName = FoundationDB.CountryDb.GetById(model.CountryId).Name;
            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            POSBLL pb = new POSBLL();
            var model = pb.GetPOSInfoById(id);

            model.Receivables_Tier = model.Receivables_Tier * 100;
            model.Markup = model.Markup * 100;

            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Actived", Value = "1" });
            statusList.Add(new SelectListItem() { Text = "Locked", Value = "0" });
            ViewBag.StatusList = statusList;

            List<SelectListItem> funcList = new List<SelectListItem>();
            funcList.Add(new SelectListItem() { Text = "Enable", Value = "true" });
            funcList.Add(new SelectListItem() { Text = "Disable", Value = "false" });
            ViewBag.FuncList = funcList;

            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            var table = FiiiPayDB.DB.Queryable<MerchantWallets>().Where(t => t.MerchantAccountId == id).ToList();
            List<SelectListItem> oList = new List<SelectListItem>();

            if (table != null && table.Count > 0)
            {
                foreach (var item in table)
                {
                    oList.Add(new SelectListItem() { Text = coinList.Where(t => t.Id == item.CryptoId).Select(t => t.Name).FirstOrDefault(), Value = item.Id.ToString() });
                }
            }
            ViewBag.CURList = oList;

            return PartialView(model);
        }


        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult GetDefTier()
        {
            List<MasterSettings> list = FoundationDB.DB.Queryable<MasterSettings>().Where(r => r.Group == "Merchant").ToList();
            return Json(new SaveResult(true, (Convert.ToDecimal(list.Find(c => c.Name.Equals("Merchant_TransactionFee")).Value) * 100).ToString("0.00")).toJson());
        }
        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult GetDefMarkup()
        {
            List<MasterSettings> list = FoundationDB.DB.Queryable<MasterSettings>().Where(r => r.Group == "Merchant").ToList();
            return Json(new SaveResult(true, (Convert.ToDecimal(list.Find(c => c.Name.Equals("Merchant_Markup")).Value) * 100).ToString("0")).toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Unbind(Guid id)
        {
            POSBLL pb = new POSBLL();
            return Json(pb.GoogleUnbind(id, UserId, UserName).toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("POSInfoUpdate")]
        public ActionResult Save(POSViewModel oModel)
        {
            POSBLL pb = new POSBLL();
            return Json(pb.SaveEdit(oModel, UserId, UserName).toJson());
        }

        public ActionResult POSAccount(Guid id)
        {
            var coinList = FoundationDB.CryptocurrencyDb.GetList();
            var list = FiiiPayDB.DB.Queryable<MerchantWallets>().Where(t => t.MerchantAccountId == id).ToList();
            List<WalletsViewModel> walletsList = new List<WalletsViewModel>();
            foreach (var item in list)
            {
                WalletsViewModel model = new WalletsViewModel();
                model.Balance = item.Balance;
                model.CurrencyName = coinList.Where(t => t.Id == item.CryptoId).Select(t => t.Name).FirstOrDefault();
                model.FrozenBalance = item.FrozenBalance;
                walletsList.Add(model);
            }
            ViewBag.WalletsList = walletsList;
            return View();
        }

        public ActionResult ResetPIN(Guid Id)
        {
            POSBLL pb = new POSBLL();
            var model = pb.GetPOSInfoById(Id);
            return PartialView(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("ResetMerchantPIN")]
        public ActionResult SavePIN(string AccountId, string NewPIN)
        {
            MerchantBLL ab = new MerchantBLL();
            MerchantAccounts account = FiiiPayDB.MerchantAccountDb.GetById(AccountId);

            account.PIN = PasswordHasher.HashPassword(NewPIN);
            return Json(ab.UpdatePIN(account, UserId, UserName).toJson());
        }
        public ActionResult QRCode(Guid id)
        {
            MerchantProfileBLL pb = new MerchantProfileBLL();
            var model = pb.GetMerchantProfileSet(id);
            return PartialView(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DownloadEn()
        {
            string merchantId = Request.Form["QRCodeMerchantId"];

            MerchantAccount merchant = new MerchantBLL().GetById(new Guid(merchantId));

            string strCode = FiiiPay.Framework.Component.QRCode.Generate(SystemPlatform.FiiiPOS, QRCodeEnum.MerchantScanPay, merchant.Id.ToString());

            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(qrCodeData);
            Bitmap qrCodeImage = qrcode.GetGraphic(17, Color.Black, Color.White, null, 15, 6, false);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            string bgImagePath = @"E:\API\BackOffice\FiiiPay.BackOffice\images\bg-en.jpg";
            List<ImageMergeEntity> imgs = new List<ImageMergeEntity>();
            var bm = new Bitmap(bgImagePath);
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Image,
                Image = new Bitmap(bgImagePath),
                Index = 0
            });
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Image,
                Image = new Bitmap(ms),
                Index = 0,
                X = 300,
                Y = 620,
                AreaHeight = 650,
                AreaWidth = 650
            });
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Text,
                Text = merchant.MerchantName,
                FontSize = 50,
                FontColor = Color.Black,
                Index = 4,
                X = 220,
                Y = 1310,
                AreaHeight = 80,
                AreaWidth = 785
            });
            var imgByte = ImageHelper.ImageMerge(imgs.ToArray());

            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(merchantId + ".jpg", System.Text.Encoding.UTF8));
            Response.BinaryWrite(imgByte);
            Response.Flush();
            return null;
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DownloadZh()
        {
            string merchantId = Request.Form["QRCodeMerchantId"];

            MerchantAccount merchant = new MerchantBLL().GetById(new Guid(merchantId));

            string strCode = FiiiPay.Framework.Component.QRCode.Generate(SystemPlatform.FiiiPOS, QRCodeEnum.MerchantScanPay, merchant.Id.ToString());

            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(qrCodeData);
            Bitmap qrCodeImage = qrcode.GetGraphic(17, Color.Black, Color.White, null, 15, 6, false);
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Jpeg);

            string bgImagePath = @"E:\API\BackOffice\FiiiPay.BackOffice\images\bg-zh.jpg";
            List<ImageMergeEntity> imgs = new List<ImageMergeEntity>();
            var bm = new Bitmap(bgImagePath);
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Image,
                Image = new Bitmap(bgImagePath),
                Index = 0
            });
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Image,
                Image = new Bitmap(ms),
                Index = 0,
                X = 300,
                Y = 620,
                AreaHeight = 650,
                AreaWidth = 650
            });
            imgs.Add(new ImageMergeEntity
            {
                ContentType = ImageContentType.Text,
                Text = merchant.MerchantName,
                FontSize = 50,
                FontColor = Color.Black,
                Index = 4,
                X = 220,
                Y = 1310,
                AreaHeight = 80,
                AreaWidth = 785
            });
            var imgByte = ImageHelper.ImageMerge(imgs.ToArray());
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(merchantId + ".jpg", System.Text.Encoding.UTF8));
            Response.BinaryWrite(imgByte);
            Response.Flush();
            return null;
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult DeleteErrorCount(Guid id, string type)
        {
            MerchantBLL ab = new MerchantBLL();
            return Json(ab.DeleteErrorCount(id, type, UserId, UserName).toJson());
        }
    }
}