using FiiiPay.BackOffice.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FiiiPay.BackOffice
{
    public static class GridExtends
    {
        public static object ToGridJson<T>(this SqlSugar.ISugarQueryable<T> radios, ref GridPager pager, Func<T, object> selector =null)
        {
            int n = 0;//总记录数
            if (n > 0)
            {
                List<T> oList= radios.OrderBy(pager.SortColumn + " " + pager.OrderBy).ToPageList(pager.Page, pager.Size, ref n);
                int t = (int)Math.Ceiling((double)n / (double)pager.Size);//总页数
                pager.TotalPage = t;
                pager.Count = n;
                if (selector == null)
                    return new { total = t, page = pager.Page, records = n, rows = oList };
                else
                    return new { total = t, page = pager.Page, records = n, rows = oList.Select(selector).ToList() };
            }
            else
                return new { total = 0, page = pager.Page, records = n };
        }

        /// <summary>
        /// 将已经分页的列表转成grid格式的Json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="radios"></param>
        /// <param name="pager"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static object ToGridJson<T>(this IList<T> radios, GridPager pager, Func<T, object> selector = null)
        {
            if (radios != null)
            {
                if (selector == null)
                    return new { total = pager.TotalPage, page = pager.Page, records = pager.Count, rows = radios.ToArray() };
                else
                    return new { total = pager.TotalPage, page = pager.Page, records = pager.Count, rows = radios.Select(selector).ToArray() };
            }
            else
                return null;
        }

        /// <summary>
        /// 将未分页的列表转成grid格式的Json数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="radios"></param>
        /// <param name="pager"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static object ToGridJson<T>(this IList<T> radios, ref GridPager pager, Func<T, object> selector = null, bool needOrder=true)
        {
            if (radios != null)
            {
                int n = radios.Count();//总记录数
                int t = (int)Math.Ceiling((double)n / (double)pager.Size);//总页数
                int s = n % pager.Size;
                int takecount = pager.Size;
                if (pager.Page > t)
                    pager.Page = t;
                if (pager.Page == t && s > 0)
                    takecount = s;
                IEnumerable<T> olist;
                if(needOrder)
                    olist = radios.OrderBy(pager.SortColumn, pager.OrderBy).Skip((pager.Page - 1) * pager.Size).Take(takecount);
                else
                    olist = radios.Skip((pager.Page - 1) * pager.Size).Take(takecount);

                if (selector==null)
                    return new { total = t, page = pager.Page, records = n, rows = olist.ToArray() };
                else
                    return new { total = t, page = pager.Page, records = n, rows = olist.Select(selector).ToArray() };
            }
            else
                return null;
        }
    }
}