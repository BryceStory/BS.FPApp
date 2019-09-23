using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.BackOffice.ViewModels;
using FiiiPay.Data;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiiiPay.BackOffice.BLL
{
    public class POSBLL : BaseBLL
    {
        public List<POSViewModel> GetPOSInfoList(string merchantName, string username, string cellPhone, AccountStatus? status, string possn, int? countryId, ref GridPager pager)
        {
            string sql = @" select b.Id,b.Username,a.SN,b.Cellphone,b.MerchantName,b.CountryId,b.Email,
                             b.Status,b.IsAllowWithdrawal,b.IsAllowAcceptPayment 
                             from MerchantAccounts b
                             left join pos a on b.POSId = a.Id where 1=1 ";

            var paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(merchantName))
            {
                sql += " and b.MerchantName like @MerchantName ";
                paramList.Add(new SqlSugar.SugarParameter("@MerchantName", "%" + merchantName + "%"));
            }
            if (!string.IsNullOrEmpty(cellPhone))
            {
                sql += " and b.CellPhone like @CellPhone ";
                paramList.Add(new SqlSugar.SugarParameter("@CellPhone", "%" + cellPhone + "%"));
            }
            if (!string.IsNullOrEmpty(username))
            {
                sql += " and b.Username like @Username ";
                paramList.Add(new SqlSugar.SugarParameter("@Username", "%" + username + "%"));
            }
            if (!string.IsNullOrEmpty(possn))
            {
                sql += " and a.SN like @POSSN ";
                paramList.Add(new SqlSugar.SugarParameter("@POSSN", "%" + possn + "%"));
            }
            if (status.HasValue)
            {
                sql += "and b.Status=@Status";
                paramList.Add(new SqlSugar.SugarParameter("@Status", Convert.ToInt32(status)));
            }
            if (countryId.HasValue && countryId.Value > 0)
            {
                sql += " AND b.CountryId=@CountryId";
                paramList.Add(new SqlSugar.SugarParameter("@CountryId", countryId.Value));
            }
            var data = QueryPager.Query<POSViewModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }

        public List<POSViewModel> GetPOSInfoList(string userName, string cellPhone, string sn, ref GridPager pager)
        {
            string sql = @" select b.Id,b.Username,a.SN,b.Cellphone,b.MerchantName,b.Email,                             
                             b.Status,b.IsAllowWithdrawal,b.IsAllowAcceptPayment,b.CountryId
                             from MerchantAccounts b
                             left join pos a  on b.POSId = a.Id where 1=1 ";

            var paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(userName))
            {
                sql += " and b.UserName like @userName ";
                paramList.Add(new SqlSugar.SugarParameter("@userName", "%" + userName + "%"));
            }
            if (!string.IsNullOrEmpty(cellPhone))
            {
                sql += " and b.CellPhone like @cellPhone ";
                paramList.Add(new SqlSugar.SugarParameter("@cellPhone", "%" + cellPhone + "%"));
            }
            if (!string.IsNullOrEmpty(sn))
            {
                sql += " and a.SN like @sn ";
                paramList.Add(new SqlSugar.SugarParameter("@sn", "%" + sn + "%"));
            }
            var data = QueryPager.Query<POSViewModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }

        public POSViewModel GetPOSInfoById(Guid id)
        {
            string sql = @" select b.Id,b.Username,a.SN,b.Cellphone,b.MerchantName,b.Email,
                             (select top 1 mw.CryptoId from MerchantWallets mw where mw.MerchantAccountId = b.Id and mw.IsDefault = 1) as DefaultCryptoId,                             
                             b.Status,b.IsAllowWithdrawal,b.IsAllowAcceptPayment,b.Receivables_Tier,b.Markup,b.AuthSecretKey,b.ValidationFlag 
                             from MerchantAccounts b
                             left join pos a  on b.POSId = a.Id
                             where b.id = @id";

            return FiiiPayDB.DB.Ado.SqlQuery<POSViewModel>(sql, new { id = id }).First();
        }

        public SaveResult SaveEdit(POSViewModel model, int userId, string userName)
        {
            var oldMerchant = FiiiPayDB.MerchantAccountDb.GetById(model.Id);
            //var oldWallets = FiiiPayDB.DB.Queryable<MerchantWallets>().Where(t => t.MerchantAccountId == model.Id && t.Id == model.DefaultCryptoId).First();
            oldMerchant.IsAllowAcceptPayment = model.IsAllowAcceptPayment;
            oldMerchant.IsAllowWithdrawal = model.IsAllowWithdrawal;
            oldMerchant.Receivables_Tier = model.Receivables_Tier;
            oldMerchant.Markup = model.Markup;
            oldMerchant.Status = model.Status;
            //try
            //{
            //FiiiPayDB.DB.Ado.BeginTran();
            FiiiPayDB.MerchantAccountDb.Update(oldMerchant);
            //if (oldWallets != null)
            //{
            //    oldWallets.IsDefault = 1;
            //    FiiiPayDB.DB.Ado.ExecuteCommand("update MerchantWallets set IsDefault = 0 where MerchantAccountId = @MerchantAccountId;", new { MerchantAccountId = model.Id });
            //    FiiiPayDB.MerchantWalletDb.Update(oldWallets);
            //}
            //FiiiPayDB.DB.Ado.CommitTran();
            //}
            //catch (Exception ex)
            //{
            //    FiiiPayDB.DB.Ado.RollbackTran();
            //    throw ex;
            //}

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".SaveEdit";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Update Merchant,ID:{0},MerchantName:{1}", model.Id, model.MerchantName);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(true, "Save Success");
        }

        public SaveResult Create(POSs pos, int userId, string userName)
        {

            bool isExists = FiiiPayDB.DB.Queryable<POSs>().Where(t => t.Sn == pos.Sn).First() == null;
            if (pos != null && isExists)
            {
                pos.Timestamp = DateTime.UtcNow;
                pos.Status = false;
                var posId = FiiiPayDB.POSDb.InsertReturnIdentity(pos);
                bool saveSuccess = posId > 0;
                if (saveSuccess)
                {
                    ActionLog actionLog = new ActionLog();
                    actionLog.IPAddress = GetClientIPAddress();
                    actionLog.AccountId = userId;
                    actionLog.CreateTime = DateTime.UtcNow;
                    actionLog.ModuleCode = typeof(POSBLL).FullName + ".Create";
                    actionLog.Username = userName;
                    actionLog.LogContent = "Create POS " + posId;
                    new ActionLogBLL().Create(actionLog);
                }
                return new SaveResult(saveSuccess);
            }
            return new SaveResult(false);
        }

        public SaveResult GoogleUnbind(Guid id, int userId, string userName)
        {
            var oldMerchant = FiiiPayDB.MerchantAccountDb.GetById(id);
            oldMerchant.AuthSecretKey = "";
            oldMerchant.ValidationFlag = ValidationFlagComponent.ReduceValidationFlag(oldMerchant.ValidationFlag, ValidationFlag.GooogleAuthenticator);
            FiiiPayDB.MerchantAccountDb.Update(oldMerchant);

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".GoogleUnbind";
            actionLog.Username = userName;
            actionLog.LogContent = "Unbind " + id;
            new ActionLogBLL().Create(actionLog);


            return new SaveResult(true, "Save Success");
        }

        public SaveResult DeleteById(int id, int userId, string userName)
        {
            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".DeleteById";
            actionLog.Username = userName;
            actionLog.LogContent = "Delete POS " + id;
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.POSDb.DeleteById(id));
        }

        public List<POSs> Import(List<POSs> list, List<string> snList, int userId, string userName)
        {
            string where = string.Format("'{0}'", string.Join("','", snList));
            List<POSs> listRepeat = FiiiPayDB.POSDb.GetList().Where(t => snList.Contains(t.Sn)).ToList();

            if (listRepeat.Count > 0)
            {
                return listRepeat;
            }

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".Import";
            actionLog.Username = userName;
            actionLog.LogContent = "Import POS ";
            new ActionLogBLL().Create(actionLog);
            FiiiPayDB.POSDb.InsertRange(list.ToArray());
            return listRepeat;
        }

        public SaveResult BatchUpdate(List<long> ids, bool isEnable, int userId, string userName)
        {
            POSDAC dac = new POSDAC();


            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".BatchUpdate";
            actionLog.Username = userName;
            actionLog.LogContent = "BatchUpdate POS Ids:" + ids;
            new ActionLogBLL().Create(actionLog);
            return new SaveResult(dac.BatchUpdate(ids, isEnable));
        }


        public SaveResult MarkWhiteLabel(List<long> ids, string whiteLabel, string firstCrypto, int userId, string userName)
        {
            POSDAC dac = new POSDAC();

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".BatchMarkWhiteLabel";
            actionLog.Username = userName;
            actionLog.LogContent = "BatchUpdate POS Ids:" + ids;
            new ActionLogBLL().Create(actionLog);
            return new SaveResult(dac.BatchMarkWhiteLabel(ids, whiteLabel, firstCrypto));
        }
        public SaveResult UnMarkWhiteLabel(List<long> ids, int userId, string userName)
        {
            POSDAC dac = new POSDAC();

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(POSBLL).FullName + ".BatchUnMarkWhiteLabel";
            actionLog.Username = userName;
            actionLog.LogContent = "BatchUpdate POS Ids:" + ids;
            new ActionLogBLL().Create(actionLog);
            return new SaveResult(dac.BatchUnMarkWhiteLabel(ids));
        }
    }
}