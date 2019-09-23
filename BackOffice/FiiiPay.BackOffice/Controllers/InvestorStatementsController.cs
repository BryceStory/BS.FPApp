using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Entities.Enums;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("InvestorStatementsMenu")]
    public class InvestorStatementsController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.PageId = id;
            List<SelectListItem> actionList = new List<SelectListItem>();
            actionList = EnumHelper.GetEnumSelectList<InvestorTransactionType>("", true);
            ViewBag.ActionList = actionList;
            return View();
        }

        public JsonResult LoadData(GridPager pager, string Username, string InvestorName, string CellPhone, int? Action)
        {
            List<InvestorStatementsViewModel> data = FiiiPayDB.DB.Queryable<InvestorWalletStatements, InvestorAccounts>((statements, investor) => new object[] {
                JoinType.Left,statements.InvestorId == investor.Id})
                .WhereIF(!string.IsNullOrEmpty(Username), (statements, investor) => investor.Username.Contains(Username))
                .WhereIF(!string.IsNullOrEmpty(InvestorName), (statements, investor) => investor.InvestorName.Contains(InvestorName))
                .WhereIF(!string.IsNullOrEmpty(CellPhone), (statements, investor) => investor.Cellphone.Contains(CellPhone))
                .WhereIF(Action.HasValue, (statements, investor) => (int)statements.Action == Action.Value)
                .Select((statements, investor) => new InvestorStatementsViewModel
                { Id = statements.Id, Username = investor.Username, InvestorName = investor.InvestorName, Cellphone = investor.Cellphone, Amount = statements.Amount, Action = statements.Action, Timestamp = statements.Timestamp, Remark = statements.Remark })
                .ToList();

            var obj = data.ToGridJson(ref pager, r =>
            new
            {
                id = r.Id,
                cell = new string[]{
                        r.Id.ToString(),
                        r.Username,
                        r.InvestorName,
                        r.Cellphone,
                        r.Action.ToString(),
                        r.Amount.ToString(),
                        r.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        r.Remark
                    }
            });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}