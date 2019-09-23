using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Framework;
using FiiiPay.Framework.MongoDB;
using FiiiPOS.Business.Properties;
using FiiiPOS.DTO;

namespace FiiiPOS.Business
{
    public class ArticleComponent
    {
        private static readonly string[] VerifyTypeList = new string[] { "KYC_LV1_VERIFIEDTitle", "KYC_LV1_REJECTTitle", "KYC_LV2_VERIFIEDTitle", "KYC_LV2_REJECTTitle", "MerchantLv1Verified", "MerchantLv1Reject", "MerchantLv2Verified", "MerchantLv2Reject" };
        private static readonly string[] VerifyDefaultList = new string[] { "", "", "", "", "Merchant_KYC_LV1_DefaultBody", "", "Merchant_KYC_LV2_DefaultBody", "" };

        public List<SystemMessageES> List(int pageSize, int pageIndex, ArticleAccountType articleAccountType, Guid AccountId, bool isZH)
        {
            string GetDefault(string key)
            {
                return Resources.ResourceManager.GetString(key, new System.Globalization.CultureInfo(isZH ? "zh" : "en"));
            }

            string GetIntro(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return input;
                }
                var reg = new Regex(@"<[^<]+>");
                var reg2 = new Regex(@"&[^&]+;");
                var s = reg.Replace(input, "");
                s = reg2.Replace(s, "");
                s = s.Substring(0, Math.Min(s.Length, 50));
                return s;
            }

            var list = new ArticleDAC().List(pageSize, pageIndex, articleAccountType, AccountId);

            list.ForEach(a =>
            {
                a.Intro = a.Type == SystemMessageESType.Article ? GetIntro(a.Body) : (string.IsNullOrWhiteSpace(a.Body) ? (GetDefault(VerifyDefaultList[int.Parse(a.Attach)])) : a.Body);
                a.Title = a.Type == SystemMessageESType.Article ? a.Title : GetDefault(VerifyTypeList[int.Parse(a.Attach)]);
                if (a.Type == SystemMessageESType.Verify)
                {
                    switch ((VerifyRecordType)int.Parse(a.Attach))
                    {
                        case VerifyRecordType.MerchantLv1Verified:
                            if (string.IsNullOrWhiteSpace(a.Body))
                            {
                                a.Body = Resources.ResourceManager.GetString("Merchant_KYC_LV1_DefaultBody");
                            }
                            break;
                        case VerifyRecordType.MerchantLv1Reject:
                            break;
                        case VerifyRecordType.MerchantLv2Verified:
                            if (string.IsNullOrWhiteSpace(a.Body))
                            {
                                a.Body = Resources.ResourceManager.GetString("Merchant_KYC_LV2_DefaultBody");
                            }
                            break;
                        case VerifyRecordType.MerchantLv2Reject:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });

            return list;
        }

        public void Read(string TargetId, Guid AccountId, ReadRecordType Type)
        {
            var model = new ReadRecord
            {
                AccountId = AccountId,
                TargetId = TargetId,
                Type = Type
            };
            if (new ReadRecordDAC().Exists(model))
            {
                return;
            }
            new ReadRecordDAC().Insert(model);
        }

        public void OnekeyRead(Guid AccountId)
        {
            var list = new ArticleDAC().AllList(ArticleAccountType.FiiiPos, AccountId);
            list.ForEach(data =>
            {
                var model = new ReadRecord
                {
                    AccountId = AccountId,
                    TargetId = data.Id,
                    Type = (ReadRecordType)(int)data.Type
                };
                new ReadRecordDAC().Insert(model);
            });
        }

        public GetFirstTitleAndNotReadCountOM GetFirstTitleAndNotReadCount(ArticleAccountType articleAccountType, Guid AccountId, bool isZH)
        {
            string GetDefault(string key)
            {
                return Resources.ResourceManager.GetString(key, new System.Globalization.CultureInfo(isZH ? "zh" : "en"));
            }
            var tuple = new ArticleDAC().GetFirstTitleAndNotReadCount(articleAccountType, AccountId);

            var model = tuple.Item1;

            return new GetFirstTitleAndNotReadCountOM
            {
                SysCount = tuple.Item2,
                Title = model == null ? null : model.Type == SystemMessageESType.Article ? model.Title : GetDefault(VerifyTypeList[int.Parse(model.Attach)]),
                Timestamp = model?.CreateTime.ToUnixTime().ToString(),
                Count = MessagesComponent.GetMessagesCountByStatus(AccountId, UserType.Merchant, MessageStatus.Normal)
            };
        }

        public string GetNews(int id)
        {
            var model = new ArticleDAC().GetById(id);

            if (model == null)
            {
                return null;
            }

            var template = Resources.NewsTemplate;

            template = template.Replace("[Title]", model.Title);
            template = template.Replace("[PublishTime]", model.CreateTime.ToUnixTime().ToString());
            template = template.Replace("[Content]", model.Body);

            return template;
        }
    }
}
