using System;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.DelayWorker
    /// </summary>
    /// <seealso cref="FiiiPay.Framework.Queue.Worker" />
    public class DelayWorker : Worker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayWorker"/> class.
        /// </summary>
        public DelayWorker() { }

        /// <summary>
        /// Starts the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="action">The action.</param>
        public override void Start(string queueName, Func<string, bool> action)
        {
            base.Start(queueName + "-delay", action);
        }
    }
}
