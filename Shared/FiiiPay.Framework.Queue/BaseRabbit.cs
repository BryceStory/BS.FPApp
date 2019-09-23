using System;
using System.Configuration;
using RabbitMQ.Client;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.BaseRabbit
    /// </summary>
    public abstract class BaseRabbit : IDisposable
    {
        private static readonly ConnectionFactory _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(ConfigurationManager.AppSettings.Get("RabbitMQConnectionString"))
        };
        private static IConnection _connection;
        private static IModel _channel;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        protected virtual IConnection Connection => _connection ?? (_connection = _connectionFactory.CreateConnection());

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        protected virtual IModel Channel => _channel ?? (_channel = Connection.CreateModel());

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Channel.Close();
            Connection.Close();
            Channel.Dispose();
            Connection.Dispose();
        }

        /// <summary>
        /// Queues the declare.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        protected abstract void QueueDeclare(string queueName);
    }
}
