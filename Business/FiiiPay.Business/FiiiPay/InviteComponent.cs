using FiiiPay.Data;
using FiiiPay.DTO.Invite;
using FiiiPay.Entities;
using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Business.Properties;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Component;

namespace FiiiPay.Business
{
    public class InviteComponent : BaseComponent
    {
        private string CreateOrderno()
        {
            return NumberGenerator.GenerateUnixOrderNo();
        }
        public PreFiiiPayProfitOM PreFiiiPayProfit(UserAccount account)
        {
            var iDAC = new InviteRecordDAC();
            var collection = iDAC.GetFiiiPayProfitDetails(account.Id);
            int inviteUserCount = 0;
            decimal totalAmount = decimal.Zero;
            foreach (var item in collection)
            {
                if (item.Type == ProfitType.InvitePiiiPay)
                {
                    inviteUserCount++;
                }
                totalAmount += item.CryptoAmount;
            }
            return new PreFiiiPayProfitOM() { TotalProfitAmount = totalAmount, InvitationCount = inviteUserCount };
        }



        public PreFiiiPosProfitOM PreFiiiPosProfit(UserAccount account)
        {
            var iDAC = new InviteRecordDAC();
            var collection = iDAC.GetFiiiPosProfitDetails(account.Id, (int)SystemPlatform.FiiiPOS);
            var count = collection.GroupBy(item => item.MerchantId).Count();
            decimal rate = 0;
            if(count>= 3 && count < 10)
            {
                rate = 0.04m;
            }
            else if(count >= 10 && count < 30)
            {
                rate = 0.06m;
            }
            else if (count >= 30)
            {
                rate = 0.08m;
            }
            //todo
            decimal totalAmount = decimal.Zero;
            foreach (var item in collection)
            {
                totalAmount += item.CryptoAmount;
            }
            return new PreFiiiPosProfitOM() { CryptoAmount = totalAmount.ToString(8), InviteCount = count, ProfitRate = rate };
        }

        public List<ProfitDetailOM> FiiipayDetail(UserAccount account, int pageIndex)
        {
            var iDAC = new InviteRecordDAC();
            int offset, nextCount = 0;
            if (pageIndex == 1)
            {
                offset = 0;
                nextCount = 20;
            }
            else
            {
                offset = (pageIndex - 2) * 10 + 20;
                nextCount = 10;
            }
            var collection = iDAC.GetFiiiPayProfitDetails(account.Id, new Tuple<int, int>(offset, nextCount));
            List<ProfitDetailOM> result = new List<ProfitDetailOM>();
            collection.ForEach(item => result.Add(new ProfitDetailOM()
            {
                AccountId = item.AccountId,
                CellPhone = item.CellPhone,
                CryptoAmount = item.CryptoAmount,
                Id = item.Id,
                InvitationId = item.InvitationId,
                InvitedAccountId = item.InvitedAccountId,
                PhoneCode = item.PhoneCode,
                Timestamp = item.Timestamp,
                Type = item.Type
            }));
            return result;
        }

        public List<FiiiposBonusRecordOM> FiiiposDetail(UserAccount account)
        {
            var iDAC = new InviteRecordDAC();
            return iDAC.GetFiiiPosProfitDetailList(account.Id, (int)SystemPlatform.FiiiPOS).Select(item=> new FiiiposBonusRecordOM(){MerchantId = item.MerchantId,MerchantName = item.MerchantName,TotalCryptoAmount = item.TotalCryptoAmount.ToString(8)}).ToList();
        }
        /// <summary>
        /// 获取分红的详细信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public FiiiposProfitDetailRecordOM FiiiposMoreDetail(FiiiposBonusRecordIM im, UserAccount account)
        {
            var iDAC = new InviteRecordDAC();
            var pfDAC = new ProfitDetailDAC();
            var data = iDAC.GetDetailByAccountId(im.MerchantId);

            return new FiiiposProfitDetailRecordOM()
            {
                Data = pfDAC.GetFiiiposBonusRecords(data.Id, account).Select(item => new FiiiposProfitDetailRecordOM.BonusDetail()
                {
                    CryptoAmount = item.CryptoAmount.ToString(8),
                    Timestamp = item.Timestamp.ToUnixTime().ToString()
                }).ToList(),
                MerchantName = im.MerchantName,
                TotalAmount = im.TotalCryptoAmount.ToString(8)
            };
        }
        
