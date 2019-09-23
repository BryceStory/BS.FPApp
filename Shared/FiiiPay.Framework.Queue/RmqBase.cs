using System;
using System.Configuration;
using RabbitMQ.Client;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.RmqBase
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class RmqBase : IDisposable
    {
        private static IConnection _connection;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        protected IConnection Connection
        {
            get
            {
                if (_connection != null)
                {
                    return _connection;
                }

                var connectionFactory = new ConnectionFactory { Uri = new Uri(ConfigurationManager.AppSettings["RabbitMQConnectionString"]) };

                return _connection = connectionFactory.CreateConnection();
            }
        }
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (_connection == null)
                return;
            _connection.Dispose();
            _connection = null;
        }
    }
}