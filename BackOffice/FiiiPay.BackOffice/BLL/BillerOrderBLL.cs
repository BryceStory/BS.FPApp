using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Entities;
using FiiiPay.Framework.Queue;
using SqlSugar;
using System;

namespace FiiiPay.BackOffice.BLL
{
    public class BillerOrderBLL : BaseBLL
    {
        public SaveResult Update(BillerOrders order, int userId, string userName)
        {
            BillerOrders oldOrders = FiiiPayDB.BillerOrderDb.GetById(order.Id);
            oldOrders.Status = order.Status;
            oldOrders.Remark = order.Remark;
            oldOrders.FinishTime = DateTime.UtcNow;
            var result = FiiiPayDB.BillerOrderDb.Update(oldOrders);
            if (result && order.Status == BillerOrderStatus.Fail)
                result = new RefundAgent().BillerRefund(oldOrders.OrderNo);
            FiiiPayDB.DB.Updateable<UserTransactions>().SetColumns(t => new UserTransactions() { Status = (byte)order.Status })
                .Where(w => w.Type == UserTransactionType.BillOrder && w.DetailId == oldOrders.Id.ToString() && w.AccountId == oldOrders.AccountId);
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(AccountBLL).FullName + ".Update";
            actionLog.Username = userName;
            actionLog.LogContent = "Update BillerOrder " + order.Id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);
            if (order.Status != BillerOrderStatus.Pending)
                RabbitMQSender.SendMessage("Biller", order.Id);

            return new SaveResult(result);
        }
    }
}