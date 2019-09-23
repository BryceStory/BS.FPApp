using System;
using System.Text;
using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.RmqMessageQuqueWorker
    /// </summary>
    /// <seealso cref="FiiiPay.Framework.Queue.RmqBase" />
    /// <seealso cref="FiiiPay.Framework.Queue.IMessageQueueWorker" />
    public sealed class RmqMessageQueueWorker : RmqBase, IMessageQueueWorker
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RabbitMQWorker));

        private IModel _consumeChannel;

        private IModel ConsumeChannel
        {
            get
            {
                if (_consumeChannel != null)
                {
                    return _consumeChannel;
                }

                return _consumeChannel = Connection.CreateModel();
            }
        }


        /// <summary>
        /// Starts the consuming.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="handleMessageAction">The handle message action.</param>
        public void StartConsuming<T>(string queueName, Action<T> handleMessageAction)
        {
            ConsumeChannel.QueueDeclare(queueName, true, false, false, null);

            ConsumeChannel.BasicQos(0, 1, false);

            EventingBasicConsumer consumer = new EventingBasicConsumer(ConsumeChannel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var messageString = Encoding.UTF8.GetString(body);
                T message = JsonConvert.DeserializeObject<T>(messageString);

                _log.InfoFormat("Received {0}", message);
                handleMessageAction(message);
                _log.InfoFormat("Process {0}", message);
                ConsumeChannel.BasicAck(ea.DeliveryTag, false);
            };

            ConsumeChannel.BasicConsume(queueName, false, consumer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="handleMessageAction"></param>
        /// <param name="exchangeType"></param>
        /// <typeparam name="T"></typeparam>
        public void StartConsuming<T>(string exchange, string routingKey, Action<T> handleMessageAction, string exchangeType = ExchangeType.Direct)
        {
            var queue = ConsumeChannel.QueueDeclare();
            ConsumeChannel.ExchangeDeclare(exchange, exchangeType);
            ConsumeChannel.BasicQos(0, 1, false);
            ConsumeChannel.QueueBind(queue.QueueName, exchange, routingKey);

            EventingBasicConsumer consumer = new EventingBasicConsumer(ConsumeChannel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var messageString = Encoding.UTF8.GetString(body);
                T message = JsonConvert.DeserializeObject<T>(messageString);

                _log.InfoFormat("Received {0}", message);
                handleMessageAction(message);
                _log.InfoFormat("Process {0}", message);
                ConsumeChannel.BasicAck(ea.DeliveryTag, false);
            };

            ConsumeChannel.BasicConsume(queue.QueueName, false, consumer);
        }


        /// <summary>
        /// Stops the consuming.
        /// </summary>
        public void StopConsuming()
        {
            Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (_consumeChannel != null)
            {
                if (!_consumeChannel.IsClosed)
                {
                    _consumeChannel.Close(200, "StopConsuming");
                }

                _consumeChannel?.Dispose();
                _consumeChannel = null;
            }

            base.Dispose();
        }

    }
}