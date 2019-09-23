using FiiiPay.BackOffice.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiiiPay.Framework.Cache;
using FiiiPay.BackOffice.Utils;

namespace FiiiPay.BackOffice
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class BOAccessAttribute : AuthorizeAttribute
    {
        public BOAccessAttribute()
        {
        }
        public BOAccessAttribute(params string[] perimCode)
        {
            PerimCode = perimCode;
        }
        public string[] PerimCode { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string token = "loginuser";
            var loginId = httpContext.Request.Cookies["LoginUser"];
            if (loginId == null)
                return false;

            var cookieValue = Encrypts.GetDecryptString(loginId.Value);
            var cookieValues = cookieValue.Split(new char[] { '_' });
            if (cookieValues == null || cookieValues.Length != 2)
                return false;

            var userId = int.Parse(cookieValues[0]);
            var ticks = long.Parse(cookieValues[1]);

            DateTime dtNow = DateTime.UtcNow;
            DateTime dtToken = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(ticks);

            var totalMinutes = (dtNow - dtToken).TotalMinutes;
            if ((dtNow - dtToken).TotalMinutes > 125)//长时间没访问
                return false;

            token += userId;
            var loginSession = RedisHelper.Get<LoginUser>(token);
            if (loginSession == null)
            {
                if (httpContext.Request.IsAjaxRequest())
                {
                    httpContext.Response.AddHeader("sessionstatus", "timeout");
                    return false;
                }
                return false;
            }

            if ((dtNow - dtToken).TotalMinutes > 5)//最少5分钟更新一次Redis
            {
                httpContext.Response.Cookies.Add(httpContext.Request.Cookies["LoginUser"]);
                httpContext.Response.Cookies["LoginUser"].Value = Encrypts.GetEncryptString(userId.ToString());
                httpContext.Response.Cookies["LoginUser"].HttpOnly = true;
                httpContext.Response.Cookies["LoginUser"].Expires = DateTime.Now.AddDays(1);

                RedisHelper.KeyExpire(token, new TimeSpan(1, 0, 0));
            }

            if (PerimCode == null)
                return loginSession != null;
            if (loginSession.IsAdmin)
                return true;
            var perimList = loginSession.PerimissionList;
            if (perimList == null || perimList.Count <= 0)
                return false;
            var result = perimList.Any(t => PerimCode.Contains(t.PerimCode) && t.Value > 0);
            return result;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            filterContext.Result = new RedirectResult("/Login/Index");
        }
    }
}