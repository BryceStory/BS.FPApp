using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Framework.Exceptions;
using System.Collections.Generic;
using FiiiPay.Framework.Queue;

namespace FiiiPay.BackOffice.BLL
{
    public class OrderBLL : BaseBLL
    {
        public List<OrderViewModel> GetOrderViewList(string orderNo,string possn, ref GridPager pager)
        {
            string sql = @" select a.Id,a.OrderNo,b.MerchantName,c.SN as PostSN,D.Cellphone,a.FiatAmount,a.Markup,a.ActualFiatAmount,a.CryptoId,a.ExchangeRate,
                            a.CryptoAmount,a.TransactionFee,a.ActualCryptoAmount,a.Status,a.Timestamp from Orders a 
                            left join MerchantAccounts b on a.MerchantAccountId = b.Id 
                            left join POS c on b.POSId = c.Id 
                            left join UserAccounts d on a.UserAccountId = d.Id 
                            where 1 =1 ";

            var paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(orderNo))
            {
                sql += " and a.OrderNo like @OrderNo ";
                paramList.Add(new SqlSugar.SugarParameter("@OrderNo", "%" + orderNo + "%"));
            }
            if (!string.IsNullOrEmpty(possn))
            {
                sql += " and c.SN like @POSSN ";
                paramList.Add(new SqlSugar.SugarParameter("@POSSN", "%" + possn + "%"));
            }
            var data = QueryPager.Query<OrderViewModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }

        public SaveResult Refund(string orderNo)
        {
            var sdk = new RefundAgent();
            try
            {
                var result = sdk.Refund(orderNo);
                if (result)
                {
                    //MSMQ.BackOfficeUserRefundOrder(orderNo, 0);
                    //MSMQ.BackOfficeMerchantArticleRefundOrder(orderNo, 0);
                    RabbitMQSender.SendMessage("FiiiPay_BackOfficeRefundOrder", orderNo);
                    RabbitMQSender.SendMessage("FiiiPos_BackOfficeRefundOrder", orderNo);
                }
                return new SaveResult(true);
            }
            catch (CommonException ex)
            {
                return new SaveResult(false, ex.Message);
            }
        }
    }
}