        public List<InviteRankOM> GetInviteRankList(InviteRankIM im)
        {
            var iDAC = new InviteRecordDAC();
           
            if (im.Type == (int)SystemPlatform.FiiiPOS)
            {
                var result = iDAC.GetFiiiPosRankDetails(im.Count)?.Select(item => new InviteRankOM()
                {
                    AccountId = item.AccountId,
                    CellPhone = item.CellPhone,
                    CryptoAmount = item.CryptoAmount.ToString(2),
                    PhoneCode = item.PhoneCode
                }).ToList() ?? new List<InviteRankOM>();
                if(result.Count < im.Count)
                {
                    result.AddRange(iDAC.GetFiiiPosRankDetails(im.Count - result.Count)?.Select(item => new InviteRankOM()
                    {
                        AccountId = item.AccountId,
                        CellPhone = item.CellPhone,
                        CryptoAmount = item.CryptoAmount.ToString(2),
                        PhoneCode = item.PhoneCode
                    }).ToList() ?? new List<InviteRankOM>());
                }

                return result;
            }
            else
            {
                return iDAC.GetFiiiPayRankDetails(im.Count).Select(item => new InviteRankOM()
                {
                    AccountId = item.AccountId,
                    CellPhone = item.CellPhone,
                    CryptoAmount = item.CryptoAmount.ToString(2),
                    PhoneCode = item.PhoneCode
                }).ToList();
            }
        }

