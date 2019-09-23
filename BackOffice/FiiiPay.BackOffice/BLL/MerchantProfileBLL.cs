using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.Queue;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Component.Verification;
using FiiiPay.Framework;

namespace FiiiPay.BackOffice.BLL
{
    public class MerchantProfileBLL : BaseBLL
    {
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(BOErrorAttribute));
        public List<Entities.MerchantProfile> GetMerchantKYCProfilePageListL1(string keyword, int countryId, int? status, ref GridPager pager)
        {
            var profileSDK = new MerchantProfileAgent();
            int totalCount = 0;
            bool isDesc = (pager.OrderBy.ToLower() == "desc");
            List<Entities.MerchantProfile> data = profileSDK.GetMerchantVerifyListForL1(keyword, countryId, status, pager.SortColumn, isDesc, pager.Size, pager.Page, out totalCount);
            pager.TotalPage = (int)Math.Ceiling((double)totalCount / (double)pager.Size);
            pager.Count = totalCount;
            return data;
        }
        public List<Entities.MerchantProfile> GetMerchantProfilePageListL2(string keyword, int countryId, int? status, ref GridPager pager)
        {
            var profileSDK = new MerchantProfileAgent();
            int totalCount = 0;
            bool isDesc = (pager.OrderBy.ToLower() == "desc");
            List<Entities.MerchantProfile> data = profileSDK.GetMerchantVerifyListL2(keyword, countryId, status, pager.SortColumn, isDesc, pager.Size, pager.Page, out totalCount);
            pager.TotalPage = (int)Math.Ceiling((double)totalCount / (double)pager.Size);
            pager.Count = totalCount;
            return data;
        }



        public MerchantProfile GetMerchantProfile(Guid accountGid)
        {
            var profileSDK = new MerchantProfileAgent();
            var data = profileSDK.GetMerchantProfile(accountGid);
            return data;
        }


        public MerchantProfileSet GetMerchantProfileSet(Guid accountGid)
        {
            var profileSDK = new MerchantProfileAgent();
            var data = profileSDK.GetMerchantProfileSet(accountGid);
            return data;
        }

        public SaveResult SaveMerchantProfileVerifyL1(int AdminId, string AdminName, MerchantProfile profile)
        {
            var oldProfile = GetMerchantProfile(profile.MerchantId);
            var merchantAccount = FiiiPayDB.MerchantAccountDb.GetById(profile.MerchantId);
            merchantAccount.L1VerifyStatus = profile.L1VerifyStatus;
            if (oldProfile == null)
                return new SaveResult(false, "Data error");

            var profileSDK = new MerchantProfileAgent();
            var verifyStatus = profileSDK.UpdateL1VerifyStatus(oldProfile.MerchantId, profile.L1VerifyStatus, profile.L1Remark);
            if (verifyStatus)
            {
                FiiiPayDB.MerchantAccountDb.Update(merchantAccount);
                if ((profile.L1VerifyStatus == VerifyStatus.Certified || profile.L1VerifyStatus == VerifyStatus.Disapproval))
                {
                    var recordId = FiiiPayDB.VerifyRecordDb.InsertReturnIdentity(new VerifyRecords()
                    {
                        AccountId = profile.MerchantId,
                        Username = merchantAccount.Username,
                        Body = profile.L1Remark,
                        Type = profile.L1VerifyStatus == VerifyStatus.Certified ? VerifyRecordType.MerchantLv1Verified : VerifyRecordType.MerchantLv1Reject,
                        CreateTime = DateTime.UtcNow
                    });
                    try
                    {
                        if (profile.L1VerifyStatus == VerifyStatus.Certified)
                            RabbitMQSender.SendMessage("MerchantLv1Verified", recordId);
                        else if (profile.L1VerifyStatus == VerifyStatus.Disapproval)
                        {
                            RabbitMQSender.SendMessage("MerchantLv1VerifiedFailed", recordId);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                    }
                }
            }


            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = AdminId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MerchantProfileBLL).FullName + ".SaveMerchantProfileVerifyL1";
            actionLog.Username = AdminName;
            actionLog.LogContent = string.Format("verify merchant profile.MerchantId:{0},l1verifystatus:{1}", profile.MerchantId, profile.L1VerifyStatus.ToString());
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(verifyStatus);
        }

        public SaveResult SaveMerchantProfileVerifyL2(int AdminId, string AdminName, MerchantProfile profile)
        {
            var oldProfile = GetMerchantProfile(profile.MerchantId);
            var merchantAccount = FiiiPayDB.MerchantAccountDb.GetById(profile.MerchantId);
            merchantAccount.L2VerifyStatus = profile.L2VerifyStatus;
            if (oldProfile == null)
                return new SaveResult(false, "Data error");

            var profileSDK = new MerchantProfileAgent();
            var verifyStatus = profileSDK.UpdateL2VerifyStatus(oldProfile.MerchantId, profile.L2VerifyStatus, profile.L2Remark);
            if (verifyStatus)
            {
                FiiiPayDB.MerchantAccountDb.Update(merchantAccount);
                if ((profile.L2VerifyStatus == VerifyStatus.Certified || profile.L2VerifyStatus == VerifyStatus.Disapproval))
                {
                    var recordId = FiiiPayDB.VerifyRecordDb.InsertReturnIdentity(new VerifyRecords()
                    {
                        AccountId = profile.MerchantId,
                        Username = merchantAccount.Username,
                        Body = profile.L2Remark,
                        Type = profile.L2VerifyStatus == VerifyStatus.Certified ? VerifyRecordType.MerchantLv2Verified : VerifyRecordType.MerchantLv2Reject,
                        CreateTime = DateTime.UtcNow
                    });
                    if (profile.L2VerifyStatus == VerifyStatus.Certified)
                        RabbitMQSender.SendMessage("MerchantLv2Verified", recordId);
                    else if (profile.L2VerifyStatus == VerifyStatus.Disapproval)
                        RabbitMQSender.SendMessage("MerchantLv2VerifiedFailed", recordId);
                }
            }

            ActionLog actionLog = new ActionLog();
            actionLog.AccountId = AdminId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(MerchantProfileBLL).FullName + ".SaveMerchantProfileVerifyL2";
            actionLog.Username = AdminName;
            actionLog.LogContent = string.Format("verify merchant profile.MerchantId:{0},l2verifystatus:{1}", profile.MerchantId, profile.L2VerifyStatus.ToString());
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(verifyStatus);
        }        
    }
}