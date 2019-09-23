using System.Configuration;
using FiiiPay.Framework.Queue;

namespace FiiiPay.Business
{
    public class InvestorMSMQ
    {
        private static string FiiiPayIpAddress = ConfigurationManager.AppSettings["FiiiPayMSMQConnectionString"];
        public static void PubUserDeposit(long orderId, int priority)
        {
            RabbitMQSender.SendMessage("UserDeposit", orderId);
            //MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserDeposit", false);
            //msmq.SendMessage<long>(orderId, (MessagePriority)priority);
        }
    }
}
