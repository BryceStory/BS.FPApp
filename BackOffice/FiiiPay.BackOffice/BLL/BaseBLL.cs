using FiiiPay.BackOffice.Common;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class BaseBLL
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
        public static string GetClientIPAddress()
        {
            string userIP;
            // HttpRequest Request = HttpContext.Current.Request;  
            HttpRequest Request = HttpContext.Current.Request; // ForumContext.Current.Context.Request;  
                                                               // 如果使用代理，获取真实IP  
            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                userIP = Request.ServerVariables["REMOTE_ADDR"];
            else
                userIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIP))
                userIP = Request.UserHostAddress;
            return userIP;
        }
    }
}