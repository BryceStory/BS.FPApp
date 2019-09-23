using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using FiiiPay.Data;
using FiiiPay.Data.Agents;
using FiiiPay.Data.Agents.JPush.Model;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework.Cache;
using FiiiPay.Framework.MongoDB;
using FiiiPay.Framework.MQTT;
using FiiiPay.MessageWorkerService.Properties;
using log4net;
using Newtonsoft.Json;

namespace FiiiPay.MessageWorkerService
{
    public class FiiiPayPushComponent
    {
        public const int REDIS_LANGUAGE_DBINDEX = 3;

        private static readonly bool IsPushProduction =
            Convert.ToBoolean(ConfigurationManager.AppSettings.Get("PushProduction") ?? "false");

        public static readonly string JPUSH_TAG = ConfigurationManager.AppSettings.Get("JpushTag");

        private readonly ILog _log = LogManager.GetLogger(typeof(FiiiPayPushComponent));

        private readonly List<string> _resourcePropertyNames =
            typeof(Resources).GetProperties().Select(item => item.Name).ToList();

        //public void PushTest(Guid userId)
        //{
        //    var agent = new JPushAgent();
        //    var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{userId}");
        //    var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{userId}") ?? "en";
        //    var content = ResourceHelper.FiiiPay.GetResource("订单待支付通知", new CultureInfo(lang));
        //    var entity = new PushPayload
        //    {
        //        Audience = new { registration_id = new List<string> { regId } },
        //        Platform = "all",
        //        //Notification是显示在通知栏的内容，没有这个，将不会在通知栏显示消息
        //        Notification = new Notification
        //        {
        //            Alert = content
        //        },
        //        Message = new Message
        //        {
        //            Content = content,
        //            //Extras是通知栏推送或者应用内推送的数据包
        //            Extras = PushHelper.GetNewExtras(-1, userId, "我是子标题")
        //        },
        //        Options = new Options { IsApnsProduction = true }
        //    };
        //    agent.PushAsync(entity);
        //}

        //public async Task PushTestToAll()
        //{
        //    var agent = new JPushAgent();
        //    await agent.PushAsync(new PushPayload
        //    {
        //        Audience = new { tag = new string[] { "FiiiPay" } },
        //        Platform = "all",
        //        Notification = new Notification
        //        {
        //            Alert = "PushTestToAll2"
        //        },
        //        Message = new Message
        //        {
        //            Content = "PushTestToAll2",
        //            Extras = PushHelper.GetNewExtras(FiiiPayPushType.TYPE_ARTICLE, 1)
        //        },
        //        Options = new Options { IsApnsProduction = IsPushProduction }

        //    });
        //}

        //public async Task PushTestToUser(string regId)
        //{
        //    var agent = new JPushAgent();
        //    await agent.PushAsync(new PushPayload
        //    {
        //        Audience = new { registration_id = new string[] { regId } },
        //        Platform = "all",
        //        Notification = new Notification
        //        {
        //            Alert = "PushTestToUser3"
        //        },
        //        Message = new Message
        //        {
        //            Content = "PushTestToUser3",
        //            Extras = PushHelper.GetNewExtras(FiiiPayPushType.TYPE_ARTICLE, 1)
        //        },
        //        Options = new Options { IsApnsProduction = IsPushProduction }
        //    });
        //}

        public void PushPayOrder(string message)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<SendPayOrderModel>(message);
                if (string.IsNullOrWhiteSpace(msg.OrderNo))
                {
                    _log.Error("Push pay order null,order no " + msg.OrderNo);
                    return;
                }

                //var agent = new JPushAgent();
                //var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{msg.UserAccountId}");
                //if (regId.Contains("UTF-8"))
                //{
                //    return;
                //}
                //var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{msg.UserAccountId}") ?? "en";
                //var result = agent.Push(new PushPayload
                //{
                //    Audience = new { registration_id = new List<string> { regId } },
                //    Platform = "all",
                //    Message = new Message
                //    {
                //        Content = ResourceHelper.FiiiPay.GetResource("订单待支付通知", new CultureInfo(lang)),
                //        Extras = PushHelper.GetNewExtras(FiiiPayPushType.TYPE_PUSH_PAY_ORDER, msg.OrderNo)
                //    },
                //    Options = new Options { IsApnsProduction = IsPushProduction }
                //});

