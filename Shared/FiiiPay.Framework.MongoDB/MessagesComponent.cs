using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace FiiiPay.Framework.MongoDB
{
    /// <summary>
    /// Class FiiiPay.Framework.MongoDB.MessagesComponent
    /// </summary>
    public class MessagesComponent
    {
        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userType">Type of the user.</param>
        /// <param name="queryId">The query identifier.</param>
        /// <param name="msgType">Type of the MSG.</param>
        /// <param name="titleKey">The title key.</param>
        /// <param name="subTitleKey">The sub title key.</param>
        /// <param name="coinCode">The coin code.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        /// <param name="noticeId"></param>
        /// <returns></returns>
        public static bool AddMessage(Guid userId, UserType userType, string queryId, int msgType, string titleKey, string subTitleKey, string coinCode, string title, string body, out string noticeId)
        {
            var flag = ObjectId.GenerateNewId();
            var messages = new Messages
            {
                _id = flag,
                UserId = userId.ToString(),
                UserType = (int)userType,
                QueryId = queryId,
                MsgType = msgType,
                TitleKey = titleKey,
                SubTitleKey = subTitleKey,
                CoinCode = coinCode,
                Title = title,
                Body = body,
                Status = 0,

                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            noticeId = flag.ToString();
            return MongoDBHelper.AddSignleObject(messages);
        }

        /// <summary>
        /// Reads the message.
        /// </summary>
        /// <param name="objId">The object identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static bool ReadMessage(string objId, Guid userId)
        {
            var model = new Messages
            {
                Status = (int)MessageStatus.HasRead,
                UpdateTime = DateTime.Now
            };
            var result = MongoDBHelper.UpdateMany(x => x._id == new ObjectId(objId) && x.UserId == userId.ToString(), model, new List<string> { "Status", "UpdateTime" });
            return result > 0;
        }

        /// <summary>
        /// Onekey Reads the message.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static bool OnekeyReadMessage(Guid userId)
        {
            var model = new Messages
            {
                Status = (int)MessageStatus.HasRead,
                UpdateTime = DateTime.Now
            };
            var result = MongoDBHelper.UpdateMany(x => x.UserId == userId.ToString() && x.Status == (int)MessageStatus.Normal, model, new List<string> { "Status", "UpdateTime" });
            return result > 0;
        }

        /// <summary>
        /// 删掉消息
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool DeleteMessage(string objId, Guid userId)
        {
            var model = new Messages
            {

                Status = (int)MessageStatus.HasDelete,
                UpdateTime = DateTime.Now
            };

            //int result = MongoDBHelper.UpdateSingle<Messages>(x => x._id == new ObjectId(objId) && x.UserId == userId.ToString(), "Status", MessageStatus.HasDelete.ToString());
            var result = MongoDBHelper.UpdateMany(x => x._id == new ObjectId(objId) && x.UserId == userId.ToString(), model, new List<string> { "Status", "UpdateTime" });
            return result > 0;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<Messages> GetMessagesByPage(Guid userId, UserType userType, int pageIndex, int pageSize)
        {
            Expression<Func<Messages, bool>> filter = x => x.UserId == userId.ToString() && x.UserType == (int)userType && x.Status < (int)MessageStatus.HasDelete;

            var list = MongoDBHelper.FindMany(filter, pageIndex, pageSize);
            return list;
        }

        /// <summary>
        /// 获取状态数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userType"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static long GetMessagesCountByStatus(Guid userId, UserType userType, MessageStatus status)
        {
            Expression<Func<Messages, bool>> filter = x => x.UserId == userId.ToString() && x.UserType == (int)userType && x.Status == (int)status;

            var count = MongoDBHelper.GetCount(filter);

            return count;
        }
    }

    /// <summary>
    /// Enum FiiiPay.Framework.MongoDB.MessageStatus
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// The normal
        /// </summary>
        Normal,    //未读
        /// <summary>
        /// The has read
        /// </summary>
        HasRead,   //已读
        /// <summary>
        /// The has delete
        /// </summary>
        HasDelete  //已删
    }

    /// <summary>
    /// Enum FiiiPay.Framework.MongoDB.UserType
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// The user
        /// </summary>
        User,    //用户
        /// <summary>
        /// The merchant
        /// </summary>
        Merchant //商家
    }
}
