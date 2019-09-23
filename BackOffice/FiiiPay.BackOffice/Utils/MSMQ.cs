using FiiiPay.Framework;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using System.Web;

namespace FiiiPay.BackOffice.Utils
{
    /*public class MSMQ
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MSMQ));
        private static string FiiiPosIpAddress = ConfigurationManager.AppSettings["FiiiPosMSMQConnectionString"];
        private static string FiiiPayIpAddress = ConfigurationManager.AppSettings["FiiiPayMSMQConnectionString"];

        /// <summary>
        /// 推送给用户的公告
        /// </summary>
        /// <param name="id">公告的Id</param>
        [Obsolete]
        public static void UserArticle(int id, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserArticle", true);
            msmq.SendMessage<int>(id, (MessagePriority)priority);
        }

        /// <summary>
        /// 推送给商家的公告
        /// </summary>
        /// <param name="id">公告的Id</param>
        [Obsolete]
        public static void MerchantArticle(int id, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantArticle", true);
            msmq.SendMessage<int>(id, (MessagePriority)priority);
        }
        [Obsolete]
        public static void BackOfficeUserRefundOrder(string orderno, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "BackOfficeRefundOrder", true);
            msmq.SendMessage<string>(orderno, (MessagePriority)priority);
        }
        [Obsolete]
        public static void BackOfficeMerchantArticleRefundOrder(string orderno, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "BackOfficeRefundOrder", true);
            msmq.SendMessage<string>(orderno, (MessagePriority)priority);
        }
        [Obsolete]
        public static void UserLv1Verified(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserKYC_LV1_VERIFIED", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void UserLv1Reject(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserKYC_LV1_REJECT", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void UserLv2Verified(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserKYC_LV2_VERIFIED", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void UserLv2Reject(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserKYC_LV2_REJECT", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void MerchantLv1Verified(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantLv1Verified", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void MerchantLv1VerifiedFailed(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantLv1VerifiedFailed", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void MerchantLv2Verified(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantLv2Verified", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void MerchantLv2VerifiedFailed(long recordId, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantLv2VerifiedFailed", true);
            msmq.SendMessage<long>(recordId, (MessagePriority)priority);
        }
        [Obsolete]
        public static void PubUserInviteSuccessed(long id, int priority)
        {
            MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserInviteSuccessed", true);
            msmq.SendMessage<long>(id, (MessagePriority)priority);
            _log.Info($"Send user invite order({id})'s successed message success.");
        }
    }*/
}