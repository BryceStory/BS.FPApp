using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.Framework;
using FiiiPay.Framework.Enums;
using System;
using System.Web.Mvc;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Authenticator;

namespace FiiiPay.BackOffice.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public FileResult ShowImage(string gid)
        {
            byte[] thumb = null;
            string code = RandomAlphaNumericGenerator.Generate(4);
            try
            {
                thumb = new VerificationCode().CreateImageCode(code);
                RedisHelper.StringSet(gid, code, new TimeSpan(0, 5, 0));
            }
            catch (Exception)
            {
                thumb = System.IO.File.ReadAllBytes(Server.MapPath("~/Content/Images/NoFileFound.png"));
            }
            return File(thumb, "image/jpg");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Account account,string TokenGid, string VerificationCode)
        {
            var securityVerify = new SecurityVerification(SystemPlatform.BackOffice);

            var loginBll = new LoginBLL();
            string loginMessage = String.Empty;
            try
            {
                var loginErrorCountsInt = securityVerify.CheckErrorCount(SecurityMethod.Password, account.Username);

                var cacheCode = RedisHelper.StringGet(TokenGid);
                if (string.IsNullOrEmpty(cacheCode))
                {
                    loginMessage= "Verification code was expired";
                    securityVerify.IncreaseErrorCount(SecurityMethod.Password, account.Username);
                }
                if (VerificationCode.ToUpper() != cacheCode.ToUpper())
                {
                    loginMessage = "Verification code is wrong";
                    securityVerify.IncreaseErrorCount(SecurityMethod.Password, account.Username);
                }
                bool checkResult = loginBll.CheckUser(account.Username, account.Password, out account, ref loginMessage);
                if (!checkResult)
                {
                    securityVerify.IncreaseErrorCount(SecurityMethod.Password, account.Username);
                }
                RedisHelper.KeyDelete(TokenGid);
                securityVerify.DeleteErrorCount(SecurityMethod.Password, account.Username);
            }
            catch(Framework.Exceptions.CommonException ex)
            {
                ViewBag.LoginMessage = string.IsNullOrEmpty(loginMessage) ? ex.Message : loginMessage;
                return View(account);
            }
            
            LoginUser lu = new LoginUser();
            int roleId = account.RoleId.Value;
            lu.UserId = account.Id;
            lu.UserName = account.Username;
            lu.RoleId = roleId;
            lu.IsAdmin = false;// account.Username == "fiiipayadmin";
            if(lu.IsAdmin)
                lu.PerimissionList = loginBll.GetAllPermission(roleId);
            else
                lu.PerimissionList = loginBll.GetUserPermissionByRoleId(roleId);

            RedisHelper.Set("loginuser" + account.Id, lu, new TimeSpan(1, 0, 0));

            var userCookie = Request.Cookies["LoginUser"];
            if (userCookie == null)
            {
                var cookie = Response.Cookies["LoginUser"];
                cookie.Value = Encrypts.GetEncryptString(account.Id.ToString());
                cookie.HttpOnly = true;
                cookie.Expires = DateTime.Now.AddDays(1);
            }
            else
            {
                Response.Cookies.Add(Request.Cookies["LoginUser"]);
                Response.Cookies["LoginUser"].Value = Encrypts.GetEncryptString(account.Id.ToString());
                Response.Cookies["LoginUser"].Expires = DateTime.Now.AddDays(1);
                Response.Cookies["LoginUser"].HttpOnly = true;
            }

            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            string token = Request.Cookies["LoginUser"] == null ? "" : Request.Cookies["LoginUser"].Value;
            Request.Cookies["LoginUser"].Expires = DateTime.Now;
            RedisHelper.KeyDelete("loginuser" + token);
            return Json(new { Status = "success" });
        }
    }
}