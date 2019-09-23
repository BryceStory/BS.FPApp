using System.Collections.Generic;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.SendClient
    /// </summary>
    /// <seealso cref="FiiiPay.Framework.Queue.BaseSendClient" />
    /// <seealso cref="FiiiPay.Framework.Queue.ISender" />
    public class SendClient : BaseSendClient, ISender
    {
        private readonly IList<string> _queues = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SendClient"/> class.
        /// </summary>
        public SendClient() { }

        /// <summary>
        /// Queues the declare.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        protected override void QueueDeclare(string queueName)
        {
            if (!_queues.Contains(queueName))
            {
                Channel.QueueDeclare(queueName, true, false, false, null);
                _queues.Add(queueName);
            }
        }
    }
}
