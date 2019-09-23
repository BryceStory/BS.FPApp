using System;
using FiiiPay.Framework.Queue;
using log4net;

namespace FiiiPay.Business
{
    public class UserMSMQ
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(UserMSMQ));

        public static void PubOrderPayed(Guid orderId, int priority)
        {
            RabbitMQSender.SendMessage("OrderPayed", orderId);
            _log.Info($"Send order({orderId})'s payed message success.");
        }

        public static void PubUserDeposit(long id, int priority)
        {
            RabbitMQSender.SendMessage("UserDeposit", id);
            _log.Info($"Send user deposit order({id})'s completed message success.");
        }
        //public static void PubUserDepositCancel(long id, int priority)
        //{
        //    RabbitMQSender.SendMessage("UserDepositCancel", id);
        //    _log.Info($"Send user deposit order({id})'s canceled message success.");
        //}
        public static void PubUserWithdrawCompleted(long id, int priority)
        {
            RabbitMQSender.SendMessage("UserWithdrawCompleted", id);
            _log.Info($"Send user withdraw order({id})'s completed message success.");
        }
        public static void PubUserWithdrawReject(long id, int priority)
        {
            RabbitMQSender.SendMessage("UserWithdrawReject", id);
            _log.Info($"Send user withdraw order({id})'s rejected message success.");
        }

        public static void PubUserInviteSuccessed(long id, int priority)
        {
            RabbitMQSender.SendMessage("UserInviteSuccessed", id);
            _log.Info($"Send user invite order({id})'s successed message success.");
        }

        public static void PubMerchantInviteSuccessed(long id, int priority)
        {
            RabbitMQSender.SendMessage("MerchantInviteSuccessed", id);
            _log.Info($"Send user invite order({id})'s successed message success.");
        }

        public static void PubConsumeOrder(Guid orderId)
        {
            RabbitMQSender.SendMessage("ConsumeOrder", orderId);
            _log.Info($"Send consume order({orderId})'s successed message success.");
        }
    }
}