        public SingleBonusDetailOM GetSingleBonusDetail(int im)
        {
            var pfDAC = new ProfitDetailDAC();
            var detail = pfDAC.GetBonusDetailById(im);
            var coin = new CryptocurrencyDAC().GetById(detail.CryptoId);
            var code = coin.Code;

            string statusStr = "";
            if (detail.Status == InviteStatusType.IssuedActive)
                statusStr = Resources.DepositConfirmed;
            else if (detail.Status == InviteStatusType.IssuedFrozen)
                statusStr = Resources.DepositPending;

            //暂时只考虑一种状况
            return new SingleBonusDetailOM()
            {
                Type = SingleBonusDetailOM.BonusType.BonusIncome,
                TypeStr = Resources.BonusIncome,
                TradeDescription = Resources.BonusDescription,
                BonusFrom = Resources.BonusComeFrom,
                Status = detail.Status,
                StatusStr = statusStr,
                CryptoCode = code,
                Timestamp = detail.Timestamp.ToUnixTime().ToString(),
                OrderNo = detail.OrderNo,
                Amount = detail.CryptoAmount.ToString(coin.DecimalPlace)
            };
        }
        public int GetCurrentRank(SystemPlatform platform, UserAccount account)
        {
            var totalProfitAmount = new ProfitDetailDAC().GetUserBonusTotalAmount(account, (int)platform);
            if (totalProfitAmount == 0m)
            {
                return -1;
            }
            return new InviteRecordDAC().GetUserRanking(account.Id, (int)platform);
        }
        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="inviteCode">邀请码</param>
        /// <param name="accoundId">被邀请人id</param>
        /// /// <param name="type">1:fiiipay 2:fiiipos</param>
        public void InsertRecord(InviteRecordIM im)
        {
            //判断是fiiipos还是fiiipay

            //1 插入数据 invite profit两个表 邀请双方都支持插入数据 钱包 流水进行更新
            // 判断当前邀请人数到达五十 进行推送。。。。。。。。。。。。

            //2 插入数据 invite
            if (!(im.Type == (int)SystemPlatform.FiiiPay || im.Type == (int)SystemPlatform.FiiiPOS))
            {
                throw new ArgumentException("只支持fiiipay和fiiipos邀请");
            }
            var userAccountDAC = new UserAccountDAC();
            var account = userAccountDAC.GetByInvitationCode(im.InvitationCode);
            var settingCollection = new MasterSettingDAC().SelectByGroup("InviteReward");
            var cryptoCurrencty = new CryptocurrencyDAC().GetByCode("FIII");
            var cryptoId = cryptoCurrencty.Id;

            if (im.Type == (int)SystemPlatform.FiiiPay)
            {
                var iDAC = new InviteRecordDAC();
                var pfDAC = new ProfitDetailDAC();
                var uwComponent = new UserWalletComponent();

                var uwDAC = new UserWalletDAC();
                var uwsDAC = new UserWalletStatementDAC();
                var utDAC = new UserTransactionDAC();

                var inviteMoney = decimal.Parse(settingCollection.Where(item => item.Name == "Invite_Reward_Amount").Select(item => item.Value).FirstOrDefault());
                var rewardMoney = decimal.Parse(settingCollection.Where(item => item.Name == "Over_People_Count_Reward_Amount").Select(item => item.Value).FirstOrDefault());
                var maxCount = decimal.Parse(settingCollection.Where(item => item.Name == "Max_Reward_Amount").Select(item => item.Value).FirstOrDefault());
                var orderNo1 = CreateOrderno();
                var orderNo2 = CreateOrderno();
                var inviteWallet = uwComponent.GetUserWallet(account.Id, cryptoId);
                if (inviteWallet == null)
                    inviteWallet = uwComponent.GenerateWallet(account.Id, cryptoId);

                var beInvitedWallet = uwComponent.GetUserWallet(im.BeInvitedAccountId, cryptoId);
                if (beInvitedWallet == null)
                    beInvitedWallet = uwComponent.GenerateWallet(im.BeInvitedAccountId, cryptoId);

                int inviteId;
                long profitId1 = 0;//邀请人奖励ID
                long profitId2 = 0;//被邀请人奖励ID
                long exProfitId1 = 0;//邀请人额外奖励ID
                decimal totalReward = pfDAC.GetTotalReward(account);
                int invitedCount = pfDAC.GetInvitedCount(account);

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
                {
                    //插入数据到inviterecord表中
                    inviteId = iDAC.Insert(new InviteRecord() { Type = (InviteType)im.Type, AccountId = im.BeInvitedAccountId, Timestamp = DateTime.UtcNow, InviterCode = im.InvitationCode, InviterAccountId = account.Id });

                    if (totalReward < maxCount)
                    {
                        profitId1 = pfDAC.Insert(new ProfitDetail()
                        {
                            InvitationId = inviteId,
                            CryptoAmount = inviteMoney,
                            AccountId = account.Id,
                            Status = InviteStatusType.IssuedFrozen,
                            Type = ProfitType.InvitePiiiPay,
                            OrderNo = orderNo1,
                            Timestamp = DateTime.UtcNow,
                            CryptoId = cryptoId
                        });

                        utDAC.Insert(new UserTransaction
                        {
                            Id = Guid.NewGuid(),
                            AccountId = account.Id,
                            CryptoId = cryptoCurrencty.Id,
                            CryptoCode = cryptoCurrencty.Code,
                            Type = UserTransactionType.Profit,
                            DetailId = profitId1.ToString(),
                            Status = (byte)InviteStatusType.IssuedFrozen,
                            Timestamp = DateTime.UtcNow,
                            Amount = inviteMoney,
                            OrderNo = orderNo1
                        });

                        uwDAC.IncreaseFrozen(inviteWallet.Id, inviteMoney);

                        uwsDAC.Insert(new UserWalletStatement
                        {
                            WalletId = inviteWallet.Id,
                            Action = UserWalletStatementAction.Invite,
                            Amount = 0,
                            Balance = inviteWallet.Balance,
                            FrozenAmount = inviteMoney,
                            FrozenBalance = inviteWallet.FrozenBalance + inviteMoney,
                            Timestamp = DateTime.UtcNow
                        });

                        // 每当满50人时则可以奖励 采用了插入操作 所以只要满足为49个就可以了
                        if ((invitedCount + 1) % 50 == 0)
                        {
                            var pd50 = new ProfitDetail()
                            {
                                InvitationId = inviteId,
                                CryptoAmount = rewardMoney,
                                AccountId = account.Id,
                                Status = InviteStatusType.IssuedFrozen,
                                Type = ProfitType.Reward,
                                OrderNo = CreateOrderno(),
                                Timestamp = DateTime.UtcNow,
                                CryptoId = cryptoId
                            };
                            exProfitId1 = pfDAC.Insert(pd50);

                            utDAC.Insert(new UserTransaction
                            {
                                Id = Guid.NewGuid(),
                                AccountId = account.Id,
                                CryptoId = cryptoCurrencty.Id,
                                CryptoCode = cryptoCurrencty.Code,
                                Type = UserTransactionType.Profit,
                                DetailId = exProfitId1.ToString(),
                                Status = (byte)InviteStatusType.IssuedFrozen,
                                Timestamp = DateTime.UtcNow,
                                Amount = rewardMoney,
                                OrderNo = pd50.OrderNo
                            });

                            uwDAC.IncreaseFrozen(inviteWallet.Id, rewardMoney);

                            uwsDAC.Insert(new UserWalletStatement
                            {
                                WalletId = inviteWallet.Id,
                                Action = UserWalletStatementAction.Invite,
                                Amount = 0,
                                Balance = inviteWallet.Balance,
                                FrozenAmount = rewardMoney,
                                FrozenBalance = inviteWallet.FrozenBalance + inviteMoney + rewardMoney,
                                Timestamp = DateTime.UtcNow
                            });
                        }
                        profitId2 = pfDAC.Insert(new ProfitDetail()
                        {
                            InvitationId = inviteId,
                            CryptoAmount = inviteMoney,
                            AccountId = im.BeInvitedAccountId,
                            Type = ProfitType.BeInvited,
                            Status = InviteStatusType.IssuedActive,
                            OrderNo = orderNo2,
                            Timestamp = DateTime.UtcNow,
                            CryptoId = cryptoId
                        });
                        utDAC.Insert(new UserTransaction
                        {
                            Id = Guid.NewGuid(),
                            AccountId = im.BeInvitedAccountId,
                            CryptoId = cryptoCurrencty.Id,
                            CryptoCode = cryptoCurrencty.Code,
                            Type = UserTransactionType.Profit,
                            DetailId = profitId2.ToString(),
                            Status = (byte)InviteStatusType.IssuedActive,
                            Timestamp = DateTime.UtcNow,
                            Amount = inviteMoney,
                            OrderNo = orderNo2
                        });
                        uwDAC.Increase(beInvitedWallet.Id, inviteMoney);
                        uwsDAC.Insert(new UserWalletStatement
                        {
                            WalletId = beInvitedWallet.Id,
                            Action = UserWalletStatementAction.BeInvite,
                            Amount = inviteMoney,
                            Balance = beInvitedWallet.Balance + inviteMoney,
                            FrozenAmount = 0,
                            FrozenBalance = beInvitedWallet.FrozenBalance,
                            Timestamp = DateTime.UtcNow
                        });

                    }
                    scope.Complete();
                }

                if (!(im.Type == (int)SystemPlatform.FiiiPay || im.Type == (int)SystemPlatform.FiiiPOS))
                {
                    throw new ArgumentException("只支持fiiipay和fiiipos邀请");
                }

                if (im.Type == (int)SystemPlatform.FiiiPay)
                    UserMSMQ.PubUserInviteSuccessed(profitId2, 0);
                //else if (im.Type == SystemPlatform.FiiiPOS)
                //    MerchantMSMQ.PubUserInviteSuccessed(profitId2, 0);
            }
            //else
            //{
            //    var iDAC = new InviteRecordDAC();
            //    iDAC.Insert(new InviteRecord() { Type = im.Type, AccountId = im.BeInvitedAccountId, Timestamp = DateTime.UtcNow, InviterCode = im.InvitationCode, InviterAccountId = account.Id });
            //}
        }
    }
}
