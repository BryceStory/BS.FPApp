using FiiiPay.Entities;
using FiiiPay.Framework.Queue;
using log4net;

namespace FiiiPOS.Business
{
    public class MerchantMSMQ
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MerchantMSMQ));
        
        public static void PubPayOrder(SendPayOrderModel model, int priority)
        {
            RabbitMQSender.SendMessage<SendPayOrderModel>("PayOrder", model);
            _log.Info($"Send order({model.OrderNo})'s paying message success.");
        }
        public static void PubRefundOrder(string orderno, int priority)
        {
            RabbitMQSender.SendMessage("RefundOrder", orderno);
            //MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "RefundOrder", true);
            //msmq.SendMessage<string>(orderno, (MessagePriority)priority);
            _log.Info($"Send order({orderno})'s refunded message success.");
        }
        public static void PubMerchantDeposit(long id, int priority)
        {
            RabbitMQSender.SendMessage("MerchantDeposit", id);
            _log.Info($"Send merchant deposit order({id})'s completed message success.");
        }
        public static void PubMerchantDepositCancel(long id, int priority)
        {
            RabbitMQSender.SendMessage("MerchantDepositCancel", id);
            _log.Info($"Send merchant deposit order ({id})'s canceled message success.");
        }
        public static void PubMerchantWithdrawCompleted(long id, int priority)
        {
            RabbitMQSender.SendMessage("MerchantWithdrawCompleted", id);
            _log.Info($"Send merchant withdraw order({id})'s completed message success.");
        }
        public static void PubMerchantWithdrawReject(long id, int priority)
        {
            RabbitMQSender.SendMessage("MerchantWithdrawReject", id);
            _log.Info($"Send merchant withdraw order({id})'s rejected message success.");
        }
        public static void PubUserDeposit(long id, int priority)
        {
            RabbitMQSender.SendMessage("UserDeposit", id);
            _log.Info($"Send user deposit order({id})'s completed message success.");
        }
        public static void PubUserInviteSuccessed(long id, int priority)
        {
            RabbitMQSender.SendMessage("MerchantInviteSuccessed", id);
            _log.Info($"Send user invite order({id})'s successed message success.");
        }
    }

    public class PayOrderModel
    {
        public string OrderNo { get; set; }

        public string UserAccountId { get; set; }
    }
}
