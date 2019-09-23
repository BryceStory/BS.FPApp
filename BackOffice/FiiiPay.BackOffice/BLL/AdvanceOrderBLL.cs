using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class AdvanceOrderBLL:BaseBLL
    {
        /// <summary>
        /// 查询POS机预订单
        /// </summary>
        /// <param name="OrderName">预定人</param>
        /// <param name="Paymentstatus">付款状态</param>
        /// <param name="Shippingstatus">发货状态</param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public List<AdvanceOrders> GetCodeQuerAll(string OrderName,string Paymentstatus,string Shippingstatus, ref GridPager pager)
        {
            string sql = "select * from AdvanceOrders where 1=1 ";
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(OrderName))
            {
                sql += " AND OrderName=@OrderName";
                paramList.Add(new SqlSugar.SugarParameter("@OrderName",OrderName));
            }
            if (!string.IsNullOrEmpty(Paymentstatus))
            {
                sql += " AND Paymentstatus=@Paymentstatus";
                paramList.Add(new SqlSugar.SugarParameter("@Paymentstatus",Paymentstatus));
            }
            if (!string.IsNullOrEmpty(Shippingstatus))
            {
                sql += " AND Shippingstatus=@Shippingstatus";
                paramList.Add(new SqlSugar.SugarParameter("@Shippingstatus",Shippingstatus));
            }
            var data = QueryPager.Query<AdvanceOrders>(BoDB.DB, sql, ref pager, paramList);
            return data;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public SaveResult Create(AdvanceOrders account)
        {
            return new SaveResult(BoDB.AdvanceOrderDb.Insert(account));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="account"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SaveResult UpdatePassword(AdvanceOrders account, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.Now;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update AdvanceOrders " + account.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(BoDB.AdvanceOrderDb.Update(account));
        }
    }
}