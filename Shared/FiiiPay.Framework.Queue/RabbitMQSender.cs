using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.RabbitMQSender
    /// </summary>
    public class RabbitMQSender
    {
        private static readonly ConnectionFactory _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(ConfigurationManager.AppSettings.Get("RabbitMQConnectionString")),
            //AutomaticRecoveryEnabled = true,
            //NetworkRecoveryInterval = TimeSpan.FromSeconds(30),
            //TopologyRecoveryEnabled = true
        };
        private static IConnection _connection;
        private static IModel _channel;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public static IConnection Connection => _connection ?? (_connection = _connectionFactory.CreateConnection());

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public static IModel Channel => _channel ?? (_channel = Connection.CreateModel());

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SendMessage<TItem>(string queueName, TItem message) 
        {
            try
            {
                Channel.QueueDeclare(queueName, true, false, false, null);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(messageString);

                //写入
                Channel.BasicPublish("", queueName, properties, body);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SendMessage(string queueName, string message)
        {
            try
            {
                Channel.QueueDeclare(queueName, true, false, false, null);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                //string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(message);

                //写入
                Channel.BasicPublish("", queueName, properties, body);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SendMessage(string queueName, Guid message)
        {
            try
            {
                Channel.QueueDeclare(queueName, true, false, false, null);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                //string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(message.ToString());

                //写入
                Channel.BasicPublish("", queueName, properties, body);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SendMessage(string queueName, long message)
        {
            try
            {
                Channel.QueueDeclare(queueName, true, false, false, null);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                //string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(message.ToString());

                //写入
                Channel.BasicPublish("", queueName, properties, body);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SendMessage(string queueName, int message)
        {
            try
            {
                Channel.QueueDeclare(queueName, true, false, false, null);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                //string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(message.ToString());

                //写入
                Channel.BasicPublish("", queueName, properties, body);
            }
            catch (Exception ex )
            {
                throw ex;
            }
            return true;
        }
        
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <typeparam name="TTtem">The type of the ttem.</typeparam>
        /// <param name="exchange">The exchange.</param>
        /// <param name="type">The type.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SendMessage<TTtem>(string exchange, string type, string routingKey, TTtem message)
            where TTtem : class
        {
            try
            {
                Channel.ExchangeDeclare(exchange, type);

                var properties = Channel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(messageString);

                Channel.BasicPublish(exchange, routingKey, properties, body);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}