                var result = MqttAgent.PushMessage(new MqttMessageBuilder()
                    .WithTopic("FiiiPay", FiiiPayPushType.TYPE_PUSH_PAY_ORDER.ToString(), msg.UserAccountId)
                    .WithPayload(new { msg.OrderNo })
                    .Build());
                _log.Info($"Pushed mqtt message{result}");

                //_log.Info("Push pay order " + result);
            }
            catch (Exception exception)
            {
                _log.Error(exception);
            }
        }

        public void RefundOrder(string orderno)
        {
            var order = new OrderDAC().GetByOrderNo(orderno);
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "RefundTitle";
            var subTitleKey = "RefundSubTitle";

            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }

            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, lang);



            string noticeId = "";
            //写MongoDB [退款]
            MessagesComponent.AddMessage(order.UserAccountId.Value, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_REFUND_ORDER, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_REFUND_ORDER, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void BackOfficeRefundOrder(string orderno)
        {
            var order = new OrderDAC().GetByOrderNo(orderno);
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "BackOfficeRefundOrderTitle";
            var subTitleKey = "BackOfficeRefundOrderSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            //var content = string.Format(Resources.ResourceHelper.GetString(titleKey, new System.Globalization.CultureInfo(lang)), coin.Code);
            //var subTitle = string.Format(Resources.ResourceHelper.GetString(subTitleKey, new System.Globalization.CultureInfo(lang)), coin.Code);
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang, coin.Code);

            string noticeId = "";
            //写MongoDB [退款]
            MessagesComponent.AddMessage(order.UserAccountId.Value, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_REFUND_ORDER, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_REFUND_ORDER, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushArticle(int id)
        {
            var article = new ArticleDAC().GetById(id);
            var tags = new List<string>();
            tags.Add(JPUSH_TAG);

            RegPushByTags(FiiiPayPushType.TYPE_ARTICLE, tags, id, article.Title, article.Descdescription, Guid.NewGuid().ToString());

            //var result = agent.Push(new PushPayload
            //{
            //    Audience = new { tag_and = tags.ToArray() },
            //    Platform = "all",
            //    Notification = new Notification
            //    {
            //        Alert = article.Title
            //    },
            //    Message = new Message
            //    {
            //        Content = article.Title,
            //        Extras = PushHelper.GetNewExtras(FiiiPayPushType.TYPE_ARTICLE, id)
            //    },
            //    Options = new Options { IsApnsProduction = IsPushProduction }
            //});
        }

        public void PushDeposit(long id)
        {
            var order = new UserDepositDAC().GetById(id);
            var wallet = new UserWalletDAC().GetById(order.UserWalletId);
            var coin = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "DepositTitle";
            var subTitleKey = "DepositSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB [充币成功]
            MessagesComponent.AddMessage(order.UserAccountId, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_DEPOSIT, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_DEPOSIT, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushDepositCancel(long id)
        {
            var order = new UserDepositDAC().GetById(id);
            var wallet = new UserWalletDAC().GetById(order.UserWalletId);
            var coin = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "DepositCancelTitle";
            var subTitleKey = "DepositCancelSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang, coin.Code);

            string noticeId = "";
            //写MongoDB [充币失败]
            MessagesComponent.AddMessage(order.UserAccountId, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_DEPOSIT_CANCEL, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_DEPOSIT_CANCEL, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");

        }

        public void PushWithdrawCompleted(long id)
        {
            var order = new UserWithdrawalDAC().GetById(id);
            var wallet = new UserWalletDAC().GetById(order.UserWalletId);
            var coin = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "WithdrawalTitle";
            var subTitleKey = "WithdrawalSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, lang);

            string noticeId = "";

            //写MongoDB [充币成功]
            MessagesComponent.AddMessage(order.UserAccountId, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_WITHDRAWAL, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_WITHDRAWAL, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushWithdrawReject(long id)
        {
            var order = new UserWithdrawalDAC().GetById(id);
            var wallet = new UserWalletDAC().GetById(order.UserWalletId);
            var coin = new CryptocurrencyDAC().GetById(wallet.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "WithdrawalRejectTitle";
            var subTitleKey = "WithdrawalRejectSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB [提币失败]
            MessagesComponent.AddMessage(order.UserAccountId, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_WITHDRAWAL_Reject, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_WITHDRAWAL_Reject, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushConsume(Guid orderId)
        {
            var order = new OrderDAC().GetOrderByOrderId(orderId);
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "ConsumeTitle";
            var subTitleKey = "ConsumeSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang, coin.Code);

            string noticeId = "";
            //写MongoDB [消费（支付成功）]
            MessagesComponent.AddMessage(order.UserAccountId.Value, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_CONSUME, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_CONSUME, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushKYC_LV1_VERIFIED(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{record.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{record.AccountId}") ?? "en";
            var titleKey = "KYC_LV1_VERIFIEDTitle";
            var subTitleKey = "KYC_LV1_VERIFIEDSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource("KYC_LV1_VERIFIEDTitle", new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetResource("KYC_LV1_VERIFIEDSubTitle", new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_USER_KYC_LV1_VERIFIED, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_USER_KYC_LV1_VERIFIED, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushKYC_LV1_REJECT(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{record.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{record.AccountId}") ?? "en";
            var titleKey = "KYC_LV1_REJECTTitle";
            var subTitleKey = "KYC_LV1_REJECTSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource("KYC_LV1_REJECTTitle", new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetResource("KYC_LV1_REJECTSubTitle", new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_USER_KYC_LV1_REJECT, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_USER_KYC_LV1_REJECT, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushKYC_LV2_VERIFIED(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{record.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{record.AccountId}") ?? "en";
            var titleKey = "KYC_LV2_VERIFIEDTitle";
            var subTitleKey = "KYC_LV2_VERIFIEDSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource("KYC_LV2_VERIFIEDTitle", new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetResource("KYC_LV2_VERIFIEDSubTitle", new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_USER_KYC_LV2_VERIFIED, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_USER_KYC_LV2_VERIFIED, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushKYC_LV2_REJECT(long id)
        {
            var record = new VerifyRecordDAC().GetById(id);
            if (record == null) return;

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{record.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{record.AccountId}") ?? "en";

            var titleKey = "KYC_LV2_REJECTTitle";
            var subTitleKey = "KYC_LV2_REJECTSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource("KYC_LV2_REJECTTitle", new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetResource("KYC_LV2_REJECTSubTitle", new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(record.AccountId, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_USER_KYC_LV2_REJECT, titleKey, subTitleKey, null, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_USER_KYC_LV2_REJECT, new List<string> { regId }, id, title, subTitle, Guid.NewGuid().ToString());
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushTransferOut(long id)
        {
            var transfer = new UserTransferDAC().GetTransfer(id);
            Guid accountId = transfer.FromUserAccountId;

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{accountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{accountId}") ?? "en";
            string titleKey = "TransferOutTitle";
            string subTitleKey = "TransferOutSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetFormatResource(titleKey, new CultureInfo(lang), transfer.CoinCode);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, new CultureInfo(lang));

            List<string> regIdList = new List<string> { regId };

            string noticeId = "";
            MessagesComponent.AddMessage(accountId, UserType.User, id.ToString(), FiiiPayPushType.TYPE_USER_TRANSFER_OUT, titleKey, subTitleKey, transfer.CoinCode, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_USER_TRANSFER_OUT, regIdList, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");

        }

        public void PushTransferInto(long id)
        {
            var transfer = new UserTransferDAC().GetTransfer(id);
            Guid accountId = transfer.ToUserAccountId;

            //TODO:
            //var profile = new UserProfileAgent().GetUserProfile(transfer.FromUserAccountId);
            //string showFullname = (string.IsNullOrEmpty(profile.FirstName) ? "" : "* ") + profile.LastName;
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{accountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{accountId}") ?? "en";
            string titleKey = "TransferIntoTitle";
            string subTitleKey = "TransferIntoSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetFormatResource(titleKey, new CultureInfo(lang), transfer.CoinCode);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, new CultureInfo(lang), "***");

            List<string> regIdList = new List<string> { regId };

            string noticeId = "";
            MessagesComponent.AddMessage(accountId, UserType.User, id.ToString(), FiiiPayPushType.TYPE_USER_TRANSFER_INTO, titleKey, subTitleKey, transfer.CoinCode, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_USER_TRANSFER_INTO, regIdList, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        private void RegPush(int pushType, List<string> regIdList, object id, string title, string subTitle, string noticeId, bool SendMessage = false)
        {
            if (regIdList == null || string.IsNullOrEmpty(regIdList[0]))
                return;
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
                Options = new Options { IsApnsProduction = IsPushProduction }
            };
            if (SendMessage)
            {
                pushEntity.Message = new Message
                {
                    Title = title,
                    Content = subTitle,
                    Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                };
            }
            var agent = new JPushAgent();
            var result = agent.Push(pushEntity);
            LogHelper.Info($"regids:{string.Join(",",regIdList)} ;result:{result}");
        }

        public void PushTransferFromEx(long id)
        {
            var order = new UserExTransferOrderDAC().GetById(id);
            var crypto = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.AccountId}") ?? "en";
            var titleKey = "TransferFromFiiiExTitle";
            var subTitleKey = "TransferFromFiiiExSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetFormatResource(titleKey, new CultureInfo(lang), crypto.Code);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, new CultureInfo(lang), crypto.Code);

            string noticeId = "";
            //写MongoDB []
            MessagesComponent.AddMessage(order.AccountId, UserType.User, id.ToString(), FiiiPayPushType.TYPE_TRANSFER_FROM_EX, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_TRANSFER_FROM_EX, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushTransferToEx(long id)
        {
            var order = new UserExTransferOrderDAC().GetById(id);
            var crypto = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.AccountId}") ?? "en";
            var titleKey = "TransferToFiiiExTitle";
            var subTitleKey = "TransferToFiiiExSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetFormatResource(titleKey, new CultureInfo(lang), crypto.Code);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, new CultureInfo(lang), crypto.Code);

            string noticeId = "";
            //写MongoDB []
            MessagesComponent.AddMessage(order.AccountId, UserType.User, id.ToString(), FiiiPayPushType.TYPE_TRANSFER_TO_EX, titleKey, subTitleKey, crypto.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_TRANSFER_TO_EX, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuple">
        /// Guid为fiiipay用户的id,long为POSMerchantBindRecords的主键
        /// </param>
        public void UnBindingFiiipos(Tuple<Guid, long> tuple)
        {
            var record = new POSMerchantBindRecordDAC().GetById(tuple.Item2);
            var merchantAccountName = new MerchantAccountDAC().GetById(record.MerchantId).MerchantName;
            var accountId = tuple.Item1;
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{accountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{accountId}") ?? "en";
            string titleKey = "RewardUnbindFiiiposTitle";
            string subTitleKey = "RewardUnbindFiiiposSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource(titleKey, new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, new CultureInfo(lang), merchantAccountName);

            string noticeId = "";
            MessagesComponent.AddMessage(accountId, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_UNBIND_POS, titleKey, subTitleKey, merchantAccountName, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_UNBIND_POS, new List<string> { regId }, record.Id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuple">
        /// Guid为fiiipay用户的id,long为POSMerchantBindRecords的主键
        /// </param>
        public void InviteFiiiposSuccess(Tuple<Guid, long> tuple)
        {
            var record = new POSMerchantBindRecordDAC().GetById(tuple.Item2);
            var accountId = tuple.Item1;
            var merchant = new MerchantAccountDAC().GetById(record.MerchantId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{accountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{accountId}") ?? "en";

            string titleKey = "InviteFiiiposSuccessTitle";
            string subTitleKey = "InviteFiiiposSuccessSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource(titleKey, new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, new CultureInfo(lang), merchant.MerchantName);

            string noticeId = "";
            MessagesComponent.AddMessage(accountId, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_INVITE_FIIIPOS_SUCCESS, titleKey, subTitleKey, merchant.MerchantName, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_INVITE_FIIIPOS_SUCCESS, new List<string> { regId }, record.Id, title, subTitle, noticeId);

            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");

        }

        /// <summary>
        /// 网关支付
        /// </summary>
        /// <param name="tuple">
        /// Guid为fiiipay用户的id,long为POSMerchantBindRecords的主键
        /// </param>
        public void ShopPayment(Guid id)
        {
            var order = new GatewayOrderDAC().GetByOrderId(id);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";

            string titleKey = "GatewayOrderPaymentTitle";
            string subTitleKey = "GatewayOrderPaymentSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetFormatResource(titleKey, new CultureInfo(lang), cryptoCurrency.Code);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(order.UserAccountId.Value, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_GATEWAY_ORDER_PAYMENT, titleKey, subTitleKey, cryptoCurrency.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_GATEWAY_ORDER_PAYMENT, new List<string> { regId }, order.Id, title, subTitle, noticeId);

            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");

        }

        /// <summary>
        /// 网关支付
        /// </summary>
        /// <param name="tuple">
        /// Guid为fiiipay用户的id,long为POSMerchantBindRecords的主键
        /// </param>
        public void ShopPaymentRefund(Guid id)
        {
            var record = new GatewayRefundOrderDAC().GetById(id);
            var order = new GatewayOrderDAC().GetByOrderId(record.OrderId);
            var cryptoCurrency = new CryptocurrencyDAC().GetById(order.CryptoId);
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";

            string titleKey = "GatewayOrderPaymentRefundTitle";
            string subTitleKey = "GatewayOrderPaymentRefundSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetFormatResource(titleKey, new CultureInfo(lang), cryptoCurrency.Code);
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(order.UserAccountId.Value, UserType.User, record.Id.ToString(), FiiiPayPushType.TYPE_GATEWAY_ORDER_PAYMENT_REFUND, titleKey, subTitleKey, cryptoCurrency.Code, title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_GATEWAY_ORDER_PAYMENT_REFUND, new List<string> { regId }, record.Id, title, subTitle, noticeId);

            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");

        }

        public void PushInviteSuccess(long id)
        {
            var pfDAC = new ProfitDetailDAC();
            var accountId = pfDAC.GetAccountId(id);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{accountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{accountId}") ?? "en";
            string titleKey = "奖励FIII";
            string subTitleKey = "奖励子标题";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var title = ResourceHelper.FiiiPay.GetResource(titleKey, new CultureInfo(lang));
            var subTitle = ResourceHelper.FiiiPay.GetResource(subTitleKey, new CultureInfo(lang));

            string noticeId = "";
            MessagesComponent.AddMessage(accountId, UserType.User, id.ToString(), FiiiPayPushType.TYPE_INVITE_REWARD, titleKey, subTitleKey, "FIII", title, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_INVITE_REWARD, new List<string> { regId }, id, title, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{title}----------{subTitle}");
        }

        public void PushBiller(Guid id)
        {
            var billerOrder = new BillerOrderDAC().GetById(id);
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{billerOrder.AccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{billerOrder.AccountId}") ?? "en";
            var titleKey = billerOrder.Status == BillerOrderStatus.Complete ? "BillerTitle" : "BillerFailTitle";
            var subTitleKey = billerOrder.Status == BillerOrderStatus.Complete ? "BillerCompleteSubTitle" : "BillerFailSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, billerOrder.CryptoCode);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang);

            string noticeId = "";
            //写MongoDB [消费（支付成功）]
            MessagesComponent.AddMessage(billerOrder.AccountId, UserType.User, billerOrder.Id.ToString(), billerOrder.Status == BillerOrderStatus.Complete ? FiiiPayPushType.TYPE_BILLER_COMPLETE : FiiiPayPushType.TYPE_BILLER_FAIL, titleKey, subTitleKey, billerOrder.CryptoCode, content, subTitle, out noticeId);

            RegPush(billerOrder.Status == BillerOrderStatus.Complete ? FiiiPayPushType.TYPE_BILLER_COMPLETE : FiiiPayPushType.TYPE_BILLER_FAIL, new List<string> { regId }, billerOrder.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void PushConsume(string id)
        {
            var orderId = Guid.Parse(id);
            var order = new OrderDAC().GetOrderByOrderId(orderId);
            var coin = new CryptocurrencyDAC().GetById(order.CryptoId);

            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.UserAccountId}");

            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "ConsumeTitle";
            var subTitleKey = "ConsumeSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, coin.Code);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang, coin.Code);

            string noticeId = "";
            //写MongoDB [消费（支付成功）]
            MessagesComponent.AddMessage(order.UserAccountId.Value, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_CONSUME, titleKey, subTitleKey, coin.Code, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_CONSUME, new List<string> { regId }, order.Id, content, subTitle, noticeId);
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");
        }

        public void StoreOrderPayed(StoreOrderMessage order)
        {
            PushStoreOrderIncome(order);
            PushStoreOrderConsume(order);
        }
        private void PushStoreOrderIncome(StoreOrderMessage order)
        {
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{order.MerchantInfoId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.MerchantInfoId}") ?? "en";
            var titleKey = "StoreOrderIncomeTitle";
            var subTitleKey = "StoreOrderIncomeSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, order.CryptoCode);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang, order.CryptoCode);

            string noticeId = "";
            
            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");

            MessagesComponent.AddMessage(order.MerchantInfoId, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_STOREORDER_INCOME, titleKey, subTitleKey, order.CryptoCode, content, subTitle, out noticeId);

            RegPush(FiiiPayPushType.TYPE_STOREORDER_INCOME, new List<string> { regId }, order.Id, content, subTitle, noticeId);
        }
        private void PushStoreOrderConsume(StoreOrderMessage order)
        {
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{order.UserAccountId}") ?? "en";
            var titleKey = "StoreOrderConsumeTitle";
            var subTitleKey = "StoreOrderConsumeSubTitle";
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception("没有找到资源");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang, order.CryptoCode);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang, order.CryptoCode);

            MessagesComponent.AddMessage(order.UserAccountId, UserType.User, order.Id.ToString(), FiiiPayPushType.TYPE_STOREORDER_CONSUME, titleKey, subTitleKey, order.CryptoCode, content, subTitle, out string noticeId);
        }
        
        public void FiiipayMerchantProfileVerified(FiiiPayMerchantProfileVerified profileMessage)
        {
            var regId = RedisHelper.StringGet($"FiiiPay:Notice:UserId:{profileMessage.AccountId}");
            var lang = RedisHelper.StringGet(REDIS_LANGUAGE_DBINDEX, $"FiiiPay:Language:{profileMessage.AccountId}") ?? "en";
            string titleKey = "", subTitleKey = "";
            int pushType;
            if (profileMessage.VerifyResult == Entities.Enums.VerifyStatus.Certified)
            {
                titleKey = "FiiipayMerchantCertifiedTitle";
                subTitleKey = "FiiipayMerchantCertifiedSubTitle";
                pushType = FiiiPayPushType.FIIIPAY_MERCHANT_VERIFIED;
            }
            else if(profileMessage.VerifyResult == Entities.Enums.VerifyStatus.Disapproval)
            {
                titleKey = "FiiipayMerchantDisapprovalTitle";
                subTitleKey = "FiiipayMerchantDisapprovalSubTitle";
                pushType = FiiiPayPushType.FIIIPAY_MERCHANT_DISAPPROVED;
            }
            else
            {
                return;
            }
            if (!(_resourcePropertyNames.Contains(titleKey) && _resourcePropertyNames.Contains(subTitleKey)))
            {
                throw new Exception($"没有找到资源:{titleKey},{subTitleKey}");
            }
            var content = ResourceHelper.FiiiPay.GetFormatResource(titleKey, lang);
            var subTitle = ResourceHelper.FiiiPay.GetFormatResource(subTitleKey, lang);

            string noticeId = "";

            LogHelper.Info($"--------{lang}------{content}----------{subTitle}");

            MessagesComponent.AddMessage(profileMessage.AccountId, UserType.User, profileMessage.VerifyRecordId.ToString(), pushType, titleKey, subTitleKey, null, content, subTitle, out noticeId);

            RegPush(pushType, new List<string> { regId }, profileMessage.VerifyRecordId, content, subTitle, noticeId);
        }
        

        //private void RegPush(int pushType, List<string> regIdList, object id, string title, string subTitle, string noticeId, bool SendMessage = false)
        //{
        //    PushPayload pushEntity = new PushPayload
        //    {
        //        Audience = new { registration_id = regIdList },
        //        Platform = "all",
        //        Notification = new Notification
        //        {
        //            Alert = title,
        //            IOS = new IOS
        //            {
        //                Alert = new { title = title, body = subTitle },
        //                Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
        //            },
        //            Android = new Android
        //            {
        //                Alert = subTitle,
        //                Title = title,
        //                Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
        //            }
        //        },
        //        Options = new Options { IsApnsProduction = IsPushProduction }
        //    };
        //    if (SendMessage)
        //    {
        //        pushEntity.Message = new Message
        //        {
        //            Title = title,
        //            Content = subTitle,
        //            Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
        //        };
        //    }
        //    var agent = new JPushAgentAsny();
        //    agent.PushAsny(pushEntity);
        //}

        private void RegPushByTags(int pushType, List<string> tags, object id, string title, string subTitle, string noticeId, bool SendMessage = false)
        {
            PushPayload pushEntity = new PushPayload
            {
                Audience = new { tag_and = tags.ToArray() },
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
                Options = new Options { IsApnsProduction = IsPushProduction }

            };
            if (SendMessage)
            {
                pushEntity.Message = new Message
                {
                    Title = title,
                    Content = subTitle,
                    Extras = PushHelper.GetNewExtras(pushType, id, title, subTitle, noticeId)
                };
            }
            var agent = new JPushAgent();
            agent.Push(pushEntity);
        }
    }
}
