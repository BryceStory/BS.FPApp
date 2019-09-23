using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using System.Collections.Generic;

namespace FiiiPay.BackOffice.BLL
{
    public class WithdrawalsBLL : BaseBLL
    {
        public List<WithdrawalViewModel> GetUserWithdrawalViewList(string orderNo, string username, string address, string txid, string status, string cryptoId, ref GridPager pager)
        {
            string sql = @" select a.Id,a.OrderNo,c.Cellphone as Username,c.CountryId,a.RequestId,a.SelfPlatform,a.Address,a.Amount,b.CryptoId,d.Fee as WithdrawalFee,a.Status,a.Timestamp,a.TransactionId as TXID,a.Tag from [dbo].[UserWithdrawals] a
                            left join [dbo].[UserWallets] b on a.UserWalletId = b.Id
                            left join [dbo].[UserAccounts] c on b.UserAccountId = c.Id
                            left join [dbo].[UserWithdrawalFee] d on a.Id = d.WithdrawalId 
                            where 1 =1 ";

            var paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(orderNo))
            {
                sql += " and a.OrderNo like @OrderNo ";
                paramList.Add(new SqlSugar.SugarParameter("@OrderNo", "%" + orderNo + "%"));
            }
            if (!string.IsNullOrEmpty(username))
            {
                sql += " and c.Cellphone like @Cellphone ";
                paramList.Add(new SqlSugar.SugarParameter("@Cellphone", "%" + username + "%"));
            }
            if (!string.IsNullOrEmpty(address))
            {
                sql += " and a.Address like @Address ";
                paramList.Add(new SqlSugar.SugarParameter("@Address", "%" + address + "%"));
            }
            if (!string.IsNullOrEmpty(txid))
            {
                sql += " and a.TransactionId like @TXID ";
                paramList.Add(new SqlSugar.SugarParameter("@TXID", "%" + txid + "%"));
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " and a.Status=@Status ";
                paramList.Add(new SqlSugar.SugarParameter("@Status", status));
            }
            if (!string.IsNullOrEmpty(cryptoId))
            {
                sql += " and b.CryptoId=@CryptoId ";
                paramList.Add(new SqlSugar.SugarParameter("@CryptoId", cryptoId));
            }
            var data = QueryPager.Query<WithdrawalViewModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }

        public List<WithdrawalViewModel> GetMerchantWithdrawalViewList(string orderNo, string username, string address, string txid, string status, string cryptoId, ref GridPager pager)
        {
            string sql = @" select a.Id,a.OrderNo,c.Username,a.Address,b.CryptoId,c.CountryId,a.RequestId,a.Amount,d.Fee as WithdrawalFee,a.Status,a.Timestamp,a.TransactionId as TXID,a.Tag from [dbo].[MerchantWithdrawals] a
                            left join [dbo].[MerchantWallets] b on a.MerchantWalletId = b.Id
                            left join [dbo].[MerchantAccounts] c on b.MerchantAccountId = c.Id
                            left join [dbo].[MerchantWithdrawalFee] d on a.Id = d.WithdrawalId
                            where 1 =1 ";

            var paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(orderNo))
            {
                sql += " and a.OrderNo like @OrderNo ";
                paramList.Add(new SqlSugar.SugarParameter("@OrderNo", "%" + orderNo + "%"));
            }
            if (!string.IsNullOrEmpty(username))
            {
                sql += " and c.Username like @Username ";
                paramList.Add(new SqlSugar.SugarParameter("@Username", "%" + username + "%"));
            }
            if (!string.IsNullOrEmpty(address))
            {
                sql += " and a.Address like @Address ";
                paramList.Add(new SqlSugar.SugarParameter("@Address", "%" + address + "%"));
            }
            if (!string.IsNullOrEmpty(txid))
            {
                sql += " and a.TransactionId like @TXID ";
                paramList.Add(new SqlSugar.SugarParameter("@TXID", "%" + txid + "%"));
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " and a.Status=@Status ";
                paramList.Add(new SqlSugar.SugarParameter("@Status", status));
            }
            if (!string.IsNullOrEmpty(cryptoId))
            {
                sql += " and b.CryptoId=@CryptoId ";
                paramList.Add(new SqlSugar.SugarParameter("@CryptoId", cryptoId));
            }
            var data = QueryPager.Query<WithdrawalViewModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }
    }
}