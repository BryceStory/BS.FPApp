using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class VerifyRecordBLL : BaseBLL
    {
        public List<VerifyRecordViewModel> GetVerifyCount(string verifyAccount, string type, DateTime? startDate, DateTime? endDate, ref GridPager pager)
        {
            string sql = @"select AccountId,Username as VerifyAccount,count(*) as [VerifyCount] from ActionLog where 1=1 ";
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(verifyAccount))
            {
                sql += " and Username like @VerifyAccount ";
                paramList.Add(new SqlSugar.SugarParameter("@VerifyAccount", "%" + verifyAccount + "%"));
            }
            if (!string.IsNullOrEmpty(type))
            {
                sql += " and ModuleCode like @Type ";
                paramList.Add(new SqlSugar.SugarParameter("@Type", "%" + type + "%"));
            }
            else
            {
                sql += " and ModuleCode like @Type ";
                paramList.Add(new SqlSugar.SugarParameter("@Type", "%Verify%"));
            }
            if (startDate.HasValue)
            {
                sql += " and CreateTime >= @StartDate ";
                paramList.Add(new SqlSugar.SugarParameter("@StartDate", startDate.Value));
            }
            if (endDate.HasValue)
            {
                sql += " and CreateTime <= @EndDate ";
                paramList.Add(new SqlSugar.SugarParameter("@EndDate", endDate.Value));
            }

            sql += " group by AccountId,Username ";
            var data = QueryPager.Query<VerifyRecordViewModel>(BoDB.DB, sql, ref pager, paramList);
            return data;
        }

        public List<VerifyRecordViewModel> GetVerifyCount(string verifyAccount, string type)
        {
            string sql = @"select Username as VerifyAccount,count(*) as [VerifyCount] from ActionLog where 1=1 ";
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(verifyAccount))
            {
                sql += " and Username like @VerifyAccount ";
                paramList.Add(new SqlSugar.SugarParameter("@VerifyAccount", "%" + verifyAccount + "%"));
            }

            sql += " and ModuleCode like @Type ";
            paramList.Add(new SqlSugar.SugarParameter("@Type", "%" + type + "%"));

            sql += " and CreateTime >= @StartDate ";
            paramList.Add(new SqlSugar.SugarParameter("@StartDate", DateTime.Today));

            sql += " and CreateTime <= @EndDate ";
            paramList.Add(new SqlSugar.SugarParameter("@EndDate", DateTime.Today.AddDays(1)));

            sql += " group by AccountId,Username ";
            var data = BoDB.DB.Ado.SqlQuery<VerifyRecordViewModel>(sql, paramList).ToList();
            return data;
        }
    }
}