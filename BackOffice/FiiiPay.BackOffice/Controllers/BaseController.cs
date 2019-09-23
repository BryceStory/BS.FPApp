using FiiiPay.BackOffice.Common;
using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Framework.Cache;
using FiiiPay.BackOffice.Utils;

namespace FiiiPay.BackOffice.Controllers
{
    public class BaseController : Controller
    {
        public BOContext BoDB
        {
            get
            {
                return DataContext.GetDbContext<BOContext>();
            }
        }
        public FiiiPayContext FiiiPayDB
        {
            get
            {
                return DataContext.GetDbContext<FiiiPayContext>();
            }
        }

        public FoundationContext FoundationDB
        {
            get
            {
                return DataContext.GetDbContext<FoundationContext>();
            }
        }
        public FiiiPayEnterpriseContext FiiiPayEnterpriseDB
        {
            get
            {
                return DataContext.GetDbContext<FiiiPayEnterpriseContext>();
            }
        }

        private LoginUser _loginUser { get; set; }

        public LoginUser LoginUser
        {
            get
            {
                if (_loginUser == null)
                {
                    var loginId = Request.Cookies["LoginUser"];
                    if (loginId == null)
                        return null;
                    var cookieValue = Encrypts.GetDecryptString(loginId.Value);
                    var cookieValues = cookieValue.Split(new char[] { '_' });
                    if (cookieValues == null || cookieValues.Length != 2)
                        return null;
                    string token = cookieValues[0];
                    _loginUser = RedisHelper.Get<LoginUser>("loginuser" + token);
                }
                return _loginUser;
            }
        }

        public bool IsAdmin { get { return LoginUser.IsAdmin; } }

        public int UserId { get { return LoginUser.UserId; } }

        public string UserName { get { return LoginUser.UserName; } }

        public int RoleId { get { return LoginUser.RoleId; } }

        public string RoleName { get { return LoginUser.RoleName; } }

        public List<UserPermission> PerimissionList { get { return LoginUser.PerimissionList; } }
        
        public List<string> PermissionCodeList
        {
            get
            {
                if (LoginUser.PerimissionList == null)
                    return null;
                return LoginUser.PerimissionList.Select(t => t.PerimCode).ToList();
            }
        }
        
        public List<string> GetPermissionCodeList(int moduleId)
        {
            if (LoginUser.PerimissionList == null)
                return null;
            return LoginUser.PerimissionList.Where(t => t.ModuleId == moduleId).Select(t => t.PerimCode).ToList();
        }

        public bool CheckPerimission(string key)
        {
            if (LoginUser.PerimissionList == null)
                return false;
            return LoginUser.PerimissionList.Select(t => t.PerimCode).Contains(key);
        }

        protected List<SelectListItem> ConvertDicToSelect<T>(Dictionary<T, string> dic,bool addAllOption=false, string allOptionTxt="请选择",string allOptionValue="")
        {
            if (dic == null)
                return null;
            List<SelectListItem> olist = new List<SelectListItem>();
            if(addAllOption)
                olist.Add(new SelectListItem { Text = allOptionTxt, Value = allOptionValue });
            foreach (var item in dic)
            {
                olist.Add(new SelectListItem { Text = item.Value, Value = item.Key.ToString() });
            }
            return olist;
        }
    }
}