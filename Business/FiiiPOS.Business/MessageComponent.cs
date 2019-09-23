using System;
using System.Collections.Generic;
using FiiiPay.Business;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Framework;
using FiiiPay.Framework.MongoDB;
using FiiiPOS.DTO.Messages;

namespace FiiiPOS.Business
{
    public class MessageComponent
    {
        public bool ReadMessage(string id, Guid merchantAccountId)
        {
            return MessagesComponent.ReadMessage(id, merchantAccountId);
        }

        public bool ReadMessage(Guid merchantAccountId)
        {
            return MessagesComponent.OnekeyReadMessage(merchantAccountId);
        }

        public bool DeleteMessage(string id, Guid merchantAccountId)
        {
            return MessagesComponent.DeleteMessage(id, merchantAccountId);
        }

        public List<MessageListOMItem> GetListByPage(Guid merchantAccountId, int pageIndex, int size, bool isZH)
        {
            var list = MessagesComponent.GetMessagesByPage(merchantAccountId, UserType.Merchant, pageIndex, size);

            var outPutList = new List<MessageListOMItem>();
            foreach (var entity in list)
            {
                var lang = isZH ? "zh" : "en";
                var content = ResourceHelper.FiiiPos.GetFormatResource(entity.TitleKey, lang, entity.CoinCode);
                var subTitle = ResourceHelper.FiiiPos.GetFormatResource(entity.SubTitleKey, lang, entity.CoinCode);

                var outPutModel = new MessageListOMItem
                {
                    NoticeId = entity._id.ToString(),
                    Type = entity.MsgType,
                    QueryId = entity.QueryId,
                    Title = content,
                    SubTitle = subTitle,
                    Timestamp = entity.CreateTime.ToUnixTime(),
                    Status = entity.Status
                };

                outPutList.Add(outPutModel);
            }

            return outPutList;
        }

        public long GetTotalUnreadMessage(Guid merchantAccountId)
        {
            var count = MessagesComponent.GetMessagesCountByStatus(merchantAccountId, UserType.Merchant,
                MessageStatus.Normal);

            var sysCount = new ArticleDAC().GetUnreadCount(ArticleAccountType.FiiiPos, merchantAccountId);

            return count + sysCount;
        }
    }
}
