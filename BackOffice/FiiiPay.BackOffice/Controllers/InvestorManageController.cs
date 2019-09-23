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
using FiiiPay.Framework.Component;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("InvestorManageMenu")]
    public class InvestorManageController : BaseController
    {
        public ActionResult Index(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.PageId = id;
            return View();
        }

        public JsonResult LoadData(GridPager pager, string Username, string InvestorName, string CellPhone)
        {
            List<InvestorAccounts> data = FiiiPayDB.DB.Queryable<InvestorAccounts>()
                .WhereIF(!string.IsNullOrEmpty(Username), investor => investor.Username.Contains(Username))
                .WhereIF(!string.IsNullOrEmpty(InvestorName), investor => investor.InvestorName.Contains(InvestorName))
                .WhereIF(!string.IsNullOrEmpty(CellPhone), investor => investor.Cellphone.Contains(CellPhone)).ToList();
            var obj = data.ToGridJson(ref pager, r =>
            new
            {
                id = r.Id,
                cell = new string[]{
                        r.Id.ToString(),
                        r.Username,
                        r.InvestorName,
                        r.Cellphone,
                        r.Balance.ToString(),
                        r.Status.ToString(),
                        r.RegistrationDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                    }
            });
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int InvestorId)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem() { Text = "Active", Value = "1", Selected = true });
            statusList.Add(new SelectListItem() { Text = "Locked", Value = "0" });
            ViewBag.StatusList = statusList;

            InvestorAccounts investor = new InvestorAccounts();
            investor.Id = -1;
            if (InvestorId > 0)
            {
                investor = FiiiPayDB.InvestorAccountDb.GetById(InvestorId);
            }
            return PartialView(investor);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult Save(InvestorAccounts oInvestor)
        {
            if (oInvestor.Id > 0)//编辑
            {
                return Json(SaveEdit(oInvestor).toJson());
            }
            else//新增
            {
                return Json(SaveCreate(oInvestor).toJson());
            }
        }


        [BOAccess("InvestorCreate")]
        private SaveResult SaveCreate(InvestorAccounts oInvestor)
        {
            InvestorAccountBLL ab = new InvestorAccountBLL();
            oInvestor.RegistrationDate = DateTime.UtcNow;
            oInvestor.PIN = PasswordHasher.HashPassword(oInvestor.PIN);
            oInvestor.Password = PasswordHasher.HashPassword(oInvestor.Password);
            oInvestor.Balance = 0;
            return ab.Create(oInvestor, UserId, UserName);
        }

        [BOAccess("InvestorUpdate")]
        private SaveResult SaveEdit(InvestorAccounts oInvestor)
        {
            InvestorAccountBLL ab = new InvestorAccountBLL();
            return ab.Update(oInvestor, UserId, UserName);
        }


        public ActionResult Deposit(string Type, int InvestorId, decimal Balance)
        {
            ViewBag.Type = Type;
            ViewBag.Balance = Balance;
            InvestorAccounts investor = new InvestorAccounts();
            investor = FiiiPayDB.InvestorAccountDb.GetById(InvestorId);
            return PartialView(investor);
        }

        [HttpPost, AjaxAntiForgeryToken]
        public ActionResult SaveDeposit(int id, decimal amount, string remark, string type)
        {
            InvestorWalletStatements oStatement = new InvestorWalletStatements();
            oStatement.InvestorId = id;
            oStatement.Amount = amount;
            oStatement.Remark = remark;
            oStatement.Timestamp = DateTime.UtcNow;
            if (type.Equals("Deposit"))//充币
            {
                oStatement.Action = InvestorTransactionType.Deposit;
                return Json(SaveDeposit(oStatement).toJson());
            }
            else//扣币
            {
                oStatement.Action = InvestorTransactionType.Withhold;
                return Json(SaveWithhold(oStatement).toJson());
            }
        }


        [BOAccess("InvestorDeposit")]
        private SaveResult SaveDeposit(InvestorWalletStatements oStatement)
        {
            InvestorAccounts investor = FiiiPayDB.InvestorAccountDb.GetById(oStatement.InvestorId);
            decimal balance = investor.Balance + oStatement.Amount;
            investor.Balance = balance;
            oStatement.Balance = balance;

            InvestorWalletStatementBLL ab = new InvestorWalletStatementBLL();
            return ab.Create(oStatement, investor, UserId, UserName);
        }

        [BOAccess("InvestorWithhold")]
        private SaveResult SaveWithhold(InvestorWalletStatements oStatement)
        {
            InvestorAccounts investor = FiiiPayDB.InvestorAccountDb.GetById(oStatement.InvestorId);
            decimal balance = investor.Balance - oStatement.Amount;
            if (balance < 0)
            {
                return new SaveResult(false, "Amount cannot greater than balance");
            }
            investor.Balance = balance;
            oStatement.Balance = balance;

            InvestorWalletStatementBLL ab = new InvestorWalletStatementBLL();
            return ab.Create(oStatement, investor, UserId, UserName);
        }

        public ActionResult ResetPassword(int InvestorId)
        {
            InvestorAccounts investor = FiiiPayDB.InvestorAccountDb.GetById(InvestorId);
            return PartialView(investor);
        }
        public ActionResult ResetPIN(int InvestorId)
        {
            InvestorAccounts investor = FiiiPayDB.InvestorAccountDb.GetById(InvestorId);
            return PartialView(investor);
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("InvestorResetPassword")]
        public ActionResult SavePassword(int InvestorId, string newPassword)
        {
            InvestorAccountBLL ab = new InvestorAccountBLL();
            InvestorAccounts investor = FiiiPayDB.InvestorAccountDb.GetById(InvestorId);
            investor.Password = PasswordHasher.HashPassword(newPassword);
            return Json(ab.UpdatePassword(investor, UserId, UserName).toJson());
        }

        [HttpPost, AjaxAntiForgeryToken]
        [BOAccess("InvestorResetPIN")]
        public ActionResult SavePIN(int InvestorId, string newPIN)
        {
            InvestorAccountBLL ab = new InvestorAccountBLL();
            InvestorAccounts investor = FiiiPayDB.InvestorAccountDb.GetById(InvestorId);
            investor.PIN = PasswordHasher.HashPassword(newPIN);
            return Json(ab.UpdatePIN(investor, UserId, UserName).toJson());
        }

        public ActionResult Statements(int id)
        {
            ViewBag.PagePermissions = GetPermissionCodeList(id);
            ViewBag.PageId = id;
            return View();
        }
    }
}