using System;
using System.Collections.Generic;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.DeadLetterClient
    /// </summary>
    /// <seealso cref="FiiiPay.Framework.Queue.BaseSendClient" />
    /// <seealso cref="FiiiPay.Framework.Queue.ISender" />
    public class DeadLetterClient : BaseSendClient, ISender
    {
        private readonly IList<string> _queues = new List<string>();

        private readonly int _deadLetterTtl;

        /// <summary>
        /// Gets the retry TTL.
        /// </summary>
        /// <value>
        /// The retry TTL.
        /// </value>
        public override int RetryTTL => 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadLetterClient"/> class.
        /// </summary>
        /// <param name="deadLetterTTL">The TTL.</param>
        public DeadLetterClient(int deadLetterTTL)
        {
            _deadLetterTtl = deadLetterTTL;
        }

        /// <summary>
        /// Queues the declare.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        protected override void QueueDeclare(string queueName)
        {
            if (!_queues.Contains(queueName))
            {
                var deadQueueName = queueName + "-retry";

                var inDeadArguments = new Dictionary<string, object>
                {
                    {"x-message-ttl", _deadLetterTtl}, {"x-dead-letter-exchange", ""}, {"x-dead-letter-routing-key", deadQueueName}
                };
                var retryArguments = new Dictionary<string, object>
                {
                    {"x-message-ttl", RetryTTL}, {"x-dead-letter-exchange", ""}, {"x-dead-letter-routing-key", queueName}
                };

                Channel.QueueDeclare(queueName, true, false, false, inDeadArguments);

                Channel.QueueDeclare(deadQueueName, true, false, false, retryArguments);

                _queues.Add(queueName);
            }
        }
    }
}
