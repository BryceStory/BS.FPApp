using System.Collections.Generic;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.DelaySendClient
    /// </summary>
    /// <seealso cref="FiiiPay.Framework.Queue.BaseSendClient" />
    /// <seealso cref="FiiiPay.Framework.Queue.ISender" />
    public class DelaySendClient : BaseSendClient, ISender
    {
        private readonly IList<string> _queues = new List<string>();

        private readonly int _delayTtl;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelaySendClient"/> class.
        /// </summary>
        /// <param name="delayTTL">The delay TTL.</param>
        public DelaySendClient(int delayTTL)
        {
            _delayTtl = delayTTL;
        }

        /// <summary>
        /// Queues the declare.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        protected override void QueueDeclare(string queueName)
        {
            if (!_queues.Contains(queueName))
            {
                var delayQueue = queueName + "-delay";
                var inDelayArguments = new Dictionary<string, object>
                {
                    {"x-message-ttl", _delayTtl}, {"x-dead-letter-exchange", ""}, {"x-dead-letter-routing-key", delayQueue}
                };

                Channel.QueueDeclare(queueName, true, false, false, inDelayArguments);

                Channel.QueueDeclare(delayQueue, true, false, false, null);

                _queues.Add(queueName);
            }
        }
    }
}
