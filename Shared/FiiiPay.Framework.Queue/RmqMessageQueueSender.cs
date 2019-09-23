using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.RmqMessageQueueSender
    /// </summary>
    /// <seealso cref="FiiiPay.Framework.Queue.RmqBase" />
    /// <seealso cref="FiiiPay.Framework.Queue.IMessageQueueSender" />
    public sealed class RmqMessageQueueSender : RmqBase, IMessageQueueSender
    {
        /// <summary>
        /// Pushes the specified queue name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        public void Push<T>(string queueName, T message)
        {
            using (var publishChannel = Connection.CreateModel())
            {
                publishChannel.QueueDeclare(queueName, true, false, false, null);

                var properties = publishChannel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                string serializeMessage = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(serializeMessage);

                //写入
                publishChannel.BasicPublish("", queueName, properties, body);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <param name="exchangeType"></param>
        /// <typeparam name="T"></typeparam>
        public void Push<T>(string exchange, string routingKey, T message, string exchangeType = ExchangeType.Direct)
        {
            using (var publishChannel = Connection.CreateModel())
            {
                publishChannel.ExchangeDeclare(exchange, exchangeType);

                var properties = publishChannel.CreateBasicProperties();
                properties.Persistent = true; //使消息持久化

                string messageString = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(messageString);

                publishChannel.BasicPublish(exchange, routingKey, properties, body);

            }
        }
    }
}