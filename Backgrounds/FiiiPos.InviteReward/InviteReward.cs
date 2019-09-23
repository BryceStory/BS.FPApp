using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.Exceptions;
using log4net;
using Newtonsoft.Json;
using System;
using System.Transactions;
using FiiiPay.Foundation.Data;

namespace FiiiPos.InviteReward
{
    public class InviteReward
    {
        private static int FirstLevelCount = 3;
        private static int SecondLevelCount = 10;
        private static int ThirdLevelCount = 30;
        private static decimal FirstLevel = 0.04m;
        private static decimal SecondLevel = 0.06m;
        private static decimal ThirdLevel = 0.08m;
        //private static string CompanyFiiiPayAccountId = ConfigurationManager.AppSettings["CompanyFiiiPayAccountId"];

        private MerchantAccountDAC maDAC = new MerchantAccountDAC();
        private UserWalletDAC uwDAC = new UserWalletDAC();
        private ProfitDetailDAC pfDAC = new ProfitDetailDAC();
        private UserWalletStatementDAC uwsDAC = new UserWalletStatementDAC();
        private RewardDistributeRecordDAC rdrDAC = new RewardDistributeRecordDAC();
        private UserTransactionDAC transDAC = new UserTransactionDAC();

        public void DistributeRewards(FiiiPayRewardMessage message, int fiiicoinId)
        {
            MerchantAccountDAC maDAC = new MerchantAccountDAC();
            var merchant = maDAC.GetByUsername(message.Account);
            if (merchant == null)
            {
                throw new CommonException(10000, $"无效的商家名:{message.Account}");
            }

            InviteRecord invitor = GetInvitor(merchant.Id);
            if (invitor == null)
            {
                throw new CommonException(10000, "没有邀请人");
            }

            var invitedList = new InviteRecordDAC().GetFiiiPosRecordsByInvitorId(invitor.InviterAccountId, InviteType.Fiiipos);
            var rewardRate = GetRewardPercentage(invitedList == null ? 0 : invitedList.Count);
            if (rewardRate == 0)
            {
                throw new CommonException(10000, "没有达到奖励条件");
            }

            decimal t = (decimal)Math.Pow(10, -8);
            long nTotalReward = (long)Math.Floor(message.Reward * rewardRate);
            if (nTotalReward == 0)
            {
                throw new CommonException(10000, "奖励金额为0");
            }
            decimal rewardAmount = nTotalReward * t;
            DateTime dtNow = DateTime.UtcNow;
            if (message.CurrentDate > 0)
                dtNow = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(message.CurrentDate);
            var distributeRecord = new RewardDistributeRecords
            {
                UserAccountId = invitor.InviterAccountId,
                MerchantAccountId = merchant.Id,
                SN = message.SN,
                OriginalReward = message.Reward,
                Percentage = rewardRate,
                ActualReward = rewardAmount,
                Timestamp = dtNow
            };

            long profitId;
            decimal oldBalance = 0;
            var invitorWallet = uwDAC.GetByAccountId(invitor.InviterAccountId, fiiicoinId);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                if (invitorWallet == null)
                {
                    invitorWallet = CreateWallet(invitor.InviterAccountId, fiiicoinId, rewardAmount);
                }
                else
                {
                    oldBalance = invitorWallet.Balance;
                    uwDAC.Increase(invitorWallet.Id, rewardAmount);
                }

                var profitDetail = new ProfitDetail
                {
                    InvitationId = invitor.Id,
                    CryptoAmount = rewardAmount,
                    AccountId = invitor.InviterAccountId,
                    Status = InviteStatusType.IssuedActive,
                    Type = ProfitType.InvitePiiiPos,
                    OrderNo = CreateOrderno(),
                    Timestamp = dtNow,
                    CryptoId = invitorWallet.CryptoId,
                    CryptoCode = "FIII"
                };
                profitId = pfDAC.Insert(profitDetail);

                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = invitorWallet.Id,
                    Action = UserWalletStatementAction.Reward,
                    Amount = rewardAmount,
                    Balance = oldBalance + rewardAmount,
                    FrozenAmount = 0,
                    FrozenBalance = invitorWallet.FrozenBalance,
                    Timestamp = dtNow
                });

                transDAC.Insert(new UserTransaction
                {
                    AccountId = invitor.InviterAccountId,
                    Amount = rewardAmount,
                    CryptoCode = "FIII",
                    CryptoId = fiiicoinId,
                    DetailId = profitId.ToString(),
                    Id = Guid.NewGuid(),
                    MerchantName = string.Empty,
                    OrderNo = profitDetail.OrderNo,
                    Status = 2,
                    Timestamp = dtNow,
                    Type = UserTransactionType.Profit
                });

                distributeRecord.ProfitId = profitId;
                rdrDAC.Insert(distributeRecord);

                scope.Complete();
            }

            if (profitId > 0)
            {
                try
                {
                    MessagePushService.PubUserInviteSuccessed(profitId, 0);
                }
                catch (Exception ex)
                {
                    throw new CommonException(10000, ex.Message);
                }
            }
        }

        /// <summary>
        /// 创建用户钱包
        /// </summary>
        /// <param name="userAccountId">The user account identifier.</param>
        /// <param name="cryptoId">The crypto identifier.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        private UserWallet CreateWallet(Guid userAccountId, int cryptoId, decimal amount)
        {
            var invitorWallet = new UserWallet
            {
                UserAccountId = userAccountId,
                CryptoId = cryptoId,
                CryptoCode = "FIII",
                Balance = amount,
                FrozenBalance = 0,
                Address = null,
                Tag = null,
                HomePageRank = 0,
                PayRank = 0,
                ShowInHomePage = true
            };
            invitorWallet.Id = uwDAC.Insert(invitorWallet);

            return invitorWallet;
        }

        /// <summary>
        /// 获取邀请人ID
        /// </summary>
        /// <param name="merchantAccountId"></param>
        /// <returns></returns>
        private InviteRecord GetInvitor(Guid merchantAccountId)
        {
            return new InviteRecordDAC().GetDetailByAccountId(merchantAccountId);
        }

        /// <summary>
        /// 获取奖励百分比
        /// </summary>
        /// <param name="generateTime"></param>
        /// <param name="achieveLeveTimes"></param>
        /// <returns></returns>
        private decimal GetRewardPercentage(int invitorCount)
        {
            if (invitorCount >= ThirdLevelCount)
                return ThirdLevel;
            if (invitorCount >= SecondLevelCount)
                return SecondLevel;
            if (invitorCount >= FirstLevelCount)
                return FirstLevel;
            return 0;
        }
        private string CreateOrderno()
        {
            return (GetUnixTicks(DateTime.UtcNow)) + new Random().Next(0, 100).ToString().PadLeft(2, '0');
        }

        private long GetUnixTicks(DateTime dt)
        {
            return ((dt.ToUniversalTime().Ticks - 621355968000000000) / 10000);
        }
    }
}
