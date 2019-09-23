namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Interface FiiiPay.Framework.Queue.IMessageQueueSender
    /// </summary>
    public interface IMessageQueueSender
    {
        /// <summary>
        /// Pushes the specified queue name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        void Push<T>(string queueName, T message);
    }
}