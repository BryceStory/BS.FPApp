using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    /// <summary>
    /// FiiiPay门店
    /// </summary>
    [BOAccess("FiiiPayMerchantMenu")]
    public class FiiiPayMerchantController : BaseController
    {
        // GET: FiiiPayMerchant
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.CountryList = ConvertDicToSelect(GetCountryList(), true, allOptionTxt: "全部");
            ViewBag.StatusList = ConvertDicToSelect(GetMerchantStatusList(),true, allOptionTxt: "全部");
            ViewBag.VerifyStatusList = ConvertDicToSelect(GetVerifyStatusList(), true, allOptionTxt:"全部");
            return View();
        }

        public JsonResult LoadData(GridPager pager, string fiiipayAccount,string merchantName,int? countryId,byte? merchantStatus,byte? verifyStatus)
        {
            var list = new FiiiPayMerchantBLL().GetMerchantPager(fiiipayAccount, merchantName, countryId, merchantStatus, verifyStatus, ref pager);
            var countryList = GetCountryList();
            var statusList = GetMerchantStatusList();
            var verifyStatusList = GetVerifyStatusList();
            var allowList = GetAllowExpenseList();
            var fromTypeList = GetFromTypeList();
            var obj = list.ToGridJson(pager, r =>
                    new
                    {
                        id = r.Id,
                        cell = new
                        {
                            r.Id,
                            r.FiiiPayAccount,
                            r.MerchantName,
                            CountryName = (countryList.ContainsKey(r.CountryId) ? countryList[r.CountryId] : ""),
                            MerchantStatus = statusList[(int)r.Status],
                            VerifyStatus = verifyStatusList[(int)r.VerifyStatus],
                            AllowExpense = allowList[r.IsAllowExpense.ToString().ToLower()],
                            FromType = fromTypeList[r.FromType],
                            Options = r.VerifyStatus
                        }
                    });
            return Json(obj);
        }

        /// <summary>
        /// 门店设置
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public PartialViewResult Setting(Guid merchantId)
        {
            var merchantInfo = FiiiPayDB.MerchantInformationDb.GetById(merchantId);
            merchantInfo.Markup = Math.Truncate(merchantInfo.Markup * 10000)/100;
            merchantInfo.FeeRate = Math.Truncate(merchantInfo.FeeRate * 10000)/100;
            ViewBag.StatusList = ConvertDicToSelect(GetMerchantStatusList());
            ViewBag.AllowList = ConvertDicToSelect(GetAllowExpenseList());
            return PartialView(merchantInfo);
        }

        [BOAccess("FiiiPayMerchantSetting")]
        public JsonResult SaveSetting(MerchantInformations info)
        {
            info.FeeRate = Math.Truncate(info.FeeRate * 100) / 10000;
            info.Markup = Math.Truncate(info.Markup * 100) / 10000;
            var sr = new FiiiPayMerchantBLL().SaveSetting(info.Id, info.Status, info.IsAllowExpense, info.FeeRate, info.Markup);
            return Json(sr.toJson());
        }

        public ActionResult Add()
        {
            var cryptoList = GetCrptoCodeList();
            var countryList = FoundationDB.CountryDb.GetList(t => t.IsSupportStore);
            ViewBag.PhoneCodeList = ConvertDicToSelect(countryList.ToDictionary(k => k.Id, v => v.PhoneCode), true);
            ViewBag.CountryCodeList = ConvertDicToSelect(countryList.ToDictionary(k => k.Code, v => v.Name_CN), true);
            ViewBag.StoreTypeList = FiiiPayDB.StoreTypeDb.GetList();
            ViewBag.CryptoList = FoundationDB.CryptocurrencyDb.GetList(t => t.IsWhiteLabel == 0 && cryptoList.Contains(t.Code));
            return View(new MerchantInformations());
        }

        [BOAccess("FiiiPayMerchantCreate")]
        public JsonResult SaveAdd(MerchantEditInfoModel model, string InviteCode)
        {
            var sr = new FiiiPayMerchantBLL().SaveAdd(this.UserName, model, InviteCode);
            return Json(sr.toJson());
        }

        public ActionResult Edit(Guid id)
        {
            MerchantInformations merchantInfo = FiiiPayDB.MerchantInformationDb.GetById(id);
            var fiiipayAccount = FiiiPayDB.UserAccountDb.GetById(merchantInfo.MerchantAccountId);
            ViewBag.FiiiPayAccountName = fiiipayAccount.PhoneCode + " " + fiiipayAccount.Cellphone;

            var countryList = FoundationDB.CountryDb.GetList();
            countryList.Add(new Countries { Code = "", Name_CN = "请选择", IsSupportStore = true });
            ViewBag.CountryCodeList = new SelectList(countryList.Where(t => t.IsSupportStore), "Code", "Name_CN", countryList.Find(t => t.Id == merchantInfo.CountryId)?.Code);
            var cryptoList = GetCrptoCodeList();
            ViewBag.StoreTypeList = FiiiPayDB.StoreTypeDb.GetList();
            ViewBag.MerchantStoreTypeList = FiiiPayDB.MerchantCategoryDb.GetList(t => t.MerchantInformationId == merchantInfo.Id);
            ViewBag.CryptoList = FoundationDB.CryptocurrencyDb.GetList(t => t.IsWhiteLabel == 0 && cryptoList.Contains(t.Code));
            ViewBag.MerchantSupportCryptoList = FiiiPayDB.MerchantSupportCryptoDb.GetList(t => t.MerchantInfoId == merchantInfo.Id);
            ViewBag.FiiipayMerchantProfile = FiiiPayDB.FiiipayMerchantProfileDb.GetSingle(t => t.MerchantInfoId == merchantInfo.Id);
            ViewBag.OwnersFigure = FiiiPayDB.MerchantOwnersFigureDb.GetList(t => t.MerchantInformationId == merchantInfo.Id);
            ViewBag.InviteCode = FiiiPayDB.InviteRecordDb.GetSingle(t => t.AccountId == fiiipayAccount.Id && t.Type == InviteType.FiiipayMerchant)?.InviterCode;
            return View(merchantInfo);
        }

        [BOAccess("FiiiPayMerchantUpdate")]
        public JsonResult SaveEdit(MerchantEditInfoModel model)
        {
            var sr = new FiiiPayMerchantBLL().SaveEdit(this.UserName, model);
            var countryList = FoundationDB.CountryDb.GetList(t => t.IsSupportStore);
            ViewBag.CountryCodeList = ConvertDicToSelect(countryList.ToDictionary(k => k.Code, v => v.Name_CN), true);
            return Json(sr.toJson());
        }

        public string GetAddress(decimal lat,decimal lng,string lang)
        {
            var mapToken = "pk.eyJ1IjoiZmlpaXBheTIwMTkiLCJhIjoiY2pzZmd4OG44MDhxNzN5cHV1YzZpZG85ZCJ9.Dz091UTbErxtERwOXSWTmA";
            var curl = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{lng},{lat}.json?language={lang}&access_token=" + mapToken;
            var jsonstr = Framework.RestUtilities.GetJson(curl);
            return jsonstr;
        }

        public ActionResult Detail(Guid id)
        {
            MerchantInformations merchantInfo = FiiiPayDB.MerchantInformationDb.GetById(id);
            var country = FoundationDB.CountryDb.GetById(merchantInfo.CountryId);

            var fiiipayAccount = FiiiPayDB.UserAccountDb.GetById(merchantInfo.MerchantAccountId);
            ViewBag.CountryName = country.Code.ToUpper() == "CN" ? country.Name_CN : country.Name;
            ViewBag.FiiiPayAccountName = fiiipayAccount.PhoneCode + " " + fiiipayAccount.Cellphone;
            ViewBag.StoreTypeList = FiiiPayDB.StoreTypeDb.GetList();
            ViewBag.MerchantStoreTypeList = FiiiPayDB.MerchantCategoryDb.GetList(t => t.MerchantInformationId == merchantInfo.Id);
            ViewBag.MerchantSupportCryptoList = FiiiPayDB.MerchantSupportCryptoDb.GetList(t => t.MerchantInfoId == merchantInfo.Id);
            FiiipayMerchantProfiles profile = null;
            if (merchantInfo.VerifyStatus != Entities.Enums.VerifyStatus.Certified)
            {
                var recorde = FiiiPayDB.FiiipayMerchantVerifyRecordDb.GetList(t => t.MerchantInfoId == merchantInfo.Id).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                if (recorde != null)
                    profile = new FiiipayMerchantProfiles { MerchantInfoId = merchantInfo.Id, LicenseNo = recorde.LicenseNo, BusinessLicenseImage = recorde.BusinessLicenseImage };
            }
            else
                profile = FiiiPayDB.FiiipayMerchantProfileDb.GetSingle(t => t.MerchantInfoId == merchantInfo.Id);
            ViewBag.FiiipayMerchantProfile = profile;
            ViewBag.FiiipayMerchantVerifyRecordList = FiiiPayDB.FiiipayMerchantVerifyRecordDb.GetList(t => t.MerchantInfoId == id).OrderByDescending(t => t.CreateTime).ToList();
            ViewBag.OwnersFigure = FiiiPayDB.MerchantOwnersFigureDb.GetList(t => t.MerchantInformationId == merchantInfo.Id);
            ViewBag.InviteCode = FiiiPayDB.InviteRecordDb.GetSingle(t => t.AccountId == fiiipayAccount.Id && t.Type == InviteType.FiiipayMerchant)?.InviterCode;

            return View(merchantInfo);
        }

        public ActionResult Verify(Guid id)
        {
            MerchantInformations merchantInfo = FiiiPayDB.MerchantInformationDb.GetById(id);
            var country = FoundationDB.CountryDb.GetById(merchantInfo.CountryId);

            var fiiipayAccount = FiiiPayDB.UserAccountDb.GetById(merchantInfo.MerchantAccountId);
            ViewBag.CountryName = country.Code.ToUpper() == "CN" ? country.Name_CN : country.Name;
            ViewBag.FiiiPayAccountName = fiiipayAccount.PhoneCode + " " + fiiipayAccount.Cellphone;
            ViewBag.StoreTypeList = FiiiPayDB.StoreTypeDb.GetList();
            ViewBag.MerchantStoreTypeList = FiiiPayDB.MerchantCategoryDb.GetList(t => t.MerchantInformationId == merchantInfo.Id);
            ViewBag.MerchantSupportCryptoList = FiiiPayDB.MerchantSupportCryptoDb.GetList(t => t.MerchantInfoId == merchantInfo.Id);
            FiiipayMerchantProfiles profile = null;
            if (merchantInfo.VerifyStatus != Entities.Enums.VerifyStatus.Certified)
            {
                var recorde = FiiiPayDB.FiiipayMerchantVerifyRecordDb.GetList(t => t.MerchantInfoId == merchantInfo.Id).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                if (recorde != null)
                    profile = new FiiipayMerchantProfiles { MerchantInfoId = merchantInfo.Id, LicenseNo = recorde.LicenseNo, BusinessLicenseImage = recorde.BusinessLicenseImage };
            }
            else
                profile = FiiiPayDB.FiiipayMerchantProfileDb.GetSingle(t => t.MerchantInfoId == merchantInfo.Id);
            ViewBag.FiiipayMerchantProfile = profile;
            ViewBag.OwnersFigure = FiiiPayDB.MerchantOwnersFigureDb.GetList(t => t.MerchantInformationId == merchantInfo.Id);
            ViewBag.InviteCode = FiiiPayDB.InviteRecordDb.GetSingle(t => t.AccountId == fiiipayAccount.Id && t.Type == InviteType.FiiipayMerchant)?.InviterCode;

            return View(merchantInfo);
        }

        [HttpPost, BOAccess("FiiiPayMerchantVerify")]
        public JsonResult SaveVerify(Guid Id,byte VerifyResult, string VerifyReason)
        {
            var sr = new FiiiPayMerchantBLL().SaveVerify(Id, VerifyResult, VerifyReason, this.UserName);
            return Json(sr.toJson());
        }

        public PartialViewResult UploadIdentity()
        {
            return PartialView();
        }

        /// <summary>
        /// 上传店面图
        /// </summary>
        /// <returns></returns>
        public PartialViewResult UploadFileId()
        {
            return PartialView();
        }

        public PartialViewResult UploadOwnerFigures()
        {
            return PartialView();
        }

        private Dictionary<int, string> GetCountryList()
        {
            return FoundationDB.CountryDb.GetList().Select(t => new { t.Id, t.Name_CN })
                .ToDictionary(u => u.Id, v => v.Name_CN);
        }
        private Dictionary<string, string> GetAllowExpenseList()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("true", "启用");
            dic.Add("false", "禁用");
            return dic;
        }
        private Dictionary<int,string> GetVerifyStatusList()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)Entities.Enums.VerifyStatus.Uncertified, "未提交");
            dic.Add((int)Entities.Enums.VerifyStatus.UnderApproval, "待审核");
            dic.Add((int)Entities.Enums.VerifyStatus.Certified, "通过");
            dic.Add((int)Entities.Enums.VerifyStatus.Disapproval, "不通过");
            return dic;
        }
        private Dictionary<int, string> GetMerchantStatusList()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)Status.Enabled, "正常");
            dic.Add((int)Status.Stop, "禁用");
            return dic;
        }
        private Dictionary<byte, string> GetFromTypeList()
        {
            Dictionary<byte, string> dic = new Dictionary<byte, string>();
            dic.Add((byte)InputFromType.BOInput, "录入");
            dic.Add((byte)InputFromType.UserInput, "商家入驻");
            return dic;
        }
        private List<string> GetCrptoCodeList()
        {
            return new List<string>()
            {
                "FIII","ETH","BCHABC","USDT","BTC","LTC","XRP"
            };
        }
    }
}