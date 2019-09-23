using System;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Interface FiiiPay.Framework.Queue.ISender
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Send(string queueName, string message);

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Send(string queueName, int message);

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Send(string queueName, long message);

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Send(string queueName, Guid message);

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool Send<T>(string queueName, T message);
    }
}
