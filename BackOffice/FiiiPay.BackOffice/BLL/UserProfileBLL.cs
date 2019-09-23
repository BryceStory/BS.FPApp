using FiiiPay.BackOffice.Common;
using FiiiPay.BackOffice.Models;
using FiiiPay.Entities;
using System;
using System.Collections.Generic;
using FiiiPay.Data.Agents.APP;
using FiiiPay.Entities.EntitySet;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.Data;
using FiiiPay.Foundation.Data;
using FiiiPay.Business;
using FiiiPay.Entities.Enums;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Queue;

namespace FiiiPay.BackOffice.BLL
{
    public class UserProfileBLL : BaseBLL
    {
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(BOErrorAttribute));
        public List<Entities.UserProfile> GetUserProfilePageList(string account, int countryId, int? status, ref GridPager pager)
        {
            var profileSDK = new UserProfileAgent();
            int totalCount = 0;
            bool isDesc = (pager.OrderBy.ToLower() == "desc");
            var data = profileSDK.GetUserProfileListForL1(account, countryId, pager.SortColumn, isDesc, status, pager.Size, pager.Page, out totalCount);
            pager.TotalPage = (int)Math.Ceiling((double)totalCount / (double)pager.Size);
            pager.Count = totalCount;
            return data;
        }

        public List<UserAccountStatus> GetUserAccountStatusList(string cellphone, int country, int? status, ref GridPager pager)
        {
            var profileSDK = new UserProfileAgent();
            int totalCount = 0;
            bool isDesc = (pager.OrderBy.ToLower() == "desc");
            var data = profileSDK.GetUserAccountStatusList(cellphone, country, status, pager.Size, pager.Page, out totalCount);
            pager.TotalPage = (int)Math.Ceiling((double)totalCount / (double)pager.Size);
            pager.Count = totalCount;
            return data;
        }

        public UserProfileSet GetUserProfileSet(Guid accountGid)
        {
            var profileSDK = new UserProfileAgent();
            var data = profileSDK.GetUserProfileSet(accountGid);
            return data;
        }

        public UserProfile GetUserProfile(Guid accountGid)
        {
            var profileSDK = new UserProfileAgent();
            var data = profileSDK.GetUserProfile(accountGid);
            return data;
        }

