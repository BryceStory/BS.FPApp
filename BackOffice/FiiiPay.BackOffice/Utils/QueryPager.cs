using FiiiPay.BackOffice.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    public class QueryPager
    {
        public static List<T> Query<T>(SqlSugarClient db,string sql, ref GridPager pager, List<SugarParameter> paraList = null) where T : class, new()
        {
            string entityName = typeof(T).Name;
            var ds = Query(db,sql, ref pager, paraList);
            return EntityHelper.EncapsulateObj<T>(ds);
        }

        public static DataSet Query(SqlSugarClient db, string sql, ref GridPager pager, List<SugarParameter> paraList = null)
        {
            long startIndex = (pager.Page - 1) * pager.Size + 1;
            string sql_statement = "WITH p_pager_sorted AS";
            sql_statement += string.Format("(SELECT ROW_NUMBER() OVER (ORDER BY {0} {1}) AS RowNumber, *, count(1) over() as p_pager_totalcount from (", pager.SortColumn, pager.OrderBy);
            sql_statement += sql;
            sql_statement += ") p_pager_pagertable) SELECT * FROM p_pager_sorted";
            sql_statement += string.Format(" WHERE RowNumber BETWEEN {0} and {1}", startIndex, startIndex + pager.Size - 1);

            var ds = db.Ado.GetDataSetAll(sql_statement, paraList);

            if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                pager.Count = Convert.ToInt32(ds.Tables[0].Rows[0]["p_pager_totalcount"]);
            else
                pager.Count = 0;
            pager.TotalPage = pager.Count == 0 ? 0 : ((int)Math.Ceiling((double)pager.Count / (double)pager.Size));
            return ds;
        }
    }
}