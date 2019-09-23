using FiiiPay.Framework.Queue;

namespace FiiiPos.InviteReward
{
    public class MessagePushService
    {
        public static void PubUserInviteSuccessed(long id, int priority)
        {
            LogHelper.Info($"SendMessage:UserInviteSuccessed,id:{id}");
            RabbitMQSender.SendMessage("UserInviteSuccessed", id);
        }
    }
}
