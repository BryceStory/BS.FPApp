using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Exceptions;
using System;
using System.Transactions;

namespace FiiiPos.InviteReward.ComplementData
{
    public class InviteReward
    {
        private static int FirstLevelCount = 3;
        private static int SecondLevelCount = 10;
        private static int ThirdLevelCount = 30;
        private static decimal FirstLevel = 0.04m;
        private static decimal SecondLevel = 0.06m;
        private static decimal ThirdLevel = 0.08m;
        private static int fiiicoinId = 0;

        private MerchantAccountDAC maDAC = new MerchantAccountDAC();
        private UserWalletDAC uwDAC = new UserWalletDAC();
        private ProfitDetailDAC pfDAC = new ProfitDetailDAC();
        private UserWalletStatementDAC uwsDAC = new UserWalletStatementDAC();
        private RewardDistributeRecordDAC rdrDAC = new RewardDistributeRecordDAC();

        public void DistributeRewards(FiiiPayRewardMessage message,DateTime dt)
        {
            if (fiiicoinId == 0)
            {
                var fiiicoin = new CryptocurrencyDAC().GetByCode("FIII");
                fiiicoinId = fiiicoin.Id;
            }
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
            long nTotalReward = (long)Math.Floor(message.ActualReward * rewardRate);
            if (nTotalReward == 0)
            {
                throw new CommonException(10000, "奖励金额为0");
            }
            decimal rewardAmount = nTotalReward * t;
            var distributeRecord = new RewardDistributeRecords
            {
                UserAccountId = invitor.InviterAccountId,
                MerchantAccountId = merchant.Id,
                SN = message.SN,
                OriginalReward = message.ActualReward,
                Percentage = rewardRate,
                ActualReward = rewardAmount,
                Timestamp = dt
            };

            long profitId = 0;
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
                profitId = pfDAC.Insert(new ProfitDetail()
                {
                    InvitationId = invitor.Id,
                    CryptoAmount = rewardAmount,
                    AccountId = invitor.InviterAccountId,
                    Status = InviteStatusType.IssuedActive,
                    Type = ProfitType.InvitePiiiPos,
                    OrderNo = CreateOrderno(),
                    Timestamp = dt,
                    CryptoId = invitorWallet.CryptoId
                });
                uwsDAC.Insert(new UserWalletStatement
                {
                    WalletId = invitorWallet.Id,
                    Action = UserWalletStatementAction.Reward,
                    Amount = rewardAmount,
                    Balance = oldBalance + rewardAmount,
                    FrozenAmount = 0,
                    FrozenBalance = invitorWallet.FrozenBalance,
                    Timestamp = dt,
                    Remark = "System"
                });

                distributeRecord.ProfitId = profitId;
                rdrDAC.Insert(distributeRecord);

                new TempDataDAC().MessageComplated(message.Id);

                scope.Complete();
            }
        }

        private UserWallet CreateWallet(Guid userAccountId, int cryptoId, decimal amount)
        {
            UserWallet invitorWallet = new UserWallet
            {
                UserAccountId = userAccountId,
                CryptoId = cryptoId,
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

        private InviteRecord GetInvitor(Guid merchantAccountId)
        {
            return new InviteRecordDAC().GetDetailByAccountId(merchantAccountId);
        }

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
            return GetUnixTicks(DateTime.UtcNow) + new Random().Next(0, 100).ToString().PadLeft(2, '0');
        }

        private long GetUnixTicks(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
    }
}