        public SaveResult SaveProfileVerify(int AdminId, string AdminName, UserProfile profile)
        {
            var oldProfile = GetUserProfile(profile.UserAccountId.Value);
            var userAccount = FiiiPayDB.UserAccountDb.GetById(profile.UserAccountId);
            userAccount.L1VerifyStatus = profile.L1VerifyStatus.Value;
            if (oldProfile == null)
                return new SaveResult(false, "Data error");

            var profileSDK = new UserProfileAgent();
            int count = profileSDK.GetCountByIdentityDocNo(profile.UserAccountId.Value, oldProfile.IdentityDocNo);

            if (profile.L1VerifyStatus == VerifyStatus.Certified && count > 6)
            {
                return new SaveResult(false, "This identty document has been used for 7 accounts, cannot be verified ");
            }
            long profitId = 0;//被邀请人奖励ID
            long exProfitId = 0;//邀请人额外奖励ID

            bool result = profileSDK.UpdateL1Status(profile.UserAccountId.Value, profile.L1VerifyStatus.Value, profile.L1Remark);

            if (result)
            {
                FiiiPayDB.UserAccountDb.Update(userAccount);
            }
            else
            {
                profileSDK.UpdateL1Status(profile.UserAccountId.Value, oldProfile.L1VerifyStatus.Value, oldProfile.L1Remark);
            }

            if (result && profile.L1VerifyStatus == VerifyStatus.Certified)
            {
                var invite = FiiiPayDB.DB.Queryable<InviteRecords>().Where(t => t.AccountId == profile.UserAccountId.Value).First();
                if (invite != null)
                {
                    var inviteProfit = FiiiPayDB.DB.Queryable<ProfitDetails>().Where(t => t.AccountId == invite.InviterAccountId && t.InvitationId == invite.Id && t.Type == ProfitType.InvitePiiiPay).First();
                    var rewardProfit = FiiiPayDB.DB.Queryable<ProfitDetails>().Where(t => t.AccountId == invite.InviterAccountId && t.Status == InviteStatusType.IssuedFrozen && t.Type == ProfitType.Reward).OrderBy(t => t.Timestamp).First();

                    profitId = inviteProfit.Id;
                    var uwComponent = new UserWalletBLL();
                    var uwsDAC = new UserWalletStatementDAC();
                    var uwDAC = new UserWalletDAC();
                    var pfDAC = new ProfitDetailDAC();
                    var utDAC = new UserTransactionDAC();

                    int invitedCount = pfDAC.GetInvitedAndActiveCount(invite.InviterAccountId);

                    var cryptoId = new CryptocurrencyDAC().GetByCode("FIII").Id;
                    var inviteWallet = uwComponent.GetUserWallet(invite.InviterAccountId, cryptoId);
                    if (inviteWallet == null)
                        inviteWallet = uwComponent.GenerateWallet(invite.InviterAccountId, cryptoId);

                    var inviteMoney = inviteProfit.CryptoAmount;

                    var adoResult = FiiiPayDB.DB.Ado.UseTran(() =>
                    {
                        try
                        {
                            //解冻奖励
                            uwDAC.Unfreeze(inviteWallet.Id, inviteMoney);
                            //插入奖励流水
                            uwsDAC.Insert(new UserWalletStatement
                            {
                                WalletId = inviteWallet.Id,
                                Action = UserWalletStatementAction.Invite,
                                Amount = inviteMoney,
                                Balance = inviteWallet.Balance + inviteMoney,
                                Timestamp = DateTime.UtcNow
                            });
                            //修改奖励状态为已激活
                            pfDAC.UpdateStatus(inviteProfit.Id, InviteStatusType.IssuedActive);
                            utDAC.UpdateStatus(UserTransactionType.Profit, inviteProfit.Id.ToString(), invite.InviterAccountId, (byte)InviteStatusType.IssuedActive);

                            // 每当满50人时则可以奖励 采用了插入操作 所以只要满足为49个就可以了
                            if ((invitedCount + 1) % 50 == 0)
                            {
                                exProfitId = rewardProfit.Id;
                                var rewardMoney = rewardProfit.CryptoAmount;
                                //解冻满50人的额外奖励
                                uwDAC.Unfreeze(inviteWallet.Id, rewardMoney);
                                //插入满50人的额外奖励流水
                                uwsDAC.Insert(new UserWalletStatement
                                {
                                    WalletId = inviteWallet.Id,
                                    Action = UserWalletStatementAction.Reward,
                                    Amount = rewardMoney,
                                    Balance = inviteWallet.Balance + rewardMoney,
                                    Timestamp = DateTime.UtcNow
                                });
                                //修改奖励状态为已激活
                                pfDAC.UpdateStatus(rewardProfit.Id, InviteStatusType.IssuedActive);
                                utDAC.UpdateStatus(UserTransactionType.Profit, rewardProfit.Id.ToString(), invite.InviterAccountId, (byte)InviteStatusType.IssuedActive);
                            }
                        }
                        catch (Exception e)
                        {
                            log.Info(profile.UserAccountId + "    --------- " + e.ToString());
                        }
                    });
                    result = adoResult.Data;
                }
            }

            if (result && (profile.L1VerifyStatus == VerifyStatus.Certified || profile.L1VerifyStatus == VerifyStatus.Disapproval))
            {
                var recordId = FiiiPayDB.VerifyRecordDb.InsertReturnIdentity(new VerifyRecords()
                {
                    AccountId = profile.UserAccountId.Value,
                    Username = userAccount.Cellphone,
                    Body = profile.L1Remark,
                    Type = profile.L1VerifyStatus == VerifyStatus.Certified ? VerifyRecordType.UserLv1Verified : VerifyRecordType.UserLv1Reject,
                    CreateTime = DateTime.UtcNow
                });
                if (profile.L1VerifyStatus == VerifyStatus.Certified)
                    RabbitMQSender.SendMessage("UserKYC_LV1_VERIFIED", recordId);
                else if (profile.L1VerifyStatus == VerifyStatus.Disapproval)
                    RabbitMQSender.SendMessage("UserKYC_LV1_REJECT", recordId);
            }

            if (result && profitId > 0)
            {
                RabbitMQSender.SendMessage("UserInviteSuccessed", profitId);
            }
            if (result && exProfitId > 0)
            {
                RabbitMQSender.SendMessage("UserInviteSuccessed", profitId);
            }
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = AdminId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserProfileBLL).FullName + ".SaveProfileVerify";
            actionLog.Username = AdminName;
            actionLog.LogContent = string.Format("verify user profile.accountId:{0},verifystatus:{1}", profile.UserAccountId, profile.L1VerifyStatus.ToString());
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(result);
        }

        public List<Entities.UserProfile> GetUserResidencePageList(string account, int countryId, int? status, ref GridPager pager)
        {
            var profileSDK = new UserProfileAgent();
            int totalCount = 0;
            bool isDesc = (pager.OrderBy.ToLower() == "desc");
            var data = profileSDK.GetUserProfileListForL2(account, countryId, pager.SortColumn, isDesc, status, pager.Size, pager.Page, out totalCount);
            pager.TotalPage = (int)Math.Ceiling((double)totalCount / (double)pager.Size);
            pager.Count = totalCount;
            return data;
        }

        public SaveResult SaveResidenceVerify(int AdminId, string AdminName, UserProfile profile)
        {
            var oldProfile = GetUserProfile(profile.UserAccountId.Value);
            var userAccount = FiiiPayDB.UserAccountDb.GetById(profile.UserAccountId);
            userAccount.L2VerifyStatus = profile.L2VerifyStatus.Value;
            if (oldProfile == null)
                return new SaveResult(false, "Data error");

            var profileSDK = new UserProfileAgent();
            var status = profileSDK.UpdateL2Status(profile.UserAccountId.Value, profile.L2VerifyStatus.Value, profile.L2Remark);
            if (status)
            {
                FiiiPayDB.UserAccountDb.Update(userAccount);
                if ((profile.L2VerifyStatus == VerifyStatus.Certified || profile.L2VerifyStatus == VerifyStatus.Disapproval))
                {
                    var recordId = FiiiPayDB.VerifyRecordDb.InsertReturnIdentity(new VerifyRecords()
                    {
                        AccountId = profile.UserAccountId.Value,
                        Username = userAccount.Cellphone,
                        Body = profile.L2Remark,
                        Type = profile.L2VerifyStatus == VerifyStatus.Certified ? VerifyRecordType.UserLv2Verified : VerifyRecordType.UserLv2Reject,
                        CreateTime = DateTime.UtcNow
                    });
                    if (profile.L2VerifyStatus == VerifyStatus.Certified)
                        RabbitMQSender.SendMessage("UserKYC_LV2_VERIFIED", recordId);
                    else if (profile.L2VerifyStatus == VerifyStatus.Disapproval)
                        RabbitMQSender.SendMessage("UserKYC_LV2_REJECT", recordId);
                }
            }
            ActionLog actionLog = new ActionLog();
            actionLog.IPAddress = GetClientIPAddress();
            actionLog.AccountId = AdminId;
            actionLog.CreateTime = DateTime.UtcNow;
            actionLog.ModuleCode = typeof(UserProfileBLL).FullName + ".SaveResidenceVerify";
            actionLog.Username = AdminName;
            actionLog.LogContent = string.Format("verify user residence.accountId:{0},verifystatus:{1}", profile.UserAccountId, profile.L1VerifyStatus.ToString());
            new ActionLogBLL().Create(actionLog);

            return new SaveResult(status);
        }
    }
}