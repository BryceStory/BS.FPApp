using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Web;
using FiiiPay.Framework;
using System.Configuration;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Queue;

namespace FiiiPay.Business
{
    public class FiiiEXTransferMSMQ
    {
        private static string FiiiPayIpAddress = ConfigurationManager.AppSettings["FiiiPayMSMQConnectionString"];
        private static string FiiiPosIpAddress = ConfigurationManager.AppSettings["FiiiPosMSMQConnectionString"];

        public static void PubUserTransferFromEx(long orderId, int priority)
        {
            LogHelper.Info($"Send UserTransferFromEx msmq: {FiiiPayIpAddress}UserTransferFromEx: orderId={orderId}");
            RabbitMQSender.SendMessage("UserTransferFromEx", orderId);
            //MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserTransferFromEx", true);
            //msmq.SendMessage<long>(orderId, (MessagePriority)priority);
        }

        public static void PubUserTransferToEx(long orderId, int priority)
        {
            LogHelper.Info($"Send UserTransferToEx msmq: {FiiiPayIpAddress}UserTransferToEx: orderId={orderId}");
            RabbitMQSender.SendMessage("UserTransferToEx", orderId);
            //MSMQHelper msmq = new MSMQHelper(FiiiPayIpAddress + "UserTransferToEx", true);
            //msmq.SendMessage<long>(orderId, (MessagePriority)priority);
        }

        public static void PubMerchantTransferFromEx(long orderId, int priority)
        {
            LogHelper.Info($"Send MerchantTransferFromEx msmq: {FiiiPayIpAddress}MerchantTransferFromEx: orderId={orderId}");
            RabbitMQSender.SendMessage("MerchantTransferFromEx", orderId);
            //MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantTransferFromEx", true);
            //msmq.SendMessage<long>(orderId, (MessagePriority)priority);
        }

        public static void PubMerchantTransferToEx(long orderId, int priority)
        {
            LogHelper.Info($"Send MerchantTransferToEx msmq: {FiiiPayIpAddress}MerchantTransferToEx: orderId={orderId}");
            RabbitMQSender.SendMessage("MerchantTransferToEx", orderId);
            //MSMQHelper msmq = new MSMQHelper(FiiiPosIpAddress + "MerchantTransferToEx", true);
            //msmq.SendMessage<long>(orderId, (MessagePriority)priority);
        }
    }
}
