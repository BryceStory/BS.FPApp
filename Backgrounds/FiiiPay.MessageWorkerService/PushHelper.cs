using System;
using System.Collections.Generic;
using FiiiPay.Framework;

namespace FiiiPay.MessageWorkerService
{
    public static class PushHelper
    {
        public static Dictionary<string, object> GetNewExtras(int type, object queryId, string title = null, string subTitle = null, string noticeId = null)
        {
            return new Dictionary<string, object> {
                { "NoticeId",string.IsNullOrWhiteSpace(noticeId) ? Guid.NewGuid().ToString() : noticeId},
                { "Timestamp", DateTime.UtcNow.ToUnixTime().ToString()},
                { "Type", type},
                { "QueryId", queryId.ToString()},
                { "Title", title},
                { "SubTitle", subTitle},
            };
        }
    }
}
