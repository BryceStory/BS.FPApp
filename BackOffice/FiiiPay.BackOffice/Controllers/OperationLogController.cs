using FiiiPay.BackOffice.BLL;
using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FiiiPay.BackOffice.Controllers
{
    public class OperationLogController : BaseController
    {
        // GET: OperationLog
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult LoadData(GridPager pager, string userName, DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
            List<ActionLog> list = BoDB.DB.Queryable<ActionLog>()
                .WhereIF(!string.IsNullOrEmpty(userName), r => r.Username.Contains(userName))
                .WhereIF(startDate.HasValue, r => r.CreateTime > startDate.Value)
                .WhereIF(endDate.HasValue, r => r.CreateTime < endDate.Value)
                .ToList();
            var obj = list.ToGridJson(ref pager, r =>
                new
                {
                    id = r.Id,
                    cell = new string[]{
                        r.Id.ToString(),
                        r.CreateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        r.Username,
                        r.LogContent,
                        r.IPAddress
                    }
                });
            //total 总页数, page 当前页, records 总记录数
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}