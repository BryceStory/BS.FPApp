using FiiiPay.Framework.Queue;
using log4net;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public abstract class RedisMQFactory
    {
        public abstract void PubDeposit(long id);

        public abstract void PubDepositCancel(long depositId);
        public abstract void PubWithdrawReject(long withdrawalId);
        public abstract void PubWithdrawCompleted(long withdrawalId);
    }

    public class MerchantRedisMQFactory : RedisMQFactory
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(MerchantRedisMQFactory));

        public override void PubDeposit(long id)
        {
            //MerchantMSMQ.PubMerchantDeposit(id,0);
            RabbitMQSender.SendMessage("MerchantDeposit", id);
            _log.Info($"Send merchant deposit order({id})'s completed message success.");
        }

        public override void PubDepositCancel(long id)
        {
            //MerchantMSMQ.PubMerchantDepositCancel(depositId,0);
            RabbitMQSender.SendMessage("MerchantDepositCancel", id);
            _log.Info($"Send merchant deposit order ({id})'s canceled message success.");
        }

        public override void PubWithdrawReject(long id)
        {
            //MerchantMSMQ.PubMerchantWithdrawReject(withdrawalId,0);
            RabbitMQSender.SendMessage("MerchantWithdrawReject", id);
            _log.Info($"Send merchant withdraw order({id})'s rejected message success.");
        }

        public override void PubWithdrawCompleted(long id)
        {
            //MerchantMSMQ.PubMerchantWithdrawCompleted(withdrawalId,0);
            RabbitMQSender.SendMessage("MerchantWithdrawCompleted", id);
            _log.Info($"Send merchant withdraw order({id})'s completed message success.");
        }
    }

    public class UserRedisMQFactory : RedisMQFactory
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(UserRedisMQFactory));

        public override void PubDeposit(long id)
        {
            //UserMSMQ.PubUserDeposit(id, 0);
            RabbitMQSender.SendMessage("UserDeposit", id);
            _log.Info($"Send user deposit order({id})'s completed message success.");
        }

        public override void PubDepositCancel(long depositId)
        {
            //UserMSMQ.PubUserDepositCancel(depositId, 0);
            RabbitMQSender.SendMessage("UserDepositCancel", depositId);
            _log.Info($"Send user deposit order({depositId})'s canceled message success.");
        }

        public override void PubWithdrawReject(long withdrawalId)
        {
            //UserMSMQ.PubUserWithdrawReject(withdrawalId, 0);
            RabbitMQSender.SendMessage("UserWithdrawReject", withdrawalId);
            _log.Info($"Send user withdraw order({withdrawalId})'s rejected message success.");
        }

        public override void PubWithdrawCompleted(long withdrawalId)
        {
            //UserMSMQ.PubUserWithdrawCompleted(withdrawalId, 0);
            RabbitMQSender.SendMessage("UserWithdrawCompleted", withdrawalId);
            _log.Info($"Send user withdraw order({withdrawalId})'s completed message success.");
        }
    }
}