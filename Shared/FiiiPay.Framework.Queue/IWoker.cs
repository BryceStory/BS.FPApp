using System;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Interface FiiiPay.Framework.Queue.IWoker
    /// </summary>
    public interface IWoker
    {
        /// <summary>
        /// Starts the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        void Start(string queueName, Func<string, bool> action);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns></returns>
        void Stop();
    }
}
