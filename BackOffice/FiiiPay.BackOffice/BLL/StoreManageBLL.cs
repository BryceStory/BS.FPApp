using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.BackOffice.Utils;
using FiiiPay.Entities;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.BLL
{
    public class StoreManageBLL : BaseBLL
    {
        public List<ViewModels.FiiiPosMerchantInfoListModel> GetFiiiPosMerchantPager(string merchantName, string userName, string cellPhone, int? countryId, int? verifyStatus, ref GridPager pager)
        {
            string sql = "SELECT a.Id,b.Username,b.Cellphone,a.MerchantName,a.CountryId,a.Status,a.VerifyStatus FROM dbo.MerchantInformations a LEFT JOIN dbo.MerchantAccounts b on a.MerchantAccountId=b.Id WHERE AccountType=" + (int)AccountType.Merchant;
            List<SqlSugar.SugarParameter> paramList = new List<SqlSugar.SugarParameter>();
            if (!string.IsNullOrEmpty(userName))
            {
                sql += " AND b.[Username]=@Username";
                paramList.Add(new SqlSugar.SugarParameter("@Username", userName));
            }
            if (!string.IsNullOrEmpty(merchantName))
            {
                sql += " AND a.[MerchantName] LIKE @MerchantName";
                paramList.Add(new SqlSugar.SugarParameter("@MerchantName", "%" + merchantName + "%"));
            }
            if (!string.IsNullOrEmpty(cellPhone))
            {
                sql += " AND b.[Cellphone]=@Cellphone";
                paramList.Add(new SqlSugar.SugarParameter("@Cellphone", cellPhone));
            }
            if (countryId.HasValue)
            {
                sql += " AND a.[CountryId]=@CountryId";
                paramList.Add(new SqlSugar.SugarParameter("@CountryId", countryId.Value));
            }
            if (verifyStatus.HasValue)
            {
                sql += " AND a.[VerifyStatus]=@VerifyStatus";
                paramList.Add(new SqlSugar.SugarParameter("@VerifyStatus", verifyStatus.Value));
            }
            var data = QueryPager.Query<ViewModels.FiiiPosMerchantInfoListModel>(FiiiPayDB.DB, sql, ref pager, paramList);
            return data;
        }
        public SaveResult SetUser(MerchantInformations model, int userId, string userName)
        {
            var info = FiiiPayDB.MerchantInformationDb.GetById(model.Id);
            info.Status = model.Status;

            // Create ActionLog
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = userId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(StoreManageBLL).FullName + ".SetUser";
            actionLog.Username = userName;
            actionLog.LogContent = string.Format("Set MerchantInformations Status,Id:{0}", model.Id);
            ActionLogBLL ab = new ActionLogBLL();
            ab.Create(actionLog);

            return new SaveResult(FiiiPayDB.MerchantInformationDb.Update(info));
        }
        public SaveResult SaveProfileVerify(MerchantInformations info, int AdminId, string AdminName)
        {
            var oldInfo = FiiiPayDB.MerchantInformationDb.GetById(info.Id);
            //oldInfo.VerifyStatus = info.VerifyStatus;
            //oldInfo.Remark = info.Remark;
            //oldInfo.VerifyDate = DateTime.Now;

            var result = FiiiPayDB.MerchantInformationDb.Update(c => new MerchantInformations
            {
                VerifyStatus = info.VerifyStatus,
                Remark = info.Remark,
                VerifyDate = DateTime.UtcNow
            }, w => w.Id == info.Id);

            if (result && (info.VerifyStatus == VerifyStatus.Certified || info.VerifyStatus == VerifyStatus.Disapproval))
            {
                var recordId = FiiiPayDB.VerifyRecordDb.InsertReturnIdentity(new VerifyRecords()
                {
                    AccountId = info.MerchantAccountId,
                    Username = "",
                    Body = info.Remark,
                    Type = info.VerifyStatus == VerifyStatus.Certified ? VerifyRecordType.StoreVerified : VerifyRecordType.StoreReject,
                    CreateTime = DateTime.UtcNow
                });
                if (info.VerifyStatus == VerifyStatus.Certified)
                    RabbitMQSender.SendMessage("STORE_VERIFIED", oldInfo.Id);
                else if (info.VerifyStatus == VerifyStatus.Disapproval)
                    RabbitMQSender.SendMessage("STORE_REJECT", oldInfo.Id);
            }

            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = AdminId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(StoreManageBLL).FullName + ".SaveProfileVerify";
            actionLog.Username = AdminName;
            actionLog.LogContent = string.Format("verify store merchantId:{0},verifystatus:{1}", info.MerchantAccountId, info.VerifyStatus.ToString());
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(result);
        }
    }
}