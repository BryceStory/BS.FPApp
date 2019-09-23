using System;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Interface FiiiPay.Framework.Queue.IMessageQueueWorker
    /// </summary>
    public interface IMessageQueueWorker
    {
        /// <summary>
        /// Starts the consuming.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="handleMessageAction">The handle message action.</param>
        void StartConsuming<T>(string queueName, Action<T> handleMessageAction);

        /// <summary>
        /// Stops the consuming.
        /// </summary>
        void StopConsuming();
    }
}