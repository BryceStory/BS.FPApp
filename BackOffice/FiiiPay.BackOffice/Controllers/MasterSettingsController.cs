using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("MasterSettingsMenu")]
    public class MasterSettingsController : BaseController
    {
        // GET: MasterSettings
        public ActionResult Index(int id)
        {
            var groupList = new List<string> { "Merchant", "FiiicoinRate", "BillerMaxAmount", "RedPocket" };
            List<MasterSettings> list = FoundationDB.DB.Queryable<MasterSettings>().Where(r => groupList.Contains(r.Group)).ToList();

            MasterSettingListModel model = new MasterSettingListModel
            {
                Merchant_Markup = list.Find(t => t.Group == "Merchant" && t.Name == "Merchant_Markup")?.Value,
                Merchant_TransactionFee = list.Find(t => t.Group == "Merchant" && t.Name == "Merchant_TransactionFee")?.Value,
                Biller_Day_MaxAmount = list.Find(t => t.Group == "BillerMaxAmount" && t.Name == "Biller_Day_MaxAmount")?.Value,
                Biller_Month_MaxAmount = list.Find(t => t.Group == "BillerMaxAmount" && t.Name == "Biller_Month_MaxAmount")?.Value,
                Biller_MaxAmount = list.Find(t => t.Group == "BillerMaxAmount" && t.Name == "Biller_MaxAmount")?.Value,
                DiscountRate = list.Find(t => t.Group == "BillerMaxAmount" && t.Name == "DiscountRate")?.Value,
                BillerEnable = list.Find(t => t.Group == "BillerMaxAmount" && t.Name == "BillerEnable")?.Value,
                Error_Tolerant_Rate = list.Find(t => t.Group == "BillerMaxAmount" && t.Name == "Error_Tolerant_Rate")?.Value,
                RedPocket_AmountLimit = list.Find(t => t.Group == "RedPocket" && t.Name == "RedPocket_AmountLimit")?.Value,
                RedPocket_CountLimit = list.Find(t => t.Group == "RedPocket" && t.Name == "RedPocket_CountLimit")?.Value,
            };

            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "True", Selected = list.Find(t => t.Name.Equals("BillerEnable")).Value == "true", Value = "true" });
            statusList.Add(new SelectListItem() { Text = "False", Selected = list.Find(t => t.Name.Equals("BillerEnable")).Value == "false", Value = "false" });
            ViewBag.StatusList = statusList;
            ViewBag.PagePermissions = GetPermissionCodeList(id);

            return View(model);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("MasterSettingsUpdate")]
        public ActionResult Save(MasterSettingListModel model)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            dic.Add("Merchant", new List<string>()
            {
                $"Merchant_Markup,{model.Merchant_Markup}",
                $"Merchant_TransactionFee,{model.Merchant_TransactionFee}"
            });
            dic.Add("BillerMaxAmount", new List<string>()
            {
                $"Biller_Day_MaxAmount,{model.Biller_Day_MaxAmount}",
                $"Biller_Month_MaxAmount,{model.Biller_Month_MaxAmount}",
                $"Biller_MaxAmount,{model.Biller_MaxAmount}",
                $"DiscountRate,{model.DiscountRate}",
                $"BillerEnable,{model.BillerEnable}",
                $"Error_Tolerant_Rate,{model.Error_Tolerant_Rate}"
            });
            dic.Add("RedPocket", new List<string>()
            {
                $"RedPocket_AmountLimit,{model.RedPocket_AmountLimit}",
                $"RedPocket_CountLimit,{model.RedPocket_CountLimit}"
            });

            var result = new MasterSettingBLL().UpdateBatch(dic, UserId, UserName);
            return Json(result.toJson());
        }
    }
}