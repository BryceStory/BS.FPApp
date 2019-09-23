using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Common.BOErrorAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
