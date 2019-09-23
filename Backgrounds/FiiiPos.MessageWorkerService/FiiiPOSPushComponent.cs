using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.Data.Agents.JPush.Model;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.MongoDB;
using log4net;

namespace FiiiPos.MessageWorkerService
{
    public class FiiiPOSPushComponent
    {
        public const int REDIS_LANGUAGE_DBINDEX = 3;

        private static readonly bool IsPushProduction =
            Convert.ToBoolean(ConfigurationManager.AppSettings.Get("PushProduction") ?? "false");

        public static readonly string JPUSH_TAG = ConfigurationManager.AppSettings.Get("JpushTag");

        private readonly ILog _log = LogManager.GetLogger(typeof(FiiiPOSPushComponent));

        public const string FiiiPOS_APP_Notice_MerchantId = "FiiiPOS:APP:Notice:MerchantId";
        public const string FiiiPOS_APP_Language_MerchantId = "FiiiPOS:APP:Language:MerchantId";

        public void PushOrderPayed(Guid orderId)
        {
            var order = new OrderDAC().GetOrderByOrderId(orderId);
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);
            var agent = new JPushAgent();
            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{order.MerchantAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{order.MerchantAccountId}") ?? "en";
            var titleKey = "ReceiptTitle";
            var subTitleKey = "ReceiptSubTitle";

            var content = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB [收款成功]
            MessagesComponent.AddMessage(order.MerchantAccountId, UserType.Merchant, order.Id.ToString(), FiiiPayPushType.TYPE_RECEIPT, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            var result = agent.Push(new PushPayload
            {
                Audience = new { registration_id = new List<string> { regId } },
                Platform = "all",
                Notification = new Notification
                {
                    Alert = content,
                    Android = new Android
                    {
                        Alert = subTitle,
                        Title = content,
                        Extras = PushHelper.GetNewExtras(FiiiPayPushType.TYPE_RECEIPT, orderId, content, subTitle, noticeId)
                    }
                },
                Message = new Message
                {
                    Content = content,
                    Extras = PushHelper.GetNewExtras(FiiiPayPushType.TYPE_RECEIPT, orderId, content, subTitle, noticeId)
                },
                Options = new Options { IsApnsProduction = IsPushProduction }

            });

            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushRefundOrder(string orderno)
        {
            var order = new OrderDAC().GetByOrderNo(orderno);
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{order.MerchantAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{order.MerchantAccountId}") ?? "en";
            var titleKey = "BackOfficeRefundOrderTitle";
            var subTitleKey = "BackOfficeRefundOrderSubTitle";
            var content = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPos.GetFormatResource(subTitleKey, lang, coin.Code);

            string noticeId = "";
            //写MongoDB [提币成功]
            MessagesComponent.AddMessage(order.MerchantAccountId, UserType.Merchant, order.Id.ToString(), FiiiPayPushType.TYPE_REFUND_ORDER, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_REFUND_ORDER, new List<string> { regId }, order.Id, content, subTitle, noticeId);

            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushArticle(int id)
        {
            var article = new ArticleDAC().GetById(id);

            var tags = new List<string>();
            tags.Add(JPUSH_TAG);

            RegPushByTags(FiiiPayPushType.TYPE_ARTICLE, tags, id, article.Title, article.Descdescription, Guid.NewGuid().ToString());

        }

        public void PushDeposit(long id)
        {
            var deposit = new MerchantDepositDAC().GetById(id);
            var wallet = new MerchantWalletDAC().GetById(deposit.MerchantWalletId);
            var crypto = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{deposit.MerchantAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{deposit.MerchantAccountId}") ?? "en";
            var titleKey = "DepositTitle";
            var subTitleKey = "DepositSubTitle";
            var title = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, crypto.Code);
            var subTitle = ResourceHelper.FiiiPos.GetFormatResource(subTitleKey, lang, crypto.Code);

            string noticeId = "";
            //写MongoDB [充币成功]
            MessagesComponent.AddMessage(deposit.MerchantAccountId, UserType.Merchant, id.ToString(), FiiiPayPushType.TYPE_DEPOSIT, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_DEPOSIT, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushDepositCancel(long id)
        {
            var deposit = new MerchantDepositDAC().GetById(id);
            var wallet = new MerchantWalletDAC().GetById(deposit.MerchantWalletId);
            var crypto = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{deposit.MerchantAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{deposit.MerchantAccountId}") ?? "en";
            var titleKey = "DepositCancelTitle";
            var subTitleKey = "DepositCancelSubTitle";
            var title = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, crypto.Code);
            var subTitle = ResourceHelper.FiiiPos.GetFormatResource(subTitleKey, lang, crypto.Code);

            string noticeId = "";
            //写MongoDB [充币失败]
            MessagesComponent.AddMessage(deposit.MerchantAccountId, UserType.Merchant, id.ToString(), FiiiPayPushType.TYPE_DEPOSIT_CANCEL, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_DEPOSIT_CANCEL, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushWithdrawCompleted(long id)
        {
            var withdraw = new MerchantWithdrawalDAC().GetById(id);
            var wallet = new MerchantWalletDAC().GetById(withdraw.MerchantWalletId);
            var crypto = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{withdraw.MerchantAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{withdraw.MerchantAccountId}") ?? "en";
            var titleKey = "WithdrawalTitle";
            var subTitleKey = "WithdrawalSubTitle";
            var title = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, crypto.Code);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB [提币成功]
            MessagesComponent.AddMessage(withdraw.MerchantAccountId, UserType.Merchant, id.ToString(), FiiiPayPushType.TYPE_WITHDRAWAL, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_WITHDRAWAL, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");

        }

        public void PushWithdrawReject(long id)
        {
            var withdraw = new MerchantWithdrawalDAC().GetById(id);
            var wallet = new MerchantWalletDAC().GetById(withdraw.MerchantWalletId);
            var crypto = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{withdraw.MerchantAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{withdraw.MerchantAccountId}") ?? "en";
            var titleKey = "WithdrawalRejectTitle";
            var subTitleKey = "WithdrawalRejectSubTitle";
            var title = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, crypto.Code);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB [提币失败]
            MessagesComponent.AddMessage(withdraw.MerchantAccountId, UserType.Merchant, id.ToString(), FiiiPayPushType.TYPE_WITHDRAWAL_Reject, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_WITHDRAWAL_Reject, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");

        }


        public void MerchantLv1Verified(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{record.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{record.AccountId}") ?? "en";
            var titleKey = "MerchantLv1Verified";
            var subTitleKey = "MerchantLv1VerifiedSubTitle";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.Merchant, record.Id.ToString(), FiiiPayPushType.TYPE_Merchant_KYC_LV1_VERIFIED, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_Merchant_KYC_LV1_VERIFIED, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void MerchantLv1Reject(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{record.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{record.AccountId}") ?? "en";
            var titleKey = "MerchantLv1Reject";
            var subTitleKey = "MerchantLv1RejectSubTitle";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.Merchant, record.Id.ToString(), FiiiPayPushType.TYPE_Merchant_KYC_LV1_REJECT, titleKey, subTitleKey, "null", title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_Merchant_KYC_LV1_REJECT, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void MerchantLv2Verified(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{record.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{record.AccountId}") ?? "en";
            var titleKey = "MerchantLv2Verified";
            var subTitleKey = "MerchantLv2VerifiedSubTitle";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);
            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.Merchant, record.Id.ToString(), FiiiPayPushType.TYPE_Merchant_KYC_LV2_VERIFIED, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_Merchant_KYC_LV2_VERIFIED, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void MerchantLv2Reject(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{record.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{record.AccountId}") ?? "en";
            var titleKey = "MerchantLv2Reject";
            var subTitleKey = "MerchantLv2RejectSubTitle";

            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);
            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.Merchant, record.Id.ToString(), FiiiPayPushType.TYPE_Merchant_KYC_LV2_REJECT, titleKey, subTitleKey, "null", title, subTitle, out noticeId);


            RegPush(FiiiPayPushType.TYPE_Merchant_KYC_LV2_REJECT, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushTransferFromEx(long id)
        {
            var order = new MerchantExTransferOrderDAC().GetById(id);
            var crypto = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{order.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{order.AccountId}") ?? "en";
            var titleKey = "TransferFromFiiiExTitle";
            var subTitleKey = "TransferFromFiiiExSubTitle";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB []
            MessagesComponent.AddMessage(order.AccountId, UserType.Merchant, id.ToString(), FiiiPayPushType.TYPE_TRANSFER_FROM_EX, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_TRANSFER_FROM_EX, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushTransferToEx(long id)
        {
            var order = new MerchantExTransferOrderDAC().GetById(id);
            var crypto = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{order.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{order.AccountId}") ?? "en";
            var titleKey = "TransferToFiiiExTitle";
            var subTitleKey = "TransferToFiiiExSubTitle";
            var title = ResourceHelper.FiiiPos.GetFormatResource(titleKey, lang, crypto.Code);
            var subTitle = ResourceHelper.FiiiPos.GetFormatResource(subTitleKey, lang, crypto.Code);

            string noticeId = "";
            //写MongoDB []
            MessagesComponent.AddMessage(order.AccountId, UserType.Merchant, id.ToString(), FiiiPayPushType.TYPE_TRANSFER_TO_EX, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_TRANSFER_TO_EX, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushInviteSuccess(long id)
        {
            var pfDAC = new ProfitDetailDAC();
            var accountId = pfDAC.GetAccountId(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{accountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{accountId}") ?? "en";
            string titleKey = "奖励FIII";
            string subTitleKey = "奖励子标题";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(accountId, UserType.User, id.ToString(), FiiiPayPushType.TYPE_INVITE_REWARD, titleKey, subTitleKey, "FIII", title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_INVITE_REWARD, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }



        private void RegPush(int pushType, List<string> regIdList, object id, string title, string subTitle, string noticeId)
        {
            PushPayload pushEntity = new PushPayload
            {
                Audience = new { registration_id = regIdList },
                Platform = "all",
                Notification = new Notification
                {
                    Alert = title,
                    IOS = new IOS
                    {
                        Alert = new { title = title, body = subTitle },
                        Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                    },
                    Android = new Android
                    {
                        Alert = subTitle,
                        Title = title,
                        Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                    }
                },
                Message = new Message
                {
                    Title = title,
                    Content = subTitle,
                    Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                },
                Options = new Options { IsApnsProduction = IsPushProduction }

            };
            var agent = new JPushAgent();
            agent.Push(pushEntity);
        }

        private void RegPushByTags(int pushType, List<string> tags, object id, string title, string subTitle, string noticeId)
        {
            PushPayload pushEntity = new PushPayload
            {
                Audience = new { tag = tags.ToArray() },
                Platform = "all",
                Notification = new Notification
                {
                    Alert = title,
                    IOS = new IOS
                    {
                        Alert = new { title = title, body = subTitle },
                        Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                    },
                    Android = new Android
                    {
                        Alert = subTitle,
                        Title = title,
                        Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                    }
                },
                Message = new Message
                {
                    Title = title,
                    Content = subTitle,
                    Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                },
                Options = new Options { IsApnsProduction = IsPushProduction }

            };
            var agent = new JPushAgent();
            agent.Push(pushEntity);
        }

        public void PushStoreVerified(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{record.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{record.AccountId}") ?? "en";
            var titleKey = "StoreVerified";
            var subTitleKey = "StoreVerifiedSubTitle";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.Merchant, record.Id.ToString(), FiiiPayPushType.TYPE_STORE_VERIFIED, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_STORE_VERIFIED, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushStoreReject(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"{FiiiPOS_APP_Notice_MerchantId}:{record.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"{FiiiPOS_APP_Language_MerchantId}:{record.AccountId}") ?? "en";
            var titleKey = "StoreReject";
            var subTitleKey = "StoreRejectSubTitle";
            var title = ResourceHelper.FiiiPos.GetResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPos.GetResource(subTitleKey, lang);

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.Merchant, record.Id.ToString(), FiiiPayPushType.TYPE_STORE_REJECT, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_STORE_REJECT, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

    }
}
