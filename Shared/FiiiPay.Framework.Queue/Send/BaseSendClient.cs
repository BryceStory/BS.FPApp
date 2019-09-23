using System;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.Send.BaseSendClient
    /// </summary>
    public abstract class BaseSendClient : BaseRabbit
    {
        /// <summary>
        /// Gets the retry TTL.
        /// </summary>
        /// <value>
        /// The retry TTL.
        /// </value>
        public virtual int RetryTTL => 60000;

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual bool Send(string queueName, string message)
        {
            try
            {
                QueueDeclare(queueName);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true;

                var body = Encoding.UTF8.GetBytes(message);

                Channel.BasicPublish("", queueName, properties, body);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual bool Send(string queueName, int message)
        {
            return Send(queueName, message.ToString());
        }

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual bool Send(string queueName, long message)
        {
            return Send(queueName, message.ToString());
        }

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual bool Send(string queueName, Guid message)
        {
            return Send(queueName, message.ToString());
        }

        /// <summary>
        /// Sends the specified queue name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public virtual bool Send<T>(string queueName, T message)
        {
            return Send(queueName, JsonConvert.SerializeObject(message));
        }
    }
}
