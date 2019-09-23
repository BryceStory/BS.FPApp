using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    [BOAccess("ChartMenu")]
    public class ChartController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMemberCount(int timeType)
        {
            List<MemberCountChart> chartList = new ChartBLL().GetMemberCount(timeType);
            int userTotal = FiiiPayDB.DB.Queryable<UserAccounts>().Count();
            int merchantTotal = FiiiPayDB.DB.Queryable<MerchantAccounts>().Count();

            return Json(new { Data=chartList, UserTotal= userTotal, MerchantTotal= merchantTotal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConsumeCount(int timeType)
        {
            List<ConsumeCountChart> chartList = new ChartBLL().GetConsumeCount(timeType);
            int totalCount = FiiiPayDB.DB.Queryable<Orders>().Count();

            return Json(new { Data = chartList, TotalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }
    }